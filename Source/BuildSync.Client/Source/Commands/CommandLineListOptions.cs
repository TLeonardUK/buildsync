/*
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
using BuildSync.Core;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;
using CommandLine;

namespace BuildSync.Client.Commands
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("list", HelpText = "Lists all builds that exists at a given path on the server.")]
    public class CommandLineListOptions
    {
        /// <summary>
        /// 
        /// </summary>
        [Value(0, MetaName = "VirtualPath", Required = false, HelpText = "Path, in the servers virtual file system, whose contents should be listed.")]
        public string VirtualPath { get; set; } = "";

        /// <summary>
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

            BuildsRecievedHandler BuildsRecievedHandler = (RootPath, Builds) =>
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

            Program.PumpLoop(
                () =>
                {
                    if (!Program.NetClient.IsConnected)
                    {
                        IpcClient.Respond("FAILED: Lost connection to server.");
                        return true;
                    }

                    return GotResults;
                }
            );

            Program.NetClient.OnBuildsRecieved -= BuildsRecievedHandler;
        }
    }
}