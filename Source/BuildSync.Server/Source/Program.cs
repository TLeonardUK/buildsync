using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BuildSync.Core;
using BuildSync.Core.Manifests;
using BuildSync.Core.Utils;
using BuildSync.Core.Users;
using BuildSync.Server.Commands;
using CommandLine;
using CommandLine.Text;

namespace BuildSync.Server
{
    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static BuildSyncServer NetServer;

        /// <summary>
        /// 
        /// </summary>
        public static BuildManifestRegistry BuildRegistry;

        /// <summary>
        /// 
        /// </summary>
        public static UserManager UserManager;

        /// <summary>
        /// 
        /// </summary>
        private static bool IsClosing = false;

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
        public static bool InCommandLineMode = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool RespondingToIpc = false;

        /// <summary>
        /// 
        /// </summary>
        public static CommandIPC IPCServer = new CommandIPC("buildsync-server", false);

        #region Unmanaged Functions

        /// <summary>
        /// 
        /// </summary>
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CtrlType"></param>
        /// <returns></returns>
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Handler"></param>
        /// <param name="Add"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public static string AppDataDir
        {
            get
            {
                string Namespace = "Server";

#if !SHIPPING
                int ProcessCount = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length;
                if (ProcessCount > 1)
                {
                    Namespace = "Server." + (ProcessCount - 1);
                }
#endif

                return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BuildSync"), Namespace);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            Logger.SetupStandardLogging(Path.Combine(AppDataDir, "Logging"), InCommandLineMode);

#if SHIPPING
            try
            {
                using (new SingleGlobalInstance(100))
                {
#endif

#if true
                    SetConsoleCtrlHandler((CtrlTypes CtrlType) =>
                    {
                        if (CtrlType == CtrlTypes.CTRL_C_EVENT ||
                            CtrlType == CtrlTypes.CTRL_BREAK_EVENT ||
                            CtrlType == CtrlTypes.CTRL_CLOSE_EVENT ||
                            CtrlType == CtrlTypes.CTRL_LOGOFF_EVENT ||
                            CtrlType == CtrlTypes.CTRL_SHUTDOWN_EVENT)
                        {
                            Logger.Log(LogLevel.Warning, LogCategory.Main, "Recieved close event from console.");
                            return true;
                        }
                        return false;
                    }, true);

                    OnStart();

                    while (!IsClosing)
                    {
                        OnPoll();
                        Thread.Sleep(1);
                    }

                    OnStop();
#else
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        new MainService()
                    };
                    ServiceBase.Run(ServicesToRun);
#endif

#if SHIPPING
                }
            }
            catch (System.TimeoutException Ex)
            {
                Console.Write("Application is already running.");
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

            Settings.Save(SettingsPath);

            ApplySettings();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ApplySettings()
        {
            ProcessUtils.SetLaunchOnStartup("Build Sync - Server", Settings.RunOnStartup);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SaveSettings()
        {
            Settings.Save(SettingsPath);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OnStart()
        {
            InitSettings();

            BuildRegistry = new BuildManifestRegistry();
            BuildRegistry.Open(Path.Combine(Settings.StoragePath, "Manifests"), Settings.MaximumManifests);

            UserManager = new UserManager(Settings.Users);
            UserManager.UsersUpdated += () =>
            {
                Settings.Users = UserManager.Users;
                SaveSettings();
            };

            NetServer = new BuildSyncServer();
            NetServer.Start(Settings.ServerPort, BuildRegistry, UserManager);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OnPoll()
        {
            NetServer.Poll();

            PollIpcServer();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OnStop()
        {
            if (NetServer != null)
            {
                NetServer.Disconnect();
                NetServer = null;
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
                        var parserResult = parser.ParseArguments<CommandLineConfigureOptions, CommandLineAddUserOptions, CommandLineRemoveUserOptions, CommandLineListUsersOptions, CommandLineGrantPermissionOptions, CommandLineRevokePermissionOptions>(Args);
                        parserResult.WithParsed<CommandLineConfigureOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineAddUserOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineRemoveUserOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineListUsersOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineGrantPermissionOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineRevokePermissionOptions>(opts => { opts.Run(IPCServer); });
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
