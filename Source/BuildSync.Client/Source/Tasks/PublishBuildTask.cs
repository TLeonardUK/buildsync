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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core;
using BuildSync.Core.Client;
using BuildSync.Core.Downloads;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Tasks
{
    /// <summary>
    /// </summary>
    public enum BuildPublishingState
    {
        ScanningFiles,
        CopyingFiles,
        UploadingManifest,
        FailedVirtualPathAlreadyExists,
        FailedGuidAlreadyExists,
        PermissionDenied,
        Failed,
        Success,
        Unknown
    }

    /// <summary>
    /// </summary>
    public class PublishBuildTask
    {
        private string LocalPath = "";
        private string StoragePath = "";
        private BuildManifest Manifest;

        /// <summary>
        /// </summary>
        public string CurrentFile { get; private set; }

        /// <summary>
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// </summary>
        public BuildPublishingState State { get; private set; }

        /// <summary>
        /// </summary>
        public void Commit()
        {
            // Add "completed" manifest downloader entry for this build so we can start seeding it.
            ManifestDownloadState LocalState = Program.ManifestDownloadManager.AddLocalDownload(Manifest, StoragePath);

            // Allow this manifest id to be downloaded now.
            Program.ManifestDownloadManager.UnblockDownload(Manifest.Guid);

            Program.SaveSettings();
        }

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="VirtualPath"></param>
        /// <param name="LocalPath"></param>
        public void Start(string VirtualPath, string InLocalPath)
        {
            LocalPath = InLocalPath;
            State = BuildPublishingState.ScanningFiles;

            Task.Run(
                () =>
                {
                    try
                    {
                        Guid NewManifestId = Guid.NewGuid();

                        // Calculate max size of all files in folder to publish.
                        string[] Files = Directory.GetFiles(LocalPath, "*", SearchOption.AllDirectories);
                        long TotalSize = 0;
                        for (int i = 0; i < Files.Length; i++)
                        {
                            TotalSize += new FileInfo(Files[i]).Length;
                        }

                        // Allocate appropriate folder to store build in.
                        if (!Program.StorageManager.AllocateSpace(TotalSize, NewManifestId, out StoragePath))
                        {
                            Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to allocate space, storage folders likely at capacity.");

                            State = BuildPublishingState.Failed;
                            return;
                        }

                        // Don't allow downloading of this manifest until we have fully committed it.
                        Program.ManifestDownloadManager.BlockDownload(NewManifestId);

                        // Recreated directory.
                        if (Directory.Exists(StoragePath))
                        {
                            FileUtils.DeleteDirectory(StoragePath);
                        }

                        Directory.CreateDirectory(StoragePath);

                        // Copy each file over to the storage folder.
                        State = BuildPublishingState.CopyingFiles;
                        for (int i = 0; i < Files.Length; i++)
                        {
                            string Src = Files[i];
                            string RelativePath = Files[i].Substring(LocalPath.Length).TrimStart('/', '\\');
                            string Dst = Path.Combine(StoragePath, RelativePath);

                            string DstDir = Path.GetDirectoryName(Dst);
                            if (!Directory.Exists(DstDir))
                            {
                                Directory.CreateDirectory(DstDir);
                            }

                            Progress = i / (float) Files.Length * 50.0f;
                            CurrentFile = Src;

                            File.Copy(Src, Dst);
                        }

                        // Build manifest from directory.
                        Manifest = BuildManifest.BuildFromDirectory(
                            NewManifestId, StoragePath, VirtualPath, Program.IOQueue, (InFile, InProgress) =>
                            {
                                Progress = 50.0f + InProgress * 0.5f;
                                if (InFile.Length > 0)
                                {
                                    CurrentFile = InFile;
                                }
                            }
                        );

                        ManualResetEvent PublishCompleteEvent = new ManualResetEvent(false);
                        ManifestPublishResultRecievedHandler PublishHandler = (ManifestId, Result) =>
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
                                    case PublishManifestResult.PermissionDenied:
                                    {
                                        State = BuildPublishingState.PermissionDenied;
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

                        State = BuildPublishingState.Success;
                        Progress = 100;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Main, "Manifest publishing failed with exception: {0}", ex.Message);
                        State = BuildPublishingState.Failed;
                    }
                }
            );
        }
    }
}