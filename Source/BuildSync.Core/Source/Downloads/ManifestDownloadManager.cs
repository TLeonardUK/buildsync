//#define CHECKSUM_EACH_BLOCK

using BuildSync.Core.Manifests;
using BuildSync.Core.Networking;
using BuildSync.Core.Utils;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ManifestId"></param>
    public delegate void ManifestRequestedHandler(Guid ManifestId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ManifestId"></param>
    public delegate void DownloadErrorHandler(Guid ManifestId);

    /// <summary>
    /// 
    /// </summary>
    public delegate void BlockAccessCompleteHandler(bool bSuccess);

    /// <summary>
    /// 
    /// </summary>
    public class ManifestDownloadRequiredBlock
    {
        public Guid ManifestId;
        public int RangeStart;
        public int RangeEnd;
        public long Size;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ManifestDownloadQueue
    {
        public Guid ManifestId;
        public int HighestPriority = -1;
        public List<ManifestDownloadRequiredBlock> ToDownloadBlocks = new List<ManifestDownloadRequiredBlock>();
    };

    /// <summary>
    /// 
    /// </summary>
    public struct ManifestPendingDownloadBlock
    {
        public Guid ManifestId;
        public int BlockIndex;

        public ulong TimeStarted;
        public long Size;

        public bool Recieved;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ManifestDownloadManager
    {
        /// <summary>
        /// 
        /// </summary>
        public event ManifestRequestedHandler OnManifestRequested;

        /// <summary>
        /// 
        /// </summary>
        public event DownloadErrorHandler OnDownloadError;

        /// <summary>
        /// 
        /// </summary>
        private ManifestDownloadStateCollection StateCollection = new ManifestDownloadStateCollection();

        /// <summary>
        /// 
        /// </summary>
        private BuildManifestRegistry ManifestRegistry = null;

        /// <summary>
        /// 
        /// </summary>
        public AsyncIOQueue IOQueue = null;

        /// <summary>
        /// 
        /// </summary>
        private BlockListState AvailableBlocks = null;

        /// <summary>
        /// 
        /// </summary>
        public List<ManifestPendingDownloadBlock> DownloadQueue
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        private string StorageRootPath = "";

        /// <summary>
        /// 
        /// </summary>
        private List<string> PendingOrphanCleanups = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public long StorageMaxSize
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private const int ManifestRequestInterval = 30 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private const int IdealDownloadQueueSizeBytes = 150 * 1024 * 1024;

        /// <summary>
        /// 
        /// </summary>
        private const int MaxDownloadQueueItems = 250;

        /// <summary>
        /// 
        /// </summary>
        private bool InternalTrafficEnabled = true;
        public bool TrafficEnabled
        {
            get { return InternalTrafficEnabled; }
            set
            {
                if (InternalTrafficEnabled != value)
                {
                    if (!value && DownloadQueue != null)
                    {
                        DownloadQueue.Clear();
                    }
                    InternalTrafficEnabled = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DownloadInitializationInProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DownloadValidationInProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DownloadInstallInProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AreStatesDirty
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SkipValidation
        {
            get;
            set;
        } = false;

        /// <summary>
        /// 
        /// </summary>
        private int Internal_StateDirtyCount = 0;
        public int StateDirtyCount
        {
            get
            {
                return Internal_StateDirtyCount;
            }
            set
            {
                Internal_StateDirtyCount = value;
                AreStatesDirty = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ManifestDownloadStateCollection States
        {
            get { return StateCollection; }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<Guid> BlockedDownloadManifestIds = new List<Guid>();

        /// <summary>
        /// 
        /// </summary>
        private ulong SplitIndexLastUpdatedTimer = 0;

        /// <summary>
        /// 
        /// </summary>
        private int SplitIndex = -1;

        /// <summary>
        /// 
        /// </summary>
        private const int SplitIndexUpdateInterval = 5 * 60 * 1000;

        /// <summary>
        /// 
        /// </summary>
        public BlockListState GetBlockListState(bool ReturnEmptyIfTrafficDisabled = true)
        {
            BlockListState Result = new BlockListState();

            if (ReturnEmptyIfTrafficDisabled && !TrafficEnabled)
            {
                Result.States = new ManifestBlockListState[0];
                return Result;
            }

            Result.States = new ManifestBlockListState[StateCollection.States.Count];

            for (int i = 0; i < StateCollection.States.Count; i++)
            {
                ManifestDownloadState Downloader = StateCollection.States[i];

                ManifestBlockListState ManifestState = new ManifestBlockListState();
                ManifestState.Id = Downloader.ManifestId;
                ManifestState.IsActive = (Downloader.State == ManifestDownloadProgressState.Downloading && !Downloader.Paused);
                ManifestState.BlockState = Downloader.BlockStates;

                Result.States[i] = ManifestState;
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestDownloader"></param>
        /// <param name="Config"></param>
        public void Start(string InStorageRootPath, long InStorageMaxSize, ManifestDownloadStateCollection ResumeStateCollection, BuildManifestRegistry Registry, AsyncIOQueue InIOQueue)
        {
            StorageRootPath = InStorageRootPath;
            StorageMaxSize = InStorageMaxSize;

            IOQueue = InIOQueue;

            ManifestRegistry = Registry;
            StateCollection = ResumeStateCollection;
            if (StateCollection == null)
            {
                StateCollection = new ManifestDownloadStateCollection();
            }

            foreach (ManifestDownloadState State in StateCollection.States)
            {
                State.Manifest = ManifestRegistry.GetManifestById(State.ManifestId);
                if (State.Manifest == null || !Directory.Exists(State.LocalFolder))
                {
                    // If we don't have the manifest locally we cannot determine we have all blocks, so mark all as unavailable.
                    // Someone probably balls up their local storage.
                    State.BlockStates.SetAll(false);
                    StateDirtyCount++;

                    State.State = ManifestDownloadProgressState.RetrievingManifest;
                }
            }

            CleanUpOrphanBuilds();

            StateDirtyCount++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NewDirectory"></param>
        public void UpdateStoragePath(string NewDirectory)
        {
            string OldPath = StorageRootPath;
            StorageRootPath = NewDirectory;

            foreach (ManifestDownloadState State in StateCollection.States)
            {
                string RelativePath = State.LocalFolder.Substring(OldPath.Length).Trim('\\', '/');
                State.LocalFolder = Path.Combine(NewDirectory, RelativePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <returns></returns>
        public string GetManifestStorageDirectory(Guid ManifestId)
        {
            return Path.Combine(StorageRootPath, ManifestId.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        public void BlockDownload(Guid ManifestId)
        {
            lock (BlockedDownloadManifestIds)
            {
                BlockedDownloadManifestIds.Add(ManifestId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        public void UnblockDownload(Guid ManifestId)
        {
            lock (BlockedDownloadManifestIds)
            {
                BlockedDownloadManifestIds.Remove(ManifestId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Priority"></param>
        public ManifestDownloadState AddLocalDownload(BuildManifest Manifest, string LocalPath, bool Available = true)
        {
            ManifestDownloadState State = GetDownload(Manifest.Guid);

            if (ManifestRegistry.GetManifestById(Manifest.Guid) == null)
            {
                ManifestRegistry.RegisterManifest(Manifest);
            }

            if (State == null)
            {
                State = new ManifestDownloadState();
                State.Id = Guid.NewGuid();
            }
            State.ManifestId = Manifest.Guid;
            State.Priority = 2;
            State.Manifest = Manifest;
            State.State = Available ? ManifestDownloadProgressState.Complete : ManifestDownloadProgressState.Validating;
            State.LocalFolder = Path.Combine(StorageRootPath, State.ManifestId.ToString());
            State.LastActive = DateTime.Now;

            // We have everything.
            State.BlockStates.Resize((int)Manifest.BlockCount);
            State.BlockStates.SetAll(true);

            StoreFileCompletedStates(State);

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Added local download of manifest: {0}", Manifest.Guid.ToString());
            StateCollection.States.Add(State);

            StateDirtyCount++;

            return State;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Priority"></param>
        public ManifestDownloadState StartDownload(Guid ManifestId, int Priority)
        {
            lock (BlockedDownloadManifestIds)
            {
                if (BlockedDownloadManifestIds.Contains(ManifestId))
                {
                    return null;
                }
            }

            ManifestDownloadState State = GetDownload(ManifestId);
            if (State != null)
            {
                return State;
            }

            State = new ManifestDownloadState();
            State.Id = Guid.NewGuid();
            State.ManifestId = ManifestId;
            State.Priority = Priority;
            State.Manifest = null;
            State.State = ManifestDownloadProgressState.RetrievingManifest;
            State.LocalFolder = Path.Combine(StorageRootPath, State.ManifestId.ToString());
            State.LastActive = DateTime.Now;

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Started download of manifest: {0}", ManifestId.ToString());
            StateCollection.States.Add(State);

            StateDirtyCount++;

            return State;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Priority"></param>
        public void RestartDownload(Guid ManifestId)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                return;
            }

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Restarted download of manifest: {0}", ManifestId.ToString());
            State.Paused = false;
            State.BlockStates.SetAll(false);
            State.State = ManifestDownloadProgressState.RetrievingManifest;

            ClearFileCompletedStates(State);

            StateDirtyCount++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Priority"></param>
        public void ValidateDownload(Guid ManifestId)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                return;
            }

            if (State.State != ManifestDownloadProgressState.Complete)
            {
                return;
            }

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Validating download of manifest: {0}", ManifestId.ToString());
            State.Paused = false;
            State.State = ManifestDownloadProgressState.Validating;

            StateDirtyCount++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Priority"></param>
        public void InstallDownload(Guid ManifestId)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                return;
            }

            if (State.State != ManifestDownloadProgressState.Complete)
            {
                return;
            }

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Installing download of manifest: {0}", ManifestId.ToString());
            State.Paused = false;
            State.State = ManifestDownloadProgressState.Installing;

            StateDirtyCount++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        public void PauseDownload(Guid ManifestId)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                return;
            }

            if (!State.Paused)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Paused download of manifest: {0}", ManifestId.ToString());
                State.Paused = true;

                StateDirtyCount++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        public void ResumeDownload(Guid ManifestId)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                return;
            }

            if (State.Paused)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Resumed download of manifest: {0}", ManifestId.ToString());
                State.Paused = false;

                StateDirtyCount++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        public ManifestDownloadState GetDownload(Guid ManifestId)
        {
            foreach (ManifestDownloadState State in StateCollection.States)
            {
                if (State.ManifestId == ManifestId)
                {
                    return State;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        private void PerformInstallation(ManifestDownloadState State)
        {
            string ConfigFilePath = Path.Combine(State.LocalFolder, "buildsync.json");
            if (!File.Exists(ConfigFilePath))
            {
                return;
            }

            BuildSettings Settings;
            if (!BuildSettings.Load<BuildSettings>(ConfigFilePath, out Settings))
            {
                throw new Exception("The included buildsync.json file could not be loaded, it may be malformed.");
            }

            List<BuildLaunchMode> Modes;
            try
            {
                Modes = Settings.Compile();

                // Add various internal variables to pass in bits of info.
                foreach (BuildLaunchMode Mode in Modes)
                {
                    Mode.AddStringVariable("INSTALL_DEVICE_NAME", State.InstallDeviceName);
                    Mode.AddStringVariable("BUILD_DIR", State.LocalFolder);
                }
            }
            catch (InvalidOperationException Ex)
            {
                throw new Exception("Error encountered while evaluating launch settings:\n\n" + Ex.Message);
            }

            // Install each mode.
            foreach (BuildLaunchMode Mode in Modes)
            {
                string ErrorMessage = "";
                if (!Mode.Install(State.LocalFolder, ref ErrorMessage))
                {
                    throw new Exception("Error encountered while evaluating installing:\n\n" + ErrorMessage);
                }
            }

            State.Installed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        private void StoreFileCompletedStates(ManifestDownloadState State)
        {
            // Store timestamps of files so we can determine if someone has modified the files after our validation.
            State.FileCompletedStates.Clear();
            foreach (BuildManifestFileInfo FileInfo in State.Manifest.Files)
            {
                string LocalPath = Path.Combine(State.LocalFolder, FileInfo.Path);

                ManifestFileCompletedState CompletedState = new ManifestFileCompletedState();
                CompletedState.Path = FileInfo.Path;
                CompletedState.ModifiedTimestampOnCompleted = GetFileLastWriteTime(LocalPath);
                State.FileCompletedStates.Add(CompletedState);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        private void ClearFileCompletedStates(ManifestDownloadState State)
        {
            State.FileCompletedStates.Clear();
            ClearFileWriteCache(State.LocalFolder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        private void PollDownload(ManifestDownloadState State)
        {
            if (State.Paused)
            {
                return;
            }

            switch (State.State)
            {
                // Request manifest from central server.
                case ManifestDownloadProgressState.RetrievingManifest:
                    {
                        State.Manifest = ManifestRegistry.GetManifestById(State.ManifestId);
                        if (State.Manifest != null)
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Retrieved manifest for download '{0}', starting download.", State.Id);
                            ChangeState(State, ManifestDownloadProgressState.Initializing);
                            State.BlockStates.Resize((int)State.Manifest.BlockCount);

                            StateDirtyCount++;
                        }
                        else
                        {
                            // Request manifest from server.
                            ulong Elapsed = TimeUtils.Ticks - State.LastManifestRequestTime;
                            if (State.LastManifestRequestTime == 0 || Elapsed > ManifestRequestInterval)
                            {
                                OnManifestRequested?.Invoke(State.ManifestId);
                                State.LastManifestRequestTime = TimeUtils.Ticks;
                            }
                        }

                        break;
                    }

                // Create all the files in the directory.
                case ManifestDownloadProgressState.Initializing:
                    {
                        if (State.InitializeTask == null)
                        {
                            if (IOQueue.CloseAllStreamsInDirectory(State.LocalFolder))
                            {
                                State.InitializeTask = Task.Run(() =>
                                {
                                    try
                                    {
                                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Initializing directory: {0}", State.LocalFolder);
                                        State.Manifest.InitializeDirectory(State.LocalFolder, IOQueue, (float Progress) =>
                                        {
                                            State.InitializeProgress = Progress;
                                        });
                                        ChangeState(State, ManifestDownloadProgressState.Downloading);
                                    }
                                    catch (Exception Ex)
                                    {
                                        ChangeState(State, ManifestDownloadProgressState.InitializeFailed, true);
                                        Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to intialize directory with error: {0}", Ex.Message);
                                    }
                                    finally
                                    {
                                        State.InitializeTask = null;
                                    }
                                });
                            }
                            else
                            {
                                ChangeState(State, ManifestDownloadProgressState.ValidationFailed, true);
                                Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to initialize, unable to close all streams to directory.");
                            }
                        }

                        break;
                    }

                // Restarting from an initialization failure, try again.
                case ManifestDownloadProgressState.InitializeFailed:
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Retrying initialization after resume from error.");
                        ChangeState(State, ManifestDownloadProgressState.Initializing);
                        break;
                    }

                // Installing all launch modes on device.
                case ManifestDownloadProgressState.Installing:
                    {
                        if (State.InstallTask == null)
                        {
                            State.InstallTask = Task.Run(() =>
                            {
                                try
                                {
                                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Installing on device {0} directory: {1}", State.InstallDeviceName, State.LocalFolder);
                                    PerformInstallation(State);
                                    ChangeState(State, ManifestDownloadProgressState.Complete);
                                }
                                catch (Exception Ex)
                                {
                                    ChangeState(State, ManifestDownloadProgressState.InstallFailed, true);
                                    Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to install with error: {0}", Ex.Message);
                                }
                                finally
                                {
                                    State.InstallTask = null;
                                }
                            });
                        }

                        break;
                    }

                // Restarting from an install failure, try again.
                case ManifestDownloadProgressState.InstallFailed:
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Retrying install after resume from error.");
                        ChangeState(State, ManifestDownloadProgressState.Installing);
                        break;
                    }

                // Get downloading them blocks.
                case ManifestDownloadProgressState.Downloading:
                    {
                        if (State.BlockStates.AreAllSet(true))
                        {
                            if (SkipValidation)
                            {
                                ChangeState(State, ManifestDownloadProgressState.Complete);
                            }
                            else
                            {
                                ChangeState(State, ManifestDownloadProgressState.Validating);
                            }
                        }

                        break;
                    }

                // Download complete, just monitor for needing to switch back to downloading if we loose blocks.
                case ManifestDownloadProgressState.Complete:
                    {
                        if (!State.BlockStates.AreAllSet(true))
                        {
                            ChangeState(State, ManifestDownloadProgressState.Downloading);
                        }

                        break;
                    }

                // Check all the file checksums match, if any fail, requeue all their blocks.
                case ManifestDownloadProgressState.Validating:
                    {
                        if (State.ValidationTask == null)
                        {
                            // Close all async io streams to the download we are working on.
                            if (IOQueue.CloseAllStreamsInDirectory(State.LocalFolder))
                            {
                                State.ValidationTask = Task.Run(() =>
                                {
                                    try
                                    {
                                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Validating directory: {0}", State.LocalFolder);
                                        List<int> FailedBlocks = State.Manifest.Validate(State.LocalFolder, AsyncIOQueue.GlobalBandwidthStats, IOQueue, (float Progress) => {
                                            State.ValidateProgress = Progress; 
                                        });
                                        if (FailedBlocks.Count == 0)
                                        {
                                            if (State.InstallOnComplete)
                                            {
                                                ChangeState(State, ManifestDownloadProgressState.Installing);
                                            }
                                            else
                                            {
                                                ChangeState(State, ManifestDownloadProgressState.Complete);
                                            }

                                            StoreFileCompletedStates(State);
                                        }
                                        else
                                        {
                                            foreach (int Block in FailedBlocks)
                                            {
                                                MarkBlockAsUnavailable(State.ManifestId, Block);
                                                //MarkFileAsUnavailable(State.ManifestId, File);
                                            }

                                            ChangeState(State, ManifestDownloadProgressState.ValidationFailed, true);
                                        }
                                    }
                                    catch (Exception Ex)
                                    {
                                        ChangeState(State, ManifestDownloadProgressState.ValidationFailed, true);
                                        Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to validate directory with error: {0}", Ex.Message);
                                    }
                                    finally
                                    {
                                        State.ValidationTask = null;
                                    }
                                });
                            }
                            else
                            {
                                ChangeState(State, ManifestDownloadProgressState.ValidationFailed, true);
                                Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to validate, unable to close all streams to directory.");
                            }
                        }

                        break;
                    }

                // Restarting from an validation failure, try again.
                case ManifestDownloadProgressState.ValidationFailed:
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Retrying downloading after resume from validation error.");
                        ChangeState(State, ManifestDownloadProgressState.Downloading);
                        break;
                    }
            }
        }

        private struct LastFileWriteTimeCacheEntry
        {
            public DateTime LastModified;
            public ulong LastCacheUpdate;
        }

        private Dictionary<string, LastFileWriteTimeCacheEntry> LastFileWriteCache = new Dictionary<string, LastFileWriteTimeCacheEntry>();
        private const int MaxLastFileWriteCacheEntryDuration = 5 * 1000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private DateTime GetFileLastWriteTime(string Path)
        {
            lock (LastFileWriteCache)
            {
                ulong time = TimeUtils.Ticks;
                if (LastFileWriteCache.ContainsKey(Path))
                {
                    if (time - LastFileWriteCache[Path].LastCacheUpdate > MaxLastFileWriteCacheEntryDuration)
                    {
                        LastFileWriteCache.Remove(Path);
                    }
                    else
                    {
                        return LastFileWriteCache[Path].LastModified;
                    }
                }

                FileInfo Info = new FileInfo(Path);
                if (!Info.Exists)
                {
                    return DateTime.UtcNow;
                }

                LastFileWriteTimeCacheEntry Entry;
                Entry.LastModified = Info.LastWriteTimeUtc;
                Entry.LastCacheUpdate = time;
                LastFileWriteCache.Add(Path, Entry);

                return Entry.LastModified;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private void ClearFileWriteCache(string Path)
        {
            string NormalizedPath = FileUtils.NormalizePath(Path);

            lock (LastFileWriteCache)
            {
                ulong time = TimeUtils.Ticks;
                List<string> KeysToRemove = new List<string>();
                foreach (var pair in LastFileWriteCache)
                {
                    if (FileUtils.NormalizePath(pair.Key) == NormalizedPath)
                    {
                        KeysToRemove.Add(pair.Key);
                    }
                }
                foreach (string key in KeysToRemove)
                {
                    LastFileWriteCache.Remove(key);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NewState"></param>
        private void ChangeState(ManifestDownloadState State, ManifestDownloadProgressState NewState, bool IsError = false)
        {
            State.State = NewState;
            StateDirtyCount++;

            if (IsError)
            {
                State.Paused = true;
                OnDownloadError?.Invoke(State.ManifestId);
            }

            if (NewState != ManifestDownloadProgressState.Complete)
            {
                State.Installed = false;
                ClearFileCompletedStates(State);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            bool AnyValidating = false;
            bool AnyInitialising = false;
            bool AnyInstalling = false;

            foreach (ManifestDownloadState State in StateCollection.States)
            {
                PollDownload(State);

                if (State.State == ManifestDownloadProgressState.Initializing)
                {
                    AnyInitialising = true;
                }
                else if (State.State == ManifestDownloadProgressState.Validating)
                {
                    AnyValidating = true;
                }
                else if (State.State == ManifestDownloadProgressState.Installing)
                {
                    AnyInstalling = true;
                }
            }

            DownloadInitializationInProgress = AnyInitialising;
            DownloadValidationInProgress = AnyValidating;
            DownloadInstallInProgress = AnyInstalling;

            PruneDiskSpace();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PruneDiskSpace()
        {
            long TotalSize = 0;
            foreach (ManifestDownloadState State in StateCollection.States)
            {
                if (State.Manifest != null)
                {
                    TotalSize += State.Manifest.GetTotalSize();
                }
            }

            if (TotalSize > StorageMaxSize)
            {
                // Select manifests for deletion.
                List<ManifestDownloadState> DeletionCandidates = new List<ManifestDownloadState>();
                foreach (ManifestDownloadState State in StateCollection.States)
                {
                    if (!State.Active)
                    {
                        if (!FileUtils.AnyRunningProcessesInDirectory(State.LocalFolder))
                        {
                            DeletionCandidates.Add(State);
                        }
                    }
                }

                DeletionCandidates.Sort((Item1, Item2) => Item1.LastActive.CompareTo(Item2.LastActive));

                // Delete this state.
                if (DeletionCandidates.Count > 0)
                {
                    ManifestDownloadState State = DeletionCandidates[0];

                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Deleting download to prune storage space: {0}", State.ManifestId.ToString());

                    // Remove state from our state collection.
                    StateCollection.States.Remove(State);
                    StateDirtyCount++;

                    // Ask IO queue to delete the folder.
                    IOQueue.DeleteDir(State.LocalFolder, null);

                    // Remove the manifest from the registry.
                    ManifestRegistry.UnregisterManifest(State.ManifestId);
                }

                CleanUpOrphanBuilds();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanUpOrphanBuilds()
        {
            if (Directory.Exists(StorageRootPath))
            {
                // Check if there are any folders in the storage directory that do not have a manifest associated with them.
                foreach (string Dir in Directory.GetDirectories(StorageRootPath))
                {
                    Guid ManifestId = Guid.Empty;

                    if (Guid.TryParse(Path.GetFileName(Dir), out ManifestId))
                    {
                        BuildManifest Manifest = ManifestRegistry.GetManifestById(ManifestId);
                        if (Manifest == null)
                        {
                            lock (PendingOrphanCleanups)
                            {
                                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Deleting directory in storage folder that appears to have no matching manifest (probably a previous failed delete): {0}", Dir);
                                if (!PendingOrphanCleanups.Contains(Dir))
                                {
                                    PendingOrphanCleanups.Add(Dir);
                                    IOQueue.DeleteDir(Dir, (bool bSuccess) =>
                                    {
                                        PendingOrphanCleanups.Remove(Dir);
                                    });
                                }
                            }
                        }
                        else
                        {
                            // If we have a manifest but no download state add as local download
                            // with no available blocks so we can clean it up if needed for space.
                            if (GetDownload(ManifestId) == null)
                            {
                                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Found build directory for manifest, but no download state, added as local download, might have been orphaned due to settings save failure?: {0}", Dir);
                                AddLocalDownload(Manifest, Dir, false);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="BlockIndex"></param>
        /// <param name="Data"></param>
        public bool GetBlockInfo(Guid ManifestId, int BlockIndex, ref BuildManifestBlockInfo BlockInfo)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request for block from manifest we do not have.");
                return false;
            }

            if (!State.Manifest.GetBlockInfo(BlockIndex, ref BlockInfo))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request for invalid block info.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public class BlockIOState
        {
            public int SubBlocksRemaining = 0;
            public bool WasSuccess = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="BlockIndex"></param>
        /// <param name="Data"></param>
        public bool GetBlockData(Guid ManifestId, int BlockIndex, ref NetCachedArray Data, BlockAccessCompleteHandler Callback)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null || State.Manifest == null)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request for block from manifest we do not have.");
                Callback?.Invoke(false);
                return false;
            }

            BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
            if (!State.Manifest.GetBlockInfo(BlockIndex, ref BlockInfo))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request for invalid block info.");
                Callback?.Invoke(false);
                return false;
            }

            // Ensure we have block.
            if (!State.BlockStates.Get(BlockIndex))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request for block data for block we do not have.");
                Callback?.Invoke(false);
                return false;
            }

            BlockIOState WriteState = new BlockIOState();
            WriteState.SubBlocksRemaining = BlockInfo.SubBlocks.Count;
            WriteState.WasSuccess = true;

            Data.Resize((int)BlockInfo.TotalSize);

            byte[] DataBuffer = Data.Data;

            for (int i = 0; i < BlockInfo.SubBlocks.Count; i++)
            {
                BuildManifestSubBlockInfo SubBlockInfo = BlockInfo.SubBlocks[i];

                string LocalFile = Path.Combine(State.LocalFolder, SubBlockInfo.File.Path);
                IOQueue.Read(LocalFile, SubBlockInfo.FileOffset, SubBlockInfo.FileSize, Data.Data, SubBlockInfo.OffsetInBlock, (bool bSuccess) =>
                {
                    if (bSuccess)
                    {
                        State.BandwidthStats.Out(SubBlockInfo.FileSize);
                    }
                    else
                    {
                        WriteState.WasSuccess = false;
                    }

                    // Check file has not been modified since completed.
                    foreach (ManifestFileCompletedState CompletedState in State.FileCompletedStates)
                    {
                        if (SubBlockInfo.File.Path == CompletedState.Path)
                        {
                            if (GetFileLastWriteTime(LocalFile) != CompletedState.ModifiedTimestampOnCompleted)
                            {
                                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Block index {0} in manifest {1} failed as has been modified since it was downloaded, failed to get block data.", BlockIndex, ManifestId.ToString());
                                WriteState.WasSuccess = false;
                            }
                        }
                    }

                    if (Interlocked.Decrement(ref WriteState.SubBlocksRemaining) == 0)
                    {
                        // Checksum block to make sure nobody has balls it up.
#if CHECKSUM_EACH_BLOCK
                        if (State.Manifest.BlockChecksums != null)
                        {
                            if (Crc32.Compute(DataBuffer, (int)BlockInfo.TotalSize) != State.Manifest.BlockChecksums[BlockIndex])
                            {
                                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Block index {0} in manifest {1} failed checksum, failed to get block data.", BlockIndex, ManifestId.ToString());
                                WriteState.WasSuccess = false;
                            }
                        }
#endif

                        Callback?.Invoke(WriteState.WasSuccess);
                    }
                });
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="BlockIndex"></param>
        /// <param name="Data"></param>
        public bool SetBlockData(Guid ManifestId, int BlockIndex, NetCachedArray Data, BlockAccessCompleteHandler Callback)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to set block from manifest we do not have.");
                Callback?.Invoke(false);
                return false;
            }

            BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
            if (!State.Manifest.GetBlockInfo(BlockIndex, ref BlockInfo))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to set block which we don't have info for.");
                Callback?.Invoke(false);
                return false;
            }

            // Ensure data is valid.
            if (Data.Length != BlockInfo.TotalSize)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to write block {0} in {1} with smaller than expected data (Recieved:{2}, Expected:{3}).", BlockIndex, ManifestId.ToString(), Data.Length, BlockInfo.TotalSize);
                Callback?.Invoke(false);
                return false;
            }

            // Ensure we have block.
            if (State.BlockStates.Get(BlockIndex))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to set block data for block we not have.");
                Callback?.Invoke(false);
                return false;
            }

            BlockIOState WriteState = new BlockIOState();
            WriteState.SubBlocksRemaining = BlockInfo.SubBlocks.Count;
            WriteState.WasSuccess = true;

            for (int i = 0; i < BlockInfo.SubBlocks.Count; i++)
            {
                BuildManifestSubBlockInfo SubBlockInfo = BlockInfo.SubBlocks[i];

                string LocalFile = Path.Combine(State.LocalFolder, SubBlockInfo.File.Path);
                IOQueue.Write(LocalFile, SubBlockInfo.FileOffset, SubBlockInfo.FileSize, Data.Data, SubBlockInfo.OffsetInBlock, (bool bSuccess) =>
                {
                    if (bSuccess)
                    {
                        State.BandwidthStats.In(SubBlockInfo.FileSize);
                    }
                    else
                    {
                        WriteState.WasSuccess = false;
                    }

                    if (Interlocked.Decrement(ref WriteState.SubBlocksRemaining) == 0)
                    {
                        Callback?.Invoke(WriteState.WasSuccess);
                    }
                });
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        public void MarkBlockAsComplete(Guid ManifestId, int BlockIndex)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to set block from manifest we do not have.");
                return;
            }

            BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
            if (!State.Manifest.GetBlockInfo(BlockIndex, ref BlockInfo))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to set block which we don't have info for.");
                return;
            }

            // Mark block as having block.
            State.BlockStates.Set(BlockIndex, true);
            StateDirtyCount++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        public void MarkFileAsUnavailable(Guid ManifestId, string Path)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to unset block from manifest we do not have.");
                return;
            }

            List<int> BlockIndices = State.Manifest.GetFileBlocks(Path);
            foreach (int i in BlockIndices)
            {
                State.BlockStates.Set(i, false);
            }
            StateDirtyCount++;

            if (State.State == ManifestDownloadProgressState.Complete && BlockIndices.Count > 0)
            {
                ChangeState(State, ManifestDownloadProgressState.Downloading);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        public void MarkBlockAsUnavailable(Guid ManifestId, int BlockIndex)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null || State.Manifest == null)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to unset block from manifest we do not have.");

                // If we doin't have a manifest, just mark everything as not having it, someone probably deleted
                // manifests locally :S
                if (State.Manifest == null)
                {
                    State.BlockStates.SetAll(false);
                    StateDirtyCount++;
                }

                return;
            }

            BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
            if (!State.Manifest.GetBlockInfo(BlockIndex, ref BlockInfo))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to unset block which we don't have info for.");
                return;
            }

            // Mark block as having block.
            State.BlockStates.Set(BlockIndex, false);
            StateDirtyCount++;
            
            if (State.State == ManifestDownloadProgressState.Complete && !State.BlockStates.AreAllSet(true))
            {
                ChangeState(State, ManifestDownloadProgressState.Downloading);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        public void MarkAllBlockFilesAsUnavailable(Guid ManifestId, int BlockIndex)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null || State.Manifest == null)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to unset block from manifest we do not have.");

                // If we doin't have a manifest, just mark everything as not having it, someone probably deleted
                // manifests locally :S
                if (State.Manifest == null)
                {
                    State.BlockStates.SetAll(false);
                    StateDirtyCount++;
                }

                return;
            }

            BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
            if (!State.Manifest.GetBlockInfo(BlockIndex, ref BlockInfo))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to unset block which we don't have info for.");
                return;
            }

            foreach (BuildManifestSubBlockInfo SubInfo in BlockInfo.SubBlocks)
            {
                List<int> blocksIndexes = State.Manifest.GetFileBlocks(SubInfo.File.Path);
                foreach (int index in blocksIndexes)
                {
                    State.BlockStates.Set(index, false, true);
                }
                State.BlockStates.CompactRanges();
            }

            StateDirtyCount++;

            if (State.State == ManifestDownloadProgressState.Complete && !State.BlockStates.AreAllSet(true))
            {
                ChangeState(State, ManifestDownloadProgressState.Downloading);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetAvailableToDownloadBlocks(BlockListState InAvailableBlocks)
        {
            AvailableBlocks = InAvailableBlocks;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateBlockQueue()
        {
            if (AvailableBlocks == null || !TrafficEnabled)
            {
                return;
            }

            // Update split index periodically.
            if (SplitIndex == -1 || TimeUtils.Ticks - SplitIndexLastUpdatedTimer > SplitIndexUpdateInterval)
            {
                SplitIndex = (new Random(Environment.TickCount)).Next();
                SplitIndexLastUpdatedTimer = TimeUtils.Ticks;
            }

            // Create queues of blocks that we want.
            List<ManifestDownloadQueue> ManifestQueues = new List<ManifestDownloadQueue>();

            foreach (ManifestDownloadState State in StateCollection.States)
            {
                if (State.State == ManifestDownloadProgressState.Downloading && !State.Paused)
                {
                    // Create queue for this manifest.
                    ManifestDownloadQueue DownloadQueue = new ManifestDownloadQueue();
                    DownloadQueue.HighestPriority = State.Priority;
                    DownloadQueue.ManifestId = State.ManifestId;
                    ManifestQueues.Add(DownloadQueue);

                    // Grab a list of available blocks for this 
                    bool[] Available = null;
                    foreach (ManifestBlockListState AState in AvailableBlocks.States)
                    {
                        if (AState.Id == State.ManifestId)
                        {
                            Available = AState.BlockState.ToArray();
                        }
                    }

                    if (Available == null || Available.Length != State.BlockStates.Size)
                    {
                        continue;
                    }

                    bool[] Current = State.BlockStates.ToArray();
                    bool[] ToDownload = new bool[Available.Length];

                    // Make list of blocks that are available and blocks that we don't have. aka. our download list.
                    for (int i = 0; i < Available.Length; i++)
                    {
                        ToDownload[i] = Available[i] && !Current[i];
                    }

                    // Generate the ranges to download.
                    SparseStateArray DownloadRanges = new SparseStateArray();
                    DownloadRanges.Size = 0;
                    DownloadRanges.Ranges = null;

                    // Split at a random place and download the second segment first. This speeds up the rate at which peers have blocks available to seed. And stops
                    // the first seeder being hammered.
                    int ThisSplitIndex = SplitIndex % ToDownload.Length;
                    DownloadRanges.AddArray(ToDownload, ThisSplitIndex, ToDownload.Length - ThisSplitIndex);
                    DownloadRanges.AddArray(ToDownload, 0, ThisSplitIndex);

                    if (DownloadRanges.Ranges != null)
                    {
                        foreach (SparseStateArray.Range range in DownloadRanges.Ranges)
                        {
                            if (range.State)
                            {
                                ManifestDownloadRequiredBlock RequiredBlock = new ManifestDownloadRequiredBlock {
                                    ManifestId = State.ManifestId,
                                    RangeStart = range.Start,
                                    RangeEnd = range.End
                                };

                                DownloadQueue.ToDownloadBlocks.Add(RequiredBlock);
                            }
                        }
                    }
                }
            }

            // Sort by priority.
            ManifestQueues.Sort((Item1, Item2) => -Item1.HighestPriority.CompareTo(Item2.HighestPriority));

            // Now to generate the actual download priority queue.
            // To do this we go through each manifest in priority order. The higher the priority the more blocks they get to add to the queue.
            // Keep going until queue is at max size.
            DownloadQueue = new List<ManifestPendingDownloadBlock>();

            BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
            long TotalSize = 0;
            while (TotalSize < IdealDownloadQueueSizeBytes && DownloadQueue.Count < MaxDownloadQueueItems)
            {
                bool BlocksAdded = false;

                foreach (ManifestDownloadQueue Queue in ManifestQueues)
                {
                    int ChunksToAdd = Math.Max(Queue.HighestPriority, 1);
                    for (int i = 0; i < ChunksToAdd && Queue.ToDownloadBlocks.Count > 0; i++)
                    {
                        ManifestDownloadRequiredBlock Block = Queue.ToDownloadBlocks[0];

                        DownloadQueue.Add(new ManifestPendingDownloadBlock { BlockIndex = Block.RangeStart, ManifestId = Queue.ManifestId });
                        BlocksAdded = true;

                        bool Result = GetBlockInfo(Queue.ManifestId, Block.RangeStart, ref BlockInfo);
                        Debug.Assert(Result == true);

                        TotalSize += BlockInfo.TotalSize;

                        Block.RangeStart++;
                        if (Block.RangeStart > Block.RangeEnd)
                        {
                            Queue.ToDownloadBlocks.RemoveAt(0);
                        }
                    }
                }

                if (!BlocksAdded)
                {
                    break;
                }
            }
        }
    }
}
