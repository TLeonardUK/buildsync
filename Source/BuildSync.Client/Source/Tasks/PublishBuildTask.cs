using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Manifests;
using BuildSync.Core;
using BuildSync.Core.Utils;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Core.Networking.Messages;

namespace BuildSync.Client.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public enum BuildPublishingState
    { 
        ScanningFiles,
        CopyingFiles,
        UploadingManifest,
        FailedVirtualPathAlreadyExists,
        FailedGuidAlreadyExists,
        Failed,
        Success,
        Unknown
    }

    /// <summary>
    /// 
    /// </summary>
    public class PublishBuildTask
    {
        private BuildManifest Manifest = null;
        private string LocalPath = "";

        /// <summary>
        /// 
        /// </summary>
        public float Progress
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CurrentFile
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public BuildPublishingState State
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="VirtualPath"></param>
        /// <param name="LocalPath"></param>
        public void Start(string VirtualPath, string InLocalPath)
        {
            LocalPath = InLocalPath;
            State = BuildPublishingState.ScanningFiles;

            Task.Run(() =>
            {
                try
                {
                    // Build manifest from directory.
                    Manifest = BuildManifest.BuildFromDirectory(LocalPath, VirtualPath, (string InFile, float InProgress) =>
                    {
                        Progress = InProgress * 0.5f;
                        CurrentFile = InFile;
                    });

                    ManualResetEvent PublishCompleteEvent = new ManualResetEvent(false);
                    ManifestPublishResultRecievedHandler PublishHandler = (Guid ManifestId, PublishManifestResult Result) =>
                    {
                        if (ManifestId == Manifest.Guid)
                        {
                            switch (Result)
                            {
                                case PublishManifestResult.Success:
                                    {
                                        break;
                                    }
                                case PublishManifestResult.VirtualPathAlreadyExists:
                                    {
                                        State = BuildPublishingState.FailedVirtualPathAlreadyExists;
                                        break;
                                    }
                                case PublishManifestResult.GuidAlreadyExists:
                                    {
                                        State = BuildPublishingState.FailedGuidAlreadyExists;
                                        break;
                                    }
                                case PublishManifestResult.Failed:
                                default:
                                    {
                                        State = BuildPublishingState.Failed;
                                        break;
                                    }
                            }

                            PublishCompleteEvent.Set();
                        }
                    };

                    Program.NetClient.OnManifestPublishResultRecieved += PublishHandler;

                    // Submit to the server.
                    State = BuildPublishingState.UploadingManifest;
                    if (!Program.NetClient.PublishManifest(Manifest))
                    {
                        State = BuildPublishingState.Failed;
                        return;
                    }

                    // Wait for complete delegate.
                    PublishCompleteEvent.WaitOne();

                    Program.NetClient.OnManifestPublishResultRecieved -= PublishHandler;

                    // Abort here if publishing failed.
                    if (State != BuildPublishingState.UploadingManifest)
                    {
                        return;
                    }

                    string LocalFolder = Program.ManifestDownloadManager.GetManifestStorageDirectory(Manifest);

                    // Recreated directory.
                    if (Directory.Exists(LocalFolder))
                    {
                        FileUtils.DeleteDirectory(LocalFolder);
                    }
                    Directory.CreateDirectory(LocalFolder);

                    // Copy each file over to the storage folder.
                    State = BuildPublishingState.CopyingFiles;
                    for (int i = 0; i < Manifest.Files.Count; i++)
                    {
                        BuildManifestFileInfo FileInfo = Manifest.Files[i];

                        string Src = Path.Combine(LocalPath, FileInfo.Path);
                        string Dst = Path.Combine(LocalFolder, FileInfo.Path);

                        string DstDir = Path.GetDirectoryName(Dst);
                        if (!Directory.Exists(DstDir))
                        {
                            Directory.CreateDirectory(DstDir);
                        }

                        Progress = 50.0f + ((i / (float)Manifest.Files.Count) * 50.0f);
                        CurrentFile = Src;

                        File.Copy(Src, Dst);
                    }

                    State = BuildPublishingState.Success;
                    Progress = 100;
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Manifest publishing failed with exception: {0}", ex.Message);
                    State = BuildPublishingState.Failed;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            // Add "completed" manifest downloader entry for this build so we can start seeding it.
            // Progress 50-100 is the copy to our storage location.
            ManifestDownloadState LocalState = Program.ManifestDownloadManager.AddLocalDownload(Manifest, LocalPath);

            Program.SaveSettings();
        }
    }
}
