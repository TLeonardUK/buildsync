using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BuildSync.Core;
using BuildSync.Core.Manifests;

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
        private static bool IsClosing = false;

        /// <summary>
        /// 
        /// </summary>
        public static Settings Settings;

        /// <summary>
        /// 
        /// </summary>
        private static string SettingsPath;

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
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {

#if true//DEBUG
            SetConsoleCtrlHandler((CtrlTypes CtrlType) =>
            {
                if (CtrlType == CtrlTypes.CTRL_C_EVENT ||
                    CtrlType == CtrlTypes.CTRL_BREAK_EVENT ||
                    CtrlType == CtrlTypes.CTRL_CLOSE_EVENT ||
                    CtrlType == CtrlTypes.CTRL_LOGOFF_EVENT ||
                    CtrlType == CtrlTypes.CTRL_SHUTDOWN_EVENT)
                {
                    Console.WriteLine("Recieved close event from console.");
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
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InitSettings()
        {
            SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BuildSync\Server\Config.xml";
            Settings.Load<Settings>(SettingsPath, out Settings);

            if (Settings.StoragePath.Length == 0)
            {
                Settings.StoragePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BuildSync\Server\Storage\";
                if (!Directory.Exists(Settings.StoragePath))
                {
                    Directory.CreateDirectory(Settings.StoragePath);
                }
            }

            Settings.Save(SettingsPath);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OnStart()
        {
            InitSettings();

            BuildRegistry = new BuildManifestRegistry();
            BuildRegistry.Open(Path.Combine(Settings.StoragePath, "Manifests"));

            NetServer = new BuildSyncServer();
            NetServer.Start(Settings.ServerPort, BuildRegistry);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OnPoll()
        {
            NetServer.Poll();
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
    }
}
