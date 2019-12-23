using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using BuildSync.Client.Forms;
using BuildSync.Core;
using System.Timers;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;

namespace BuildSync.Client
{
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
        private static int LastSettingsSaveTime = Environment.TickCount;

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
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            OnStart();

            PollTimer = new System.Timers.Timer(1);
            PollTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                lock (PollTimer)
                {
                    // Make sure it invokes on main thread, maybe spend some time
                    // make all the ui->program interaction thread safe?
                    if (AppForm != null && AppForm.IsHandleCreated)
                    {
                        AppForm.Invoke((MethodInvoker)(() =>
                        {
                            OnPoll();
                        }));
                    }
                }
            };
            PollTimer.Start();

            AppForm = new MainForm();
            Application.Run(AppForm);

            OnStop();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InitSettings()
        {
            string Namespace = "Client";

#if true//DEBUG
            int ProcessCount = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length;
            if (ProcessCount > 1)
            {
                Namespace = "Client_" + ProcessCount;
            }
#endif

            SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BuildSync\"+ Namespace + @"\Config.xml";
            Settings.Load<Settings>(SettingsPath, out Settings);

            if (Settings.StoragePath.Length == 0)
            {
                Settings.StoragePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BuildSync\" + Namespace + @"\Storage\";
                if (!Directory.Exists(Settings.StoragePath))
                {
                    Directory.CreateDirectory(Settings.StoragePath);
                }
            }

            CurrentStoragePath = Settings.StoragePath;

            Settings.Save(SettingsPath);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SaveSettings()
        {
            ForceSaveSettingsPending = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ApplySettings()
        {
            // Server settings.
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

            // Bandwidth settings.
            // TODO
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
            BuildRegistry.Open(Path.Combine(Settings.StoragePath, "Manifests"));

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
                NetClient.RequestBuilds(Path);
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
        public static void OnPoll()
        {
            NetClient.Poll();

            DownloadManager.Poll(NetClient.IsConnected);
            ManifestDownloadManager.Poll();

            // Update save data if download states have changed recently.
            if (Environment.TickCount - LastSettingsSaveTime > MinimumTimeBetweenSettingsSaves || ForceSaveSettingsPending)
            {
                if (ManifestDownloadManager.AreStatesDirty || DownloadManager.AreStatesDirty || ForceSaveSettingsPending)
                {
                    Console.WriteLine("Saving settings.");

                    Program.Settings.DownloadStates = DownloadManager.States;
                    Program.Settings.ManifestDownloadStates = ManifestDownloadManager.States;
                    Program.Settings.Save(SettingsPath);

                    ManifestDownloadManager.AreStatesDirty = false;
                    DownloadManager.AreStatesDirty = false;

                    ForceSaveSettingsPending = false;
                    LastSettingsSaveTime = Environment.TickCount;
                }
            }
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
        }
    }
}
