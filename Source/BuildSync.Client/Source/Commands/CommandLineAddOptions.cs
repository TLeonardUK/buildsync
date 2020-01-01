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
                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Path does not exists: {0}", LocalPath);
                return;
            }

            PublishBuildTask Publisher = new PublishBuildTask();
            BuildPublishingState OldPublisherState = BuildPublishingState.Unknown;
            int OldPublisherProgress = 0;

            Program.NetClient.OnConnectedToServer += () =>
            {
                Publisher.Start(VirtualPath, LocalPath);
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

            Program.PumpLoop(() =>
            {
                if (Publisher != null && (Publisher.State != OldPublisherState || (int)Publisher.Progress != OldPublisherProgress))
                {
                    OldPublisherState = Publisher.State;
                    OldPublisherProgress = (int)Publisher.Progress;

                    switch (Publisher.State)
                    {
                        case BuildPublishingState.CopyingFiles:
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "Copying files to storage: {0}%", OldPublisherProgress);
                                break;
                            }
                        case BuildPublishingState.ScanningFiles:
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "Scanning local files: {0}%", OldPublisherProgress);
                                break;
                            }
                        case BuildPublishingState.UploadingManifest:
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "Uploading manfiest to server.");
                                break;
                            }
                        case BuildPublishingState.FailedVirtualPathAlreadyExists:
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Build already exists at virtual path '{0}'.", VirtualPath);
                                Program.RequestExit();
                                break;
                            }
                        case BuildPublishingState.FailedGuidAlreadyExists:
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Manifest with same GUID already exists.");
                                Program.RequestExit();
                                break;
                            }
                        case BuildPublishingState.Success:
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "SUCCESS: Build added successfully.");
                                Program.RequestExit();
                                break;
                            }
                        case BuildPublishingState.Failed:
                        default:
                            {
                                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Undefined reason.");
                                Program.RequestExit();
                                break;
                            }
                    }
                }
            });
        }
    }
}
