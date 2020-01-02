using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using BuildSync.Client.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Users;

namespace BuildSync.Client.Commands
{
    [Verb("add", HelpText = "Adds a build to the given path on the server.")]
    public class CommandLineAddOptions
    {
        [Value(0, MetaName = "VirtualPath", Required = true, HelpText = "Path, in the servers virtual file system, that this build should be added at, eg. 'MyProject/Nightly/cs12345'.")]        
        public string VirtualPath { get; set; }

        [Value(1, MetaName = "LocalPath", Required = true, HelpText = "Path to a folder, on the local machine, that the build files exist in.")]
        public string LocalPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

            if (!Program.NetClient.HasPermission(UserPermission.ManageBuilds))
            {
                IpcClient.Respond("FAILED: Permission denied to manage builds.");
                return;
            }

            PublishBuildTask Publisher = new PublishBuildTask();
            BuildPublishingState OldPublisherState = BuildPublishingState.Unknown;
            int OldPublisherProgress = 0;

            Publisher.Start(VirtualPath, LocalPath);

            Program.PumpLoop(() =>
            {
                if (!Program.NetClient.IsConnected)
                {
                    IpcClient.Respond("FAILED: Lost connection to server.");
                    return true;
                }

                if (Publisher != null && (Publisher.State != OldPublisherState || (int)Publisher.Progress != OldPublisherProgress))
                {
                    OldPublisherState = Publisher.State;
                    OldPublisherProgress = (int)Publisher.Progress;

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
                                IpcClient.Respond(string.Format("Uploading manfiest to server."));
                                break;
                            }
                        case BuildPublishingState.FailedVirtualPathAlreadyExists:
                            {
                                IpcClient.Respond(string.Format("FAILED: Build already exists at virtual path '{0}'.", VirtualPath));
                                return true;
                            }
                        case BuildPublishingState.FailedGuidAlreadyExists:
                            {
                                IpcClient.Respond(string.Format("FAILED: Manifest with same GUID already exists."));
                                return true;
                            }
                        case BuildPublishingState.Success:
                            {
                                Publisher.Commit();
                                Publisher = null;
                                IpcClient.Respond(string.Format("SUCCESS: Build added successfully."));
                                return true;
                            }
                        case BuildPublishingState.Failed:
                        default:
                            {
                                IpcClient.Respond(string.Format("FAILED: Undefined reason."));
                                return true;
                            }
                    }
                }

                return false;
            });
        }
    }
}
