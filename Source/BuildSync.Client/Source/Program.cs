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
using BuildSync.Core.Utils;
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

                    PollTimer = new System.Timers.Timer(1);
                    PollTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
                    {
                        if (Monitor.TryEnter(PollTimer))
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

                            Monitor.Exit(PollTimer);
                        }
                    };
                    PollTimer.Start();

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

            // Bandwidth settings.
            NetConnection.GlobalBandwidthThrottleIn.MaxRate = Settings.BandwidthMaxDown;
            NetConnection.GlobalBandwidthThrottleOut.MaxRate = Settings.BandwidthMaxUp;

            // General settings.
            ProcessUtils.SetLaunchOnStartup("Build Sync - Client", Settings.RunOnStartup);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OnStart()
        {
            InitSettings();

            IOQueue = new AsyncIOQueue();

            ManifestDownloadManager = new ManifestDownloadManager();
            DownloadManager = new DownloadManager();

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
                    NewChildren.Add(new VirtualFileSystemInsertChild { VirtualPath = Build.VirtualPath, Metadata = Build.Guid });
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
                BuildFileSystem
            );
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
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OnStop()
        {
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
