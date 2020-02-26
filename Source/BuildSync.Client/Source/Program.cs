/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using BuildSync.Client.Commands;
using BuildSync.Client.Forms;
using BuildSync.Core;
using BuildSync.Core.Client;
using BuildSync.Core.Downloads;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Scm;
using BuildSync.Core.Scm.Git;
using BuildSync.Core.Scm.Perforce;
using BuildSync.Core.Utils;
using CommandLine;

namespace BuildSync.Client
{
    /// <summary>
    /// </summary>
    /// <returns>True if pump loop should terminate.</returns>
    public delegate bool PumpLoopEventHandler();

    /// <summary>
    ///     Application entry class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// </summary>
        public static MainForm AppForm;

        /// <summary>
        /// </summary>
        public static VirtualFileSystem BuildFileSystem;

        /// <summary>
        /// </summary>
        public static BuildManifestRegistry BuildRegistry;

        /// <summary>
        /// </summary>
        public static DownloadManager DownloadManager;

        /// <summary>
        /// </summary>
        public static bool InCommandLineMode = false;

        /// <summary>
        /// </summary>
        public static AsyncIOQueue IOQueue;

        /// <summary>
        /// </summary>
        public static CommandIPC IPCServer = new CommandIPC("buildsync-client", false);

        /// <summary>
        /// </summary>
        public static ManifestDownloadManager ManifestDownloadManager;

        /// <summary>
        /// </summary>
        public static Core.Client.Client NetClient;

        /// <summary>
        /// </summary>
        public static bool RespondingToIpc;

        /// <summary>
        /// </summary>
        public static ScmManager ScmManager;

        /// <summary>
        /// </summary>
        public static Settings Settings;

        /// <summary>
        /// </summary>
        private static string CurrentStoragePath = "";

        /// <summary>
        /// </summary>
        private static bool ForceSaveSettingsPending;

        /// <summary>
        /// </summary>
        private static DownloadState InternalUpdateDownload;

        /// <summary>
        /// </summary>
        private static Guid InternalUpdateManifestId = Guid.Empty;

        /// <summary>
        /// </summary>
        private static ulong LastSettingsSaveTime = TimeUtils.Ticks;

        /// <summary>
        /// </summary>
        private const int MinimumTimeBetweenSettingsSaves = 60 * 1000;

        /// <summary>
        /// </summary>
        private static string SettingsPath;

        /// <summary>
        /// </summary>
        public static string AppDataDir
        {
            get
            {
                string Namespace = "Client";

#if !SHIPPING
                int ProcessCount = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length;
                if (ProcessCount > 1)
                {
                    Namespace = "Client." + (ProcessCount - 1);
                }
#endif

                return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BuildSync"), Namespace);
            }
        }

        /// <summary>
        /// </summary>
        public static bool AutoUpdateAvailable { get; set; } = false;

