using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using BuildSync.Core;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking.Messages;
using CommandLine;

namespace BuildSync.Client.Commands
{
    [Verb("list", HelpText = "Lists all builds that exists at a given path on the server.")]
    public class CommandLineListOptions
    {
        [Value(0, MetaName = "VirtualPath", Required = false, HelpText = "Path, in the servers virtual file system, whose contents should be listed.")]
        public string VirtualPath { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            VirtualPath = VirtualFileSystem.Normalize(VirtualPath);

            if (!Program.NetClient.IsConnected)
            {
                IpcClient.Respond("FAILED: No connection to server.");
                return;
            }

            bool GotResults = false;

            BuildsRecievedHandler BuildsRecievedHandler = (string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds) =>
            {
                if (GotResults)
                {
                    return;
                }

                if (RootPath == VirtualPath)
                {
                    string Format = "{0,-30} | {1,-40} | {2,-25}";

                    IpcClient.Respond(string.Format(Format, "Path", "Id", "Create Time"));
                    foreach (NetMessage_GetBuildsResponse.BuildInfo Info in Builds)
                    {
                        if (Info.Guid == Guid.Empty)
                        {
                            IpcClient.Respond(string.Format(Format, VirtualFileSystem.GetNodeName(Info.VirtualPath), "", ""));
                        }
                        else
                        {
                            IpcClient.Respond(string.Format(Format, VirtualFileSystem.GetNodeName(Info.VirtualPath), Info.Guid, Info.CreateTime));
                        }
                    }
                    IpcClient.Respond("");

                    GotResults = true;
                }
            };

            Program.NetClient.OnBuildsRecieved += BuildsRecievedHandler;

            if (!Program.NetClient.RequestBuilds(VirtualPath))
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

                return GotResults;

            });

            Program.NetClient.OnBuildsRecieved -= BuildsRecievedHandler;
        }
    }
}
