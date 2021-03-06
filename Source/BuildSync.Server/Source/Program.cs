﻿/*
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
using System.Runtime.InteropServices;
using System.Threading;
using BuildSync.Core;
using BuildSync.Core.Server;
using BuildSync.Core.Client;
using BuildSync.Core.Licensing;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using BuildSync.Core.Tags;
using BuildSync.Core.Routes;
using BuildSync.Server.Commands;
using CommandLine;

namespace BuildSync.Server
{
    /// <summary>
    ///     Application entry class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// </summary>
        private static TagRegistry TagRegistry;

        /// <summary>
        /// </summary>
        private static RouteRegistry RouteRegistry;

        /// <summary>
        /// </summary>
        public static BuildManifestRegistry BuildRegistry;

        /// <summary>
        /// 
        /// </summary>
        public static RemoteActionServer RemoteActionServer;

        /// <summary>
        /// </summary>
        public static bool InCommandLineMode = false;

        /// <summary>
        /// </summary>
        public static CommandIPC IPCServer = new CommandIPC("buildsync-server", false);

        /// <summary>
        /// </summary>
        public static LicenseManager LicenseMgr;

        /// <summary>
        /// </summary>
        public static Core.Server.Server NetServer;

        /// <summary>
        /// </summary>
        public static bool RespondingToIpc;

        /// <summary>
        /// </summary>
        public static Settings Settings;

        /// <summary>
        /// </summary>
        public static UserManager UserManager;

        /// <summary>
        /// </summary>
        private static bool IsClosing;

        /// <summary>
        /// </summary>
        private static ulong LastSaveSettingsTime;

        /// <summary>
        /// </summary>
        private static ulong LastStatusPrintTime;

        /// <summary>
        /// </summary>
        private const ulong MinTimeBetweenDirtyManifestSaves = 60 * 1000;

        /// <summary>
        /// </summary>
        private static string SettingsPath;

        /// <summary>
        /// </summary>
        private const ulong StatusPrintInterval = 60 * 1000;

        /// <summary>
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
        /// </summary>
        public static void ApplySettings()
        {
            ProcessUtils.SetLaunchOnStartup("Build Sync - Server", Settings.RunOnStartup);

            // General settings.
            Logger.MaximumVerbosity = Program.Settings.LoggingLevel;
        }

        /// <summary>
        ///     The main entry point for the application.
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
                    SetConsoleCtrlHandler(
                        CtrlType =>
                        {
                            if (CtrlType == CtrlTypes.CTRL_C_EVENT ||
                                CtrlType == CtrlTypes.CTRL_BREAK_EVENT ||
                                CtrlType == CtrlTypes.CTRL_CLOSE_EVENT ||
                                CtrlType == CtrlTypes.CTRL_LOGOFF_EVENT ||
                                CtrlType == CtrlTypes.CTRL_SHUTDOWN_EVENT)
                            {
                                Logger.Log(LogLevel.Warning, LogCategory.Main, "Recieved close event from console.");
                                IsClosing = true;
                                return true;
                            }

                            return false;
                        }, true
                    );

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
        /// </summary>
        public static void OnPoll()
        {
            BuildRegistry.Poll();
            NetServer.Poll();
            LicenseMgr.Poll();
            RemoteActionServer.Poll();

            NetServer.MaxConnectedClients = LicenseMgr.ActiveLicense.MaxSeats;
            Settings.MaxBandwidth = NetServer.BandwidthLimit;

            BuildRegistry.PruneUnseenManifests(Settings.MaximumManifestUnseenDays);
            if (BuildRegistry.ManifestLastSeenTimesDirty && TimeUtils.Ticks - LastSaveSettingsTime > MinTimeBetweenDirtyManifestSaves)
            {
                BuildRegistry.ManifestLastSeenTimesDirty = false;

                Settings.ManifestLastSeenTimes = new Dictionary<string, DateTime>(BuildRegistry.ManifestLastSeenTimes);
                SaveSettings();
            }

            PollIpcServer();

            // Pring status every so often.
            ulong Elapsed = TimeUtils.Ticks - LastStatusPrintTime;
            if (Elapsed > StatusPrintInterval)
            {
                PrintStatus();
                LastStatusPrintTime = TimeUtils.Ticks;
            }
        }

        /// <summary>
        /// </summary>
        public static void OnStart()
        {
            InitSettings();

            TagRegistry = new TagRegistry(Settings.Tags);
            TagRegistry.TagsUpdated += () =>
            {
                Settings.Tags = TagRegistry.Tags;
                SaveSettings();
            };

            RouteRegistry = new RouteRegistry(Settings.Routes, TagRegistry);
            RouteRegistry.RoutesUpdated += () =>
            {
                Settings.Routes = RouteRegistry.Routes;
                SaveSettings();
            };

            BuildRegistry = new BuildManifestRegistry(TagRegistry);
            BuildRegistry.Open(Path.Combine(AppDataDir, "Manifests"), Settings.MaximumManifests);
            BuildRegistry.ManifestLastSeenTimes = new Dictionary<string, DateTime>(Settings.ManifestLastSeenTimes);

            LicenseMgr = new LicenseManager();
            LicenseMgr.Start(Path.Combine(AppDataDir, "License.dat"));

            UserManager = new UserManager(Settings.Users, Settings.UserGroups);
            UserManager.UsersUpdated += () =>
            {
                Settings.Users = UserManager.Users;
                SaveSettings();
            };
            UserManager.UserGroupsUpdated += () =>
            {
                Settings.UserGroups = UserManager.UserGroups;
                SaveSettings();
            };
            UserManager.PermissionsUpdated += user =>
            {
                Settings.Users = UserManager.Users;
                SaveSettings();
            };

            NetServer = new Core.Server.Server();
            RemoteActionServer = new RemoteActionServer(NetServer);

            NetServer.Start(Settings.ServerPort, BuildRegistry, UserManager, LicenseMgr, TagRegistry, RouteRegistry, RemoteActionServer);
            NetServer.BandwidthLimit = Settings.MaxBandwidth;
        }

        /// <summary>
        /// </summary>
        public static void OnStop()
        {
            SaveSettings();

            if (NetServer != null)
            {
                NetServer.Disconnect();
                NetServer = null;
            }
        }

        /// <summary>
        /// </summary>
        public static void SaveSettings()
        {
            Logger.Log(LogLevel.Verbose, LogCategory.Main, "Saving settings.");

            Settings.Save(SettingsPath);

            LastSaveSettingsTime = TimeUtils.Ticks;
        }

        /// <summary>
        /// </summary>
        private static void InitSettings()
        {
            string AppDir = AppDataDir;

            SettingsPath = Path.Combine(AppDir, "Config.json");
            SettingsBase.Load(SettingsPath, out Settings);

            if (Settings.LastUpgradeVersion < AppVersion.VersionNumber)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "InitSettings: Upgrading settings.");
                UpgradeSettings();
            }

            if (Settings.LastUpgradeVersion < 100000613)
            {
                // Copy over manifests folder from old storage path.
                string SrcPath = Path.Combine(Settings.StoragePath, "Manifests");
                string DstPath = Path.Combine(AppDataDir, "Manifests");
                if (Directory.Exists(SrcPath))
                {
                    if (!Directory.Exists(DstPath))
                    {
                        Directory.CreateDirectory(DstPath);
                    }

                    foreach (string SrcFile in Directory.GetFiles(SrcPath))
                    {
                        string DstFile = Path.Combine(DstPath, SrcFile.Substring(SrcPath.Length + 1));
                        if (!File.Exists(DstFile))
                        {
                            File.Copy(SrcFile, DstFile);
                        }
                    }
                }
            }

            Settings.Save(SettingsPath);

            ApplySettings();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void UpgradeSettings()
        {
            if (Settings.LastUpgradeVersion == 0)
            {
                // Server port was modified on this version due to protocal differences. 
                if (Settings.ServerPort == 12341)
                {
                    Settings.ServerPort = 12340;
                }
            }

            Settings.LastUpgradeVersion = AppVersion.VersionNumber;
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
                        ParserResult<object> parserResult = parser.ParseArguments<CommandLineConfigureOptions, CommandLineAddUserOptions, CommandLineRemoveUserOptions, CommandLineListUsersOptions, CommandLineGrantPermissionOptions, CommandLineRevokePermissionOptions, CommandLineApplyLicenseOptions>(Args);
                        parserResult.WithParsed<CommandLineConfigureOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineAddUserOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineRemoveUserOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineListUsersOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineAddUserGroupOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineRemoveUserGroupOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineAddUserToGroupOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineRemoveUserFromGroupOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineListUserGroupsOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineGrantPermissionOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineRevokePermissionOptions>(opts => { opts.Run(IPCServer); });
                        parserResult.WithParsed<CommandLineApplyLicenseOptions>(opts => { opts.Run(IPCServer); });
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

        /// <summary>
        /// </summary>
        private static void PrintStatus()
        {
            if (AppVersion.NonLicensed)
            {
                Logger.Log(
                    LogLevel.Info, LogCategory.Main, "Status: Clients={0}, Download={1}, Upload={2}",
                    NetServer.ClientCount,
                    StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateIn),
                    StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateOut)
                );
            }
            else
            {
                Logger.Log(
                    LogLevel.Info, LogCategory.Main, "Status: Clients={0}/{1}, Download={2}, Upload={3}",
                    NetServer.ClientCount,
                    NetServer.MaxConnectedClients,
                    StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateIn),
                    StringUtils.FormatAsTransferRate(NetConnection.GlobalBandwidthStats.RateOut)
                );
            }
        }

        #region Unmanaged Functions

        /// <summary>
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
        /// </summary>
        /// <param name="CtrlType"></param>
        /// <returns></returns>
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        /// <summary>
        /// </summary>
        /// <param name="Handler"></param>
        /// <param name="Add"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        #endregion
    }
}