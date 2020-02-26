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
using BuildSync.Core.Client;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using BuildSync.Core.Client;
using CommandLine;

namespace BuildSync.Client.Commands
{
    /// <summary>
    ///     CLI command for deleting a build that has previously been published to the server manifest registry.
    /// </summary>
    [Verb("delete", HelpText = "Deletes a build at the given path from the server.")]
    public class CommandLineDeleteOptions
    {
        /// <summary>
        ///     Path, in the servers virtual file system, of the build that should be deleted.
        /// </summary>
        [Value(0, MetaName = "VirtualPath", Required = true, HelpText = "Path, in the servers virtual file system, of the build that should be deleted.")]
        public string VirtualPath { get; set; }

        /// <summary>
        ///     Called when the CLI invokes this command.
        /// </summary>
        /// <param name="IpcClient">Interprocess communication pipe to the application that invoked this command.</param>
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

            if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageBuilds, "", false, true))
            {
                IpcClient.Respond("FAILED: Permission denied to manage builds.");
                return;
            }

            BuildsRecievedHandler BuildsRecievedHandler = (RootPath, Builds) =>
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

            ManifestDeleteResultRecievedHandler ManifestRecieveHandler = Id =>
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

            Program.PumpLoop(
                () =>
                {
                    if (!Program.NetClient.IsConnected)
                    {
                        IpcClient.Respond("FAILED: Lost connection to server.");
                        return true;
                    }

                    return IsComplete;
                }
            );

            Program.NetClient.OnManifestDeleteResultRecieved -= ManifestRecieveHandler;
            Program.NetClient.OnBuildsRecieved -= BuildsRecievedHandler;
        }
    }
}