        /// <summary>
        /// </summary>
        public static void ApplySettings()
        {
            // Server settings.
            if (NetClient != null)
            {
                if (NetClient.ServerHostname != Settings.ServerHostname ||
                    NetClient.ServerPort != Settings.ServerPort ||
                    NetClient.PeerListenPortRangeMin != Settings.ClientPortRangeMin ||
                    NetClient.PeerListenPortRangeMax != Settings.ClientPortRangeMax)
                {
                    NetClient.ServerHostname = Settings.ServerHostname;
                    NetClient.ServerPort = Settings.ServerPort;
                    NetClient.PeerListenPortRangeMin = Settings.ClientPortRangeMin;
                    NetClient.PeerListenPortRangeMax = Settings.ClientPortRangeMax;

                    NetClient.RestartConnections();
                    SaveSettings();
                }
            }

            // Storage settings.
            if (Settings.StoragePath != CurrentStoragePath)
            {
                MoveStorageDirectoryForm Dialog = new MoveStorageDirectoryForm(CurrentStoragePath, Settings.StoragePath);
                if (Dialog.ShowDialog() != DialogResult.OK)
                {
                    Settings.StoragePath = CurrentStoragePath;
                }
                else
                {
                    CurrentStoragePath = Settings.StoragePath;
                }

                SaveSettings();
            }

            if (ManifestDownloadManager != null && Settings.StorageMaxSize != ManifestDownloadManager.StorageMaxSize)
            {
                ManifestDownloadManager.StorageMaxSize = Settings.StorageMaxSize;

                SaveSettings();
            }

            // Add SCM Providers.
            foreach (ScmWorkspaceSettings ScmSettings in Settings.ScmWorkspaces)
            {
                // Check if a provider already exists for this.
                bool Exists = false;
                foreach (IScmProvider Provider in ScmManager.Providers)
                {
                    if (Provider.Server == ScmSettings.Server &&
                        Provider.Username == ScmSettings.Username &&
                        Provider.Password == ScmSettings.Password &&
                        FileUtils.NormalizePath(Provider.Root) == FileUtils.NormalizePath(ScmSettings.Location))
                    {
                        Exists = true;
                        break;
                    }
                }

                if (Exists)
                {
                    continue;
                }

                // Add new provider.
                switch (ScmSettings.ProviderType)
                {
                    case ScmProviderType.Perforce:
                    {
                        ScmManager.AddProvider(new PerforceScmProvider(ScmSettings.Server, ScmSettings.Username, ScmSettings.Password, ScmSettings.Location));
                        break;
                    }
                    case ScmProviderType.Git:
                    {
                        ScmManager.AddProvider(new GitScmProvider(ScmSettings.Server, ScmSettings.Username, ScmSettings.Password, ScmSettings.Location));
                        break;
                    }
                    default:
                    {
                        Debug.Assert(false);
                        break;
                    }
                }
            }

            // Remove old providers.
            foreach (IScmProvider Provider in ScmManager.Providers.ToArray())
            {
                bool Exists = false;
                foreach (ScmWorkspaceSettings ScmSettings in Settings.ScmWorkspaces)
                {
                    if (Provider.Server == ScmSettings.Server &&
                        Provider.Username == ScmSettings.Username &&
                        Provider.Password == ScmSettings.Password &&
                        FileUtils.NormalizePath(Provider.Root) == FileUtils.NormalizePath(ScmSettings.Location))
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    ScmManager.RemoveProvider(Provider);
                }
            }

            // Bandwidth settings.
            NetConnection.GlobalBandwidthThrottleIn.MaxRate = Settings.BandwidthMaxDown;
            NetConnection.GlobalBandwidthThrottleOut.MaxRate = Settings.BandwidthMaxUp;

            // General settings.
            ManifestDownloadManager.SkipValidation = Settings.SkipValidation;
            ManifestDownloadManager.SkipDiskAllocation = Settings.SkipDiskAllocation;

            // General settings.
            ProcessUtils.SetLaunchOnStartup("Build Sync - Client", Settings.RunOnStartup);
            Logger.MaximumVerbosity = Program.Settings.LoggingLevel;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static bool AreDownloadsAllowed()
        {
            bool Result = false;

            if (Settings.BandwidthStartTimeHour != 0 || Settings.BandwidthStartTimeMin != 0 ||
                Settings.BandwidthEndTimeHour != 0 || Settings.BandwidthEndTimeMin != 0)
            {
                TimeSpan Start = new TimeSpan(Settings.BandwidthStartTimeHour, Settings.BandwidthStartTimeMin, 0);
                TimeSpan End = new TimeSpan(Settings.BandwidthEndTimeHour, Settings.BandwidthEndTimeMin, 0);
                TimeSpan Now = DateTime.Now.TimeOfDay;

                // Start and stop times are in the same day
                if (Start <= End)
                {
                    Result = Now >= Start && Now <= End;
                }
                // Start and stop times are in different days
                else
                {
                    Result = Now >= Start || Now <= End;
                }
            }
            else
            {
                Result = true;
            }

            return Result;
        }

        /// <summary>
        /// </summary>
        public static void InstallAutoUpdate()
        {
            ManifestDownloadState AutoUpdateDownload = null;
            if (InternalUpdateDownload != null)
            {
                AutoUpdateDownload = ManifestDownloadManager.GetDownload(InternalUpdateDownload.ActiveManifestId);
            }

            if (AutoUpdateDownload == null)
            {
                Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to install update, no internal download available.");
                return;
            }

            string InstallerPath = Path.Combine(AutoUpdateDownload.LocalFolder, "installer.msi");
            if (!File.Exists(InstallerPath))
            {
                MessageBox.Show("Buildsync installer cannot be found in update download. Update is likely corrupt", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Boot up the installer.
                try
                {
                    IOQueue.CloseAllStreamsInDirectory(AutoUpdateDownload.LocalFolder);

                    Process p = new Process();
                    p.StartInfo.FileName = "msiexec";
                    p.StartInfo.Arguments = "/i \"" + InstallerPath + "\"";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.WorkingDirectory = AutoUpdateDownload.LocalFolder;
                    p.Start();

                    //Process.Start(InstallerPath, "/passive /norestart REINSTALL=ALL REINSTALLMODE=A MSIRMSHUTDOWN=1 MSIDISABLERMRESTART=0 ADDLOCAL=All");

                    Application.Exit();
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to install update '{0}' due to error: {1}", InstallerPath, Ex.Message);
                }
            }
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Logger.SetupStandardLogging(Path.Combine(AppDataDir, "Logging"), InCommandLineMode);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if SHIPPING
            try
            {
                if (WindowUtils.BringOtherAppInstanceToFront("Build Sync - Client"))
                {
                    return;
                }

                using (new SingleGlobalInstance(100))
                {
#endif
            BuildSettings.Init();

            OnStart();

            /*
                    PollTimer = new System.Timers.Timer(20);
                    PollTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
                    {
                        if (Monitor.TryEnter(PollTimer))
                        {
                            try
                            {
                                // Make sure it invokes on main thread, maybe spend some time
                                // make all the ui->program interaction thread safe?
                                if (AppForm != null && AppForm.IsHandleCreated && !AppForm.IsDisposed && !AppForm.Disposing)
                                {
                                    try
                                    {
                                        AppForm.Invoke((MethodInvoker)(() =>
                                        {
                                            OnPoll();
                                        }));
                                    }
                                    catch (ObjectDisposedException)
                                    {
                                        // Ignore ...
                                    }
                                }
                            }
                            finally
                            {
                                Monitor.Exit(PollTimer);
                            }
                        }
                    };
                    PollTimer.Start();
            */

            AppForm = new MainForm();
            Application.Run(AppForm);

            OnStop();
#if SHIPPING
                }
            }
            catch (TimeoutException Ex)
            {
                MessageBox.Show("Application is already running.", "Already Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#endif
        }

        /// <summary>
        /// </summary>
        public static void OnPoll()
        {
            NetClient.TrafficEnabled = AreDownloadsAllowed();
            ManifestDownloadManager.TrafficEnabled = AreDownloadsAllowed();

            ScmManager.Poll();
            NetClient.Poll();
            DownloadManager.Poll(NetClient.IsReadyForData);
            ManifestDownloadManager.Poll();

            // Update save data if download states have changed recently.
            if (TimeUtils.Ticks - LastSettingsSaveTime > MinimumTimeBetweenSettingsSaves || ForceSaveSettingsPending)
            {
                if (ManifestDownloadManager.AreStatesDirty || DownloadManager.AreStatesDirty || ForceSaveSettingsPending)
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Main, "Saving settings.");

                    Settings.DownloadStates = DownloadManager.States;
                    Settings.ManifestDownloadStates = ManifestDownloadManager.States;
                    SaveSettings(true);

                    ManifestDownloadManager.AreStatesDirty = false;
                    DownloadManager.AreStatesDirty = false;

                    ForceSaveSettingsPending = false;
                    LastSettingsSaveTime = TimeUtils.Ticks;
                }
            }

            PollIpcServer();
            PollAutoUpdate();
            RecordPeerStates();
        }

        /// <summary>
        /// </summary>
        public static void OnStart()
        {
            Statistic.Instantiate();

            ScmManager = new ScmManager();
            IOQueue = new AsyncIOQueue();
            ManifestDownloadManager = new ManifestDownloadManager();
            DownloadManager = new DownloadManager();

            InitSettings();

            NetClient = new Core.Client.Client();

            BuildRegistry = new BuildManifestRegistry();
            BuildRegistry.Open(Path.Combine(Settings.StoragePath, "Manifests"), int.MaxValue, true);

            NetClient.Start(
                Settings.ServerHostname,
                Settings.ServerPort,
                Settings.ClientPortRangeMin,
                Settings.ClientPortRangeMax,
                BuildRegistry,
                ManifestDownloadManager
            );

            // Setup the virtual file system we will store our available builds in.
            BuildFileSystem = new VirtualFileSystem();
            NetClient.OnPermissionsUpdated += () =>
            {
                BuildFileSystem.ForceRefresh();
                DownloadManager.ForceRefresh();
            };
            NetClient.OnConnectedToServer += () => { BuildFileSystem.ForceRefresh(); };
            NetClient.OnBuildsRecieved += (RootPath, Builds) =>
            {
                List<VirtualFileSystemInsertChild> NewChildren = new List<VirtualFileSystemInsertChild>();
                foreach (NetMessage_GetBuildsResponse.BuildInfo Build in Builds)
                {
                    NewChildren.Add(
                        new VirtualFileSystemInsertChild
                        {
                            VirtualPath = Build.VirtualPath,
                            CreateTime = Build.Guid == Guid.Empty ? DateTime.UtcNow : Build.CreateTime,
                            Metadata = Build.Guid
                        }
                    );
                }

                BuildFileSystem.ReconcileChildren(RootPath, NewChildren);
            };
            BuildFileSystem.OnRequestChildren += (FileSystem, Path) =>
            {
                if (NetClient.IsConnected)
                {
                    NetClient.RequestBuilds(Path);
                }
            };
            BuildFileSystem.Init();

            // Setup download managers for the manifest and app level.
            ManifestDownloadManager.Start(
                Path.Combine(Settings.StoragePath, "Builds"),
                Settings.StorageMaxSize,
                Settings.ManifestDownloadStates,
                BuildRegistry,
                IOQueue
            );

            DownloadManager.Start(
                ManifestDownloadManager,
                Settings.DownloadStates,
                BuildFileSystem,
                ScmManager
            );

            // Ensure we are downloading the latest update.
            string UpdateDownloadName = "$ Buildsync Update $";

            foreach (DownloadState State in DownloadManager.States.States)
            {
                if (State.Name == UpdateDownloadName)
                {
                    InternalUpdateDownload = State;
                    break;
                }
            }

            if (InternalUpdateDownload == null)
            {
                InternalUpdateDownload = DownloadManager.AddDownload(UpdateDownloadName, "$Internal$/Updates", 2, BuildSelectionRule.Newest, BuildSelectionFilter.None, "", "", true, false, "", "");
            }

            // Make sure we have to get the latest manifest id before updating.
            InternalUpdateDownload.ActiveManifestId = Guid.Empty;
        }

        /// <summary>
        /// </summary>
        public static void OnStop()
        {
            SaveSettings(true);

            if (NetClient != null)
            {
                NetClient.Disconnect();
                NetClient = null;
            }

            IOQueue.Stop();
        }

        /// <summary>
        /// </summary>
        public static void PollAutoUpdate()
        {
#if SHIPPING
            if (InternalUpdateDownload != null)
            {
                ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(InternalUpdateDownload.ActiveManifestId);
                if (Downloader != null && Downloader.State == ManifestDownloadProgressState.Complete && Downloader.Manifest != null)
                {
                    int VersionNumber = StringUtils.ConvertSemanticVerisonNumber(VirtualFileSystem.GetNodeName(Downloader.Manifest.VirtualPath));

                    if (AppVersion.VersionNumber < VersionNumber)
                    {
                        //Logger.Log(LogLevel.Info, LogCategory.Main, "Installing new update: {0}", Downloader.Manifest.VirtualPath);
                        AutoUpdateAvailable = true;
                        //InstallAutoUpdate();
                    }
                }
                else
                {
                    AutoUpdateAvailable = false;
                }
            }
#endif
        }

        /// <summary>
        /// </summary>
        public static void RecordPeerStates()
        {
            Peer[] AllPeers = NetClient.AllPeers;

            foreach (PeerSettingsRecord Record in Settings.PeerRecords)
            {
                Record.AverageRateIn = 0;
                Record.AverageRateOut = 0;

                Record.TargetInFlightData = 0;
                Record.CurrentInFlightData = 0;
            }

            foreach (Peer Peer in AllPeers)
            {
                if (Peer.Connection.Address == null)
                {
                    continue;
                }

                PeerSettingsRecord Record = Settings.GetOrCreatePeerRecord(Peer.Connection.Address.Address.ToString());

                if (Peer.Connection.IsConnected)
                {
                    long TotalIn = Peer.Connection.BandwidthStats.TotalIn;
                    long TotalOut = Peer.Connection.BandwidthStats.TotalOut;

                    Record.LastSeen = DateTime.Now;

                    Record.TotalIn += Record.TotalInTracker == -1 ? 0 : TotalIn - Record.TotalInTracker;
                    Record.TotalOut += Record.TotalOutTracker == -1 ? 0 : TotalOut - Record.TotalOutTracker;

                    Record.TotalInTracker = TotalIn;
                    Record.TotalOutTracker = TotalOut;

                    Record.AverageRateIn = Peer.Connection.BandwidthStats.RateIn; // (Record.AverageRateIn * 0.5f) + (Peer.Connection.BandwidthStats.RateIn * 0.5f);
                    Record.AverageRateOut = Peer.Connection.BandwidthStats.RateOut; // (Record.AverageRateOut * 0.5f) + (Peer.Connection.BandwidthStats.RateOut * 0.5f);

                    Record.PeakRateIn = Math.Max(Record.PeakRateIn, Record.AverageRateIn);
                    Record.PeakRateOut = Math.Max(Record.PeakRateOut, Record.AverageRateOut);

                    Record.TargetInFlightData = Peer.GetMaxInFlightData(Core.Client.Client.TargetMillisecondsOfDataInFlight);
                    Record.CurrentInFlightData = Record.TargetInFlightData - Peer.GetAvailableInFlightData(Core.Client.Client.TargetMillisecondsOfDataInFlight);

                    Record.Ping = (long) Peer.Connection.Ping.Get();
                    Record.BestPing = (long) Peer.Connection.BestPing;
                }
            }
        }

        /// <summary>
        /// </summary>
        public static void SaveSettings(bool ForceNow = false)
        {
            if (ForceNow)
            {
                Settings.Save(SettingsPath);
            }
            else
            {
                ForceSaveSettingsPending = true;
            }
        }

        /// <summary>
        /// </summary>
        internal static void PumpLoop(PumpLoopEventHandler UpdateDelegate = null)
        {
            while (true)
            {
                if (UpdateDelegate != null)
                {
                    if (UpdateDelegate.Invoke())
                    {
                        break;
                    }
                }

                OnPoll();
                Application.DoEvents();
            }
        }

        /// <summary>
        /// </summary>
        private static void InitSettings()
        {
            string AppDir = AppDataDir;

            SettingsPath = Path.Combine(AppDir, "Config.json");
            SettingsBase.Load(SettingsPath, out Settings);

            if (Settings.StoragePath.Length == 0)
            {
                Settings.StoragePath = Path.Combine(AppDir, "Storage");
                if (!Directory.Exists(Settings.StoragePath))
                {
                    Directory.CreateDirectory(Settings.StoragePath);
                }
            }

            CurrentStoragePath = Settings.StoragePath;

            if (Settings.ActiveStatistics.Count > 0)
            {
                lock (Statistic.Instances)
                {
                    foreach (KeyValuePair<Type, Statistic> pair in Statistic.Instances)
                    {
                        pair.Value.DefaultShown = Settings.ActiveStatistics.Contains(pair.Value.Name);
                    }
                }
            }

            Settings.Save(SettingsPath);

            ApplySettings();
        }

        /// <summary>
        /// </summary>
        private static void PollIpcServer()
        {
            if (!RespondingToIpc)
            {
                string Command;
                string[] Args;
                if (IPCServer.Recieve(out Command, out Args))
                {
                    RespondingToIpc = true;

                    if (Command == "RunCommand")
                    {
                        Logger.Log(LogLevel.Warning, LogCategory.Main, "Recieved ipc command '{0} {1}'.", Command, string.Join(" ", Args));

                        Parser parser = new Parser(with => with.HelpWriter = new CommandIPCWriter(IPCServer));
                        ParserResult<object> parserResult = parser.ParseArguments<CommandLineAddOptions, CommandLineDeleteOptions, CommandLineListOptions, CommandLineConfigureOptions>(Args);
                        parserResult.WithParsed<CommandLineAddOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineDeleteOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineListOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineConfigureOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithNotParsed(errs => { });

                        IPCServer.EndResponse();
                    }
                    else
                    {
                        Logger.Log(LogLevel.Warning, LogCategory.Main, "Recieved unknown ipc command '{0}'.", Command);

                        IPCServer.Respond("Unknown Command");
                    }

                    RespondingToIpc = false;
                }
            }
        }
    }
}