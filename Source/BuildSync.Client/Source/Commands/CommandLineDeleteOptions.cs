using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking.Messages;

namespace BuildSync.Client.Commands
{
    [Verb("delete", HelpText = "Deletes a build at the given path from the server.")]
    public class CommandLineDeleteOptions
    {
        [Value(0, MetaName = "VirtualPath", Required = true, HelpText = "Path, in the servers virtual file system, of the build that should be deleted.")]
        public string VirtualPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            VirtualPath = VirtualFileSystem.Normalize(VirtualPath);

            string ParentPath = VirtualFileSystem.GetParentPath(VirtualPath);
            string NodeName = VirtualFileSystem.GetNodeName(VirtualPath);

            bool DeleteInProgress = false;

            Program.NetClient.OnConnectedToServer += () =>
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, "Looking for build at path: {0}", VirtualPath);
                if (!Program.NetClient.RequestBuilds(ParentPath))
                {
                    Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Unknown error.");
                    Program.RequestExit();
                }
            };
            Program.NetClient.OnLostConnectionToServer += () =>
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Lost connection to server.");
                Program.RequestExit();
            };
            Program.NetClient.OnFailedToConnectToServer += () =>
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Failed to connect to server ({0}:{1}).", Program.Settings.ServerHostname, Program.Settings.ServerPort);
                Program.RequestExit();
            };
            Program.NetClient.OnBuildsRecieved += (string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds) =>
            {
                if (RootPath == ParentPath && !DeleteInProgress)
                {
                    bool Found = false;

                    foreach (NetMessage_GetBuildsResponse.BuildInfo Info in Builds)
                    {
                        if (VirtualFileSystem.GetNodeName(Info.VirtualPath).ToLower() == NodeName.ToLower())
                        {
                            if (Info.Guid == Guid.Empty)
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Virtual path is not a build.");
                                Program.RequestExit();
                            }
                            else
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "Deleting manifest with guid: {0}", Info.Guid);
                                Program.NetClient.DeleteManifest(Info.Guid);
                            }

                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                    {
                        Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Build does not exist at path.");
                        Program.RequestExit();
                    }

                    DeleteInProgress = true;
                }
            };
            Program.NetClient.OnManifestDeleteResultRecieved += (Guid Id) =>
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, "SUCCESS: Deleted manifest.");
                Program.RequestExit();
            };

            Program.PumpLoop(() => { });
        }
    }
}
