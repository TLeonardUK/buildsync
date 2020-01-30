using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using BuildSync.Client.Forms;
using BuildSync.Core;
using System.Timers;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Scm;
using BuildSync.Core.Scm.Perforce;
using BuildSync.Core.Scm.Git;
using BuildSync.Client.Commands;
using CommandLine;

namespace BuildSync.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if pump loop should terminate.</returns>
    public delegate bool PumpLoopEventHandler();

    /// <summary>
    /// 
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        private static System.Timers.Timer PollTimer;

        /// <summary>
        /// 
        /// </summary>
        public static MainForm AppForm;

        /// <summary>
        /// 
        /// </summary>
        public static BuildSyncClient NetClient;

        /// <summary>
        /// 
        /// </summary>
        public static BuildManifestRegistry BuildRegistry;

        /// <summary>
        /// 
        /// </summary>
        public static DownloadManager DownloadManager;

        /// <summary>
        /// 
        /// </summary>
        public static ManifestDownloadManager ManifestDownloadManager;

        /// <summary>
        /// 
        /// </summary>
        public static VirtualFileSystem BuildFileSystem;

        /// <summary>
        /// 
        /// </summary>
        public static ScmManager ScmManager;

        /// <summary>
        /// 
        /// </summary>
        public static AsyncIOQueue IOQueue;

        /// <summary>
        /// 
        /// </summary>
        public static Settings Settings;

        /// <summary>
        /// 
        /// </summary>
        private static string SettingsPath;

        /// <summary>
        /// 
        /// </summary>
        private static ulong LastSettingsSaveTime = TimeUtils.Ticks;

        /// <summary>
        /// 
        /// </summary>
        private const int MinimumTimeBetweenSettingsSaves = 60 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private static bool ForceSaveSettingsPending = false;

        /// <summary>
        /// 
        /// </summary>
        private static string CurrentStoragePath = "";

        /// <summary>
        /// 
        /// </summary>
        public static bool InCommandLineMode = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool RespondingToIpc = false;

        /// <summary>
        /// 
        /// </summary>
        public static CommandIPC IPCServer = new CommandIPC("buildsync-client", false);

        /// <summary>
        /// 
        /// </summary>
        private static DownloadState InternalUpdateDownload = null;

        /// <summary>
        /// 
        /// </summary>
        private static Guid InternalUpdateManifestId = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        public static bool AutoUpdateAvailable { get; private set; } = false;

        /// <summary>
        /// 
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
        /// The main entry point for the application.
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
        /// 
        /// </summary>
        private static void InitSettings()
        {
            string AppDir = AppDataDir;

            SettingsPath = Path.Combine(AppDir, "Config.json");
            Settings.Load<Settings>(SettingsPath, out Settings);

            if (Settings.StoragePath.Length == 0)
            {
                Settings.StoragePath = Path.Combine(AppDir, "Storage");
                if (!Directory.Exists(Settings.StoragePath))
                {
                    Directory.CreateDirectory(Settings.StoragePath);
                }
            }

            CurrentStoragePath = Settings.StoragePath;

            Settings.Save(SettingsPath);

            ApplySettings();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SaveSettings(bool ForceNow = false)
        {
            if (ForceNow)
            {
                Program.Settings.Save(SettingsPath);
            }
            else
            {
                ForceSaveSettingsPending = true;
            }
        }

        /// <summary>
        /// 
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

            // General settings.
            ProcessUtils.SetLaunchOnStartup("Build Sync - Client", Settings.RunOnStartup);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OnStart()
        {
            ScmManager = new ScmManager();
            IOQueue = new AsyncIOQueue();
            ManifestDownloadManager = new ManifestDownloadManager();
            DownloadManager = new DownloadManager();

            InitSettings();

            NetClient = new BuildSyncClient();

            BuildRegistry = new BuildManifestRegistry();
            BuildRegistry.Open(Path.Combine(Settings.StoragePath, "Manifests"), int.MaxValue);

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
            NetClient.OnConnectedToServer += () =>
            {
                BuildFileSystem.ForceRefresh();
            };
            NetClient.OnBuildsRecieved += (string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds) =>
            {
                List<VirtualFileSystemInsertChild> NewChildren = new List<VirtualFileSystemInsertChild>();
                foreach (NetMessage_GetBuildsResponse.BuildInfo Build in Builds)
                {
                    NewChildren.Add(new VirtualFileSystemInsertChild { 
                        VirtualPath = Build.VirtualPath,
                        CreateTime = Build.Guid == Guid.Empty ? DateTime.UtcNow : Build.CreateTime,
                        Metadata = Build.Guid,
                    });
                }
                BuildFileSystem.ReconcileChildren(RootPath, NewChildren);
            };
            BuildFileSystem.OnRequestChildren += (VirtualFileSystem FileSystem, string Path) =>
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
                InternalUpdateDownload = DownloadManager.AddDownload(UpdateDownloadName, "$Internal$/Updates", 2, BuildSelectionRule.Newest, BuildSelectionFilter.None, "", "", true, false, "");
            }

            // Make sure we have to get the latest manifest id before updating.
            InternalUpdateDownload.ActiveManifestId = Guid.Empty;
        }

        /// <summary>
        /// 
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
                    Result = (Now >= Start && Now <= End);
                }
                // Start and stop times are in different days
                else
                {
                    Result = (Now >= Start || Now <= End);
                }
            }
            else
            {
                Result = true;
            }

            return Result;
        }

        /// <summary>
        /// 
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

                    Program.Settings.DownloadStates = DownloadManager.States;
                    Program.Settings.ManifestDownloadStates = ManifestDownloadManager.States;
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
        /// 
        /// </summary>
        public static void RecordPeerStates()
        {
            BuildSyncClient.Peer[] AllPeers = Program.NetClient.AllPeers;

            foreach (PeerSettingsRecord Record in Settings.PeerRecords)
            {
                Record.AverageRateIn = 0;
                Record.AverageRateOut = 0;

                Record.TargetInFlightData = 0;
                Record.CurrentInFlightData = 0;
            }

            foreach (BuildSyncClient.Peer Peer in AllPeers)
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

                    Record.TotalIn += (Record.TotalInTracker == -1) ? 0 : TotalIn - Record.TotalInTracker;
                    Record.TotalOut += (Record.TotalOutTracker == -1) ? 0 : TotalOut - Record.TotalOutTracker;

                    Record.TotalInTracker = TotalIn;
                    Record.TotalOutTracker = TotalOut;

                    Record.AverageRateIn = Peer.Connection.BandwidthStats.RateIn;// (Record.AverageRateIn * 0.5f) + (Peer.Connection.BandwidthStats.RateIn * 0.5f);
                    Record.AverageRateOut = Peer.Connection.BandwidthStats.RateOut;// (Record.AverageRateOut * 0.5f) + (Peer.Connection.BandwidthStats.RateOut * 0.5f);

                    Record.PeakRateIn = Math.Max(Record.PeakRateIn, Record.AverageRateIn);
                    Record.PeakRateOut = Math.Max(Record.PeakRateOut, Record.AverageRateOut);

                    Record.TargetInFlightData = Peer.GetMaxInFlightData(BuildSyncClient.TargetMillisecondsOfDataInFlight);
                    Record.CurrentInFlightData = Record.TargetInFlightData - Peer.GetAvailableInFlightData(BuildSyncClient.TargetMillisecondsOfDataInFlight);

                    Record.Ping = (long)Peer.Connection.Ping.Get();
                    Record.BestPing = (long)Peer.Connection.BestPing;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PollAutoUpdate()
        {
#if SHIPPING
            if (InternalUpdateDownload != null)
            {
                ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(InternalUpdateDownload.ActiveManifestId);
                if (Downloader != null && Downloader.State == ManifestDownloadProgressState.Complete && Downloader.Manifest != null)
                {
                    if (Settings.LastAutoUpdateManifest != Downloader.ManifestId)
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
        /// 
        /// </summary>
        public static void InstallAutoUpdate()
        {
            ManifestDownloadState AutoUpdateDownload = null;
            if (InternalUpdateDownload != null)
            {
                AutoUpdateDownload = Program.ManifestDownloadManager.GetDownload(InternalUpdateDownload.ActiveManifestId);
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
                return;
            }
            else
            {
                // Save the last manifest we installed.
                Settings.LastAutoUpdateManifest = AutoUpdateDownload.ManifestId;
                SaveSettings(true);

                // Boot up the installer.
                try
                {
                    //Process.Start(InstallerPath, "/passive /norestart REINSTALL=ALL REINSTALLMODE=A MSIRMSHUTDOWN=1 MSIDISABLERMRESTART=0 ADDLOCAL=All");
                    Process.Start(InstallerPath);
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to install update '{0}' due to error: {1}", InstallerPath, Ex.Message);

                    Settings.LastAutoUpdateManifest = Guid.Empty;
                    SaveSettings(true);
                }

                Application.Exit();
            }
        }

        /// <summary>
        /// 
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
        /// 
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

                Program.OnPoll();
                Application.DoEvents();
            }
        }

        /// <summary>
        /// 
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

                        var parser = new Parser(with => with.HelpWriter = new CommandIPCWriter(IPCServer));
                        var parserResult = parser.ParseArguments<CommandLineAddOptions, CommandLineDeleteOptions, CommandLineListOptions, CommandLineConfigureOptions>(Args);
                        parserResult.WithParsed<CommandLineAddOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineDeleteOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineListOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineConfigureOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithNotParsed((errs) => { });

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
