using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using BuildSync.Core;
using BuildSync.Core.Utils;
using BuildSync.Core.Users;
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
            bool IsComplete = false;

            if (!Program.NetClient.IsConnected)
            {
                IpcClient.Respond("FAILED: No connection to server.");
                return;
            }

            if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageBuilds, ""))
            {
                IpcClient.Respond("FAILED: Permission denied to manage builds.");
                return;
            }

            BuildsRecievedHandler BuildsRecievedHandler = (string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds) =>
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
                                IpcClient.Respond("FAILED: Virtual path is not a build.");
                                IsComplete = true;
                            }
                            else
                            {
                                IpcClient.Respond(string.Format("Deleting manifest with guid: {0}", Info.Guid));
                                Program.NetClient.DeleteManifest(Info.Guid);
                            }

                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                    {
                        IpcClient.Respond("FAILED: Virtual path is not a build.");
                        IsComplete = true;
                    }

                    DeleteInProgress = true;
                }
            };

            ManifestDeleteResultRecievedHandler ManifestRecieveHandler = (Guid Id) =>
            {
                IpcClient.Respond("SUCCESS: Deleted manifest.");
                IsComplete = true;
            };

            Program.NetClient.OnBuildsRecieved += BuildsRecievedHandler;
            Program.NetClient.OnManifestDeleteResultRecieved += ManifestRecieveHandler; 

            if (!Program.NetClient.RequestBuilds(ParentPath))
            {
                IpcClient.Respond("FAILED: Failed to request builds for unknown reason.");
                return;
            }

            Program.PumpLoop(() => {

                if (!Program.NetClient.IsConnected)
                {
                    IpcClient.Respond("FAILED: Lost connection to server.");
                    return true;
                }

                return IsComplete;

            });

            Program.NetClient.OnManifestDeleteResultRecieved -= ManifestRecieveHandler;
            Program.NetClient.OnBuildsRecieved -= BuildsRecievedHandler;
        }
    }
}
