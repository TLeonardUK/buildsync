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

using System.IO;
using BuildSync.Client.Tasks;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using CommandLine;

namespace BuildSync.Client.Commands
{
    /// <summary>
    ///     CLI command for publishing a build onto the server manifest registry.
    /// </summary>
    [Verb("add", HelpText = "Adds a build to the given path on the server.")]
    public class CommandLineAddOptions
    {
        /// <summary>
        ///     Path to a folder, on the local machine, that the build files exist in.
        /// </summary>
        [Value(1, MetaName = "LocalPath", Required = true, HelpText = "Path to a folder, on the local machine, that the build files exist in.")]
        public string LocalPath { get; set; }

        /// <summary>
        ///     Path, in the servers virtual file system, that this build should be added at, eg. 'MyProject/Nightly/cs12345'.
        /// </summary>
        [Value(0, MetaName = "VirtualPath", Required = true, HelpText = "Path, in the servers virtual file system, that this build should be added at, eg. 'MyProject/Nightly/cs12345'.")]
        public string VirtualPath { get; set; }

        /// <summary>
        ///     Called when the CLI invokes this command.
        /// </summary>
        /// <param name="IpcClient">Interprocess communication pipe to the application that invoked this command.</param>
        internal void Run(CommandIPC IpcClient)
        {
            VirtualPath = VirtualFileSystem.Normalize(VirtualPath);

            if (!Directory.Exists(LocalPath))
            {
                IpcClient.Respond("FAILED: Path does not exists: {0}");
                return;
            }

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

            PublishBuildTask Publisher = new PublishBuildTask();
            BuildPublishingState OldPublisherState = BuildPublishingState.Unknown;
            int OldPublisherProgress = 0;

            Publisher.Start(VirtualPath, LocalPath);

            Program.PumpLoop(
                () =>
                {
                    if (!Program.NetClient.IsConnected)
                    {
                        IpcClient.Respond("FAILED: Lost connection to server.");
                        return true;
                    }

                    if (Publisher != null && (Publisher.State != OldPublisherState || (int) Publisher.Progress != OldPublisherProgress))
                    {
                        OldPublisherState = Publisher.State;
                        OldPublisherProgress = (int) Publisher.Progress;

                        switch (Publisher.State)
                        {
                            case BuildPublishingState.CopyingFiles:
                            {
                                IpcClient.Respond(string.Format("Copying files to storage: {0}%", OldPublisherProgress));
                                break;
                            }
                            case BuildPublishingState.ScanningFiles:
                            {
                                IpcClient.Respond(string.Format("Scanning local files: {0}%", OldPublisherProgress));
                                break;
                            }
                            case BuildPublishingState.UploadingManifest:
                            {
                                IpcClient.Respond("Uploading manfiest to server.");
                                break;
                            }
                            case BuildPublishingState.FailedVirtualPathAlreadyExists:
                            {
                                IpcClient.Respond(string.Format("FAILED: Build already exists at virtual path '{0}'.", VirtualPath));
                                return true;
                            }
                            case BuildPublishingState.PermissionDenied:
                            {
                                IpcClient.Respond(string.Format("FAILED: Permission denied to virtual path '{0}'.", VirtualPath));
                                return true;
                            }
                            case BuildPublishingState.FailedGuidAlreadyExists:
                            {
                                IpcClient.Respond("FAILED: Manifest with same GUID already exists.");
                                return true;
                            }
                            case BuildPublishingState.Success:
                            {
                                Publisher.Commit();
                                Publisher = null;
                                IpcClient.Respond("SUCCESS: Build added successfully.");
                                return true;
                            }
                            case BuildPublishingState.Failed:
                            default:
                            {
                                IpcClient.Respond("FAILED: Undefined reason.");
                                return true;
                            }
                        }
                    }

                    return false;
                }
            );
        }
    }
}