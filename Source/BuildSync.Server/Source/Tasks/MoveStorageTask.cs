using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Server.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public enum MoveStorageState
    {
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
            State = MoveStorageState.CopyingFiles;
            Progress = 0;

            Task.Run(() =>
            {
                try
                {
                    // Incase someone has parented the storage directory to a root drive or something
                    // super stupid like that, make sure we only move our sub directories around.
                    string[] OldDirectories =
                    {
                        Path.Combine(SrcPath, "Manifests"),
                    };
                    string[] NewDirectories =
                    {
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
                                        Progress = (float)i / (float)FilesToCopy.Count;
                                        SubProgress = (float)TotalBytes / (float)FileLength;

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
                }
                catch (Exception Ex)
                {
                    State = MoveStorageState.Failed;
                }
            });
        }
    }
}
