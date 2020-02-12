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

using BuildSync.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BuildSync.Client.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public enum MoveStorageState
    {
        WaitingForIOQueueToDrain,
        WaitingForDownloadInitToFinish,
        WaitingForDownloadValidationToFinish,
        WaitingForDownloadInstallToFinish,
        CopyingFiles,
        CleaningUpOldDirectory,
        FailedDiskError,
        Failed,
        Success,
        Unknown
    }

    /// <summary>
    /// 
    /// </summary>
    public class MoveStorageTask
    {
        private string SrcPath = "";
        private string DestPath = "";

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
        public float SubProgress
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
        public MoveStorageState State
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
        public void Start(string InSrcPath, string InDestPath)
        {
            SrcPath = InSrcPath;
            DestPath = InDestPath;
            State = MoveStorageState.WaitingForIOQueueToDrain;
            Progress = 0;

            // Disconnect from everybody.
            Program.NetClient.ConnectionsDisabled = true;

            Task.Run(() =>
            {
                try
                {
                    // Drain the IO queue.
                    State = MoveStorageState.WaitingForIOQueueToDrain;
                    Progress = 0;
                    while (!Program.IOQueue.IsEmpty)
                    {
                        Thread.Sleep(1);
                    }

                    // Wait for initialization or validation of any manifests to complete.
                    State = MoveStorageState.WaitingForDownloadInitToFinish;
                    Progress = 0;
                    while (Program.ManifestDownloadManager.DownloadInitializationInProgress)
                    {
                        Thread.Sleep(1);
                    }

                    State = MoveStorageState.WaitingForDownloadValidationToFinish;
                    Progress = 0;
                    while (Program.ManifestDownloadManager.DownloadValidationInProgress)
                    {
                        Thread.Sleep(1);
                    }

                    State = MoveStorageState.WaitingForDownloadInstallToFinish;
                    Progress = 0;
                    while (Program.ManifestDownloadManager.DownloadInstallInProgress)
                    {
                        Thread.Sleep(1);
                    }

                    // Incase someone has parented the storage directory to a root drive or something
                    // super stupid like that, make sure we only move our sub directories around.
                    string[] OldDirectories =
                    {
                        Path.Combine(SrcPath, "Builds"),
                        Path.Combine(SrcPath, "Manifests"),
                    };
                    string[] NewDirectories =
                    {
                        Path.Combine(DestPath, "Builds"),
                        Path.Combine(DestPath, "Manifests"),
                    };

                    try
                    {
                        // Make list of everything we need to copy around.
                        List<Tuple<string, string, string>> FilesToCopy = new List<Tuple<string, string, string>>();
                        foreach (string Dir in OldDirectories)
                        {
                            string[] Files = Directory.GetFiles(SrcPath, "*", SearchOption.AllDirectories);
                            foreach (string File in Files)
                            {
                                string RelativePath = File.Substring(SrcPath.Length).Trim('\\', '/');
                                string NewPath = Path.Combine(DestPath, RelativePath);
                                FilesToCopy.Add(new Tuple<string, string, string>(File, NewPath, RelativePath));
                            }
                        }

                        // Get copying.
                        for (int i = 0; i < FilesToCopy.Count; i++)
                        {
                            string OldPath = FilesToCopy[i].Item1;
                            string NewPath = FilesToCopy[i].Item2;
                            string RelativePath = FilesToCopy[i].Item3;

                            string NewPathDir = Path.GetDirectoryName(NewPath);

                            if (!Directory.Exists(NewPathDir))
                            {
                                Directory.CreateDirectory(NewPathDir);
                            }

                            byte[] CopyBuffer = new byte[1024 * 1024];
                            using (FileStream Source = new FileStream(OldPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                            {
                                long FileLength = Source.Length;
                                using (FileStream Dest = new FileStream(NewPath, FileMode.Create, FileAccess.Write))
                                {
                                    long TotalBytes = 0;
                                    int CurrentBlockSize = 0;

                                    while ((CurrentBlockSize = Source.Read(CopyBuffer, 0, CopyBuffer.Length)) > 0)
                                    {
                                        TotalBytes += CurrentBlockSize;

                                        State = MoveStorageState.CopyingFiles;
                                        CurrentFile = RelativePath;
                                        Progress = i / (float)FilesToCopy.Count;
                                        SubProgress = TotalBytes / (float)FileLength;

                                        Dest.Write(CopyBuffer, 0, CurrentBlockSize);
                                    }

                                    Dest.Flush();
                                    Dest.Close();
                                }

                                Source.Flush();
                                Source.Close();
                            }
                        }

                        // Delete old directory.
                        State = MoveStorageState.CleaningUpOldDirectory;
                        SubProgress = 0;
                        Progress = 1;
                        foreach (string Dir in OldDirectories)
                        {
                            try
                            {
                                FileUtils.DeleteDirectory(Dir);
                            }
                            catch (Exception Ex)
                            {
                                Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to delete directory {0} with error: {1}", Dir, Ex.Message);
                            }
                        }

                        // Update local paths of all manifests.
                        Program.ManifestDownloadManager.UpdateStoragePath(Path.Combine(DestPath, "Builds"));
                        Program.BuildRegistry.UpdateStoragePath(Path.Combine(DestPath, "Manifests"));

                        State = MoveStorageState.Success;
                    }
                    catch (Exception Ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to move storage directory with error: {0}", Ex.Message);

                        // Delete all the copied files.
                        foreach (string Dir in NewDirectories)
                        {
                            try
                            {
                                FileUtils.DeleteDirectory(Dir);
                            }
                            catch (Exception SubEx)
                            {
                                Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to delete directory {0} with error: {1}", Dir, SubEx.Message);
                            }
                        }

                        State = MoveStorageState.FailedDiskError;
                    }
                    finally
                    {
                        Program.NetClient.ConnectionsDisabled = false;
                    }
                }
                catch (Exception)
                {
                    State = MoveStorageState.Failed;
                }
            });
        }
    }
}
