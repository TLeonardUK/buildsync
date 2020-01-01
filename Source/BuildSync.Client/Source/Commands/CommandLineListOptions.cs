using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;
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
                IpcClient.Respond("FAILED: No connection to server.", true);
                return;
            }

            Program.NetClient.OnLostConnectionToServer += () =>
            {
                IpcClient.Respond("FAILED: Lost connection to server.", true);

            };

            // TODO: Handle unbinding this etc.

            Program.NetClient.OnBuildsRecieved += (string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds) =>
            {
                if (RootPath == VirtualPath)
                {
                    string Format = "{0,-30} | {1,-40} | {2,-25}";

                    IpcClient.Respond(string.Format(Format, "Path", "Id", "Create Time"), false);                    
                    foreach (NetMessage_GetBuildsResponse.BuildInfo Info in Builds)
                    {
                        if (Info.Guid == Guid.Empty)
                        {
                            IpcClient.Respond(string.Format(Format, VirtualFileSystem.GetNodeName(Info.VirtualPath), "", ""), false);
                        }
                        else
                        {
                            IpcClient.Respond(string.Format(Format, VirtualFileSystem.GetNodeName(Info.VirtualPath), Info.Guid, Info.CreateTime), false);
                        }
                    }
                    IpcClient.Respond("", true);
                }
            };

            Program.PumpLoop(() => { });
        }
    }
}
