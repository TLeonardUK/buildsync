/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this sofPtware must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

#define CHECKSUM_EACH_BLOCK
//#define SPARSE_CHECKSUM_EACH_BLOCK
//#define CONSTANT_REDOWNLOAD
//#define LARGE_DOWNLOAD_QUEUE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking;
using BuildSync.Core.Utils;
using BuildSync.Core.Storage;
using BuildSync.Core.Scripting;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// </summary>
    /// <param name="ManifestId"></param>
    public delegate void ManifestRequestedHandler(Guid ManifestId);

    /// <summary>
    /// </summary>
    /// <param name="ManifestId"></param>
    public delegate void DownloadErrorHandler(Guid ManifestId);

    /// <summary>
    /// </summary>
    public delegate void BlockAccessCompleteHandler(bool bSuccess);

    /// <summary>
    /// </summary>
    public struct ManifestDownloadRequiredBlock
    {
        public Guid ManifestId;
        public int RangeStart;
        public int RangeEnd;
        public long Size;
    }

    /// <summary>
    /// </summary>
    public class ManifestDownloadQueue
    {
        public int HighestPriority;
        public int SortOrder;
        public Guid ManifestId;
        public bool InUse;
        public List<ManifestDownloadRequiredBlock> ToDownloadBlocks;
    }

    /// <summary>
    /// </summary>
    public struct ManifestPendingDownloadBlock
    {
        public Guid ManifestId;
        public int BlockIndex;

        public ulong TimeStarted;
        public long Size;

        public bool Recieved;

        public override bool Equals(object x)
        {
            if (!(x is ManifestPendingDownloadBlock))
            {
                return false;
            }

            ManifestPendingDownloadBlock pair = (ManifestPendingDownloadBlock)x;

            return ManifestId == pair.ManifestId &&
                   BlockIndex == pair.BlockIndex;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + ManifestId.GetHashCode();
            hash = hash * 31 + BlockIndex.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    /// </summary>
    public enum ManifestBlockChangeType
    {
        Upload,
        Validate
    }

    /// <summary>
    /// </summary>
    public struct ManifestRecentBlockChange
    {
        public ulong Timestamp;

        public Guid ManifestId;
        public int BlockIndex;

        public ManifestBlockChangeType Type;
    }

    /// <summary>
    /// </summary>
    public class ManifestDownloadManager
    {
        /// <summary>
        /// </summary>
        public class BlockIOState
        {
            public int SubBlocksRemaining;
            public bool WasSuccess;
        }

        /// <summary>
        /// </summary>
        public event ManifestRequestedHandler OnManifestRequested;

        /// <summary>
        /// </summary>
        public event DownloadErrorHandler OnDownloadError;

        /// <summary>
        /// </summary>
        private ManifestDownloadStateCollection StateCollection = new ManifestDownloadStateCollection();

        /// <summary>
        /// </summary>
        private BuildManifestRegistry ManifestRegistry;

        /// <summary>
        /// </summary>
        public AsyncIOQueue IOQueue;

        /// <summary>
        /// </summary>
        private BlockListState AvailableBlocks;

        /// <summary>
        /// </summary>
        public List<ManifestPendingDownloadBlock> DownloadQueue { get; } = new List<ManifestPendingDownloadBlock>();

        /// <summary>
        /// </summary>
        public List<ManifestRecentBlockChange> RecentBlockChanges { get; } = new List<ManifestRecentBlockChange>();

        /// <summary>
        /// </summary>
        private const int RecentBlockChangeDuration = 1 * 1000;

        /// <summary>
        /// </summary>
        private StorageManager StorageManager = null;

        /// <summary>
        /// </summary>
        private const int ManifestRequestInterval = 30 * 1000;

        /// <summary>
        /// </summary>
#if LARGE_DOWNLOAD_QUEUE
        private const int IdealDownloadQueueSizeBytes = 1000 * 1024 * 1024;
#else
        private const int IdealDownloadQueueSizeBytes = 256 * 1024 * 1024;
#endif

        /// <summary>
        /// </summary>
#if LARGE_DOWNLOAD_QUEUE
        private const int MaxDownloadQueueItems = 1000;
#else
        private const int MaxDownloadQueueItems = 512;
#endif

        /// <summary>
        /// </summary>
        private bool InternalTrafficEnabled = true;

        public bool TrafficEnabled
        {
            get => InternalTrafficEnabled;
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
        /// </summary>
        public bool DownloadInitializationInProgress { get; private set; }

        /// <summary>
        /// </summary>
        public bool DownloadValidationInProgress { get; private set; }

        /// <summary>
        /// </summary>
        public bool DownloadInstallInProgress { get; private set; }

        /// <summary>
        /// </summary>
        public bool AreStatesDirty { get; set; }

        /// <summary>
        /// </summary>
        public bool SkipValidation { get; set; } = false;

        /// <summary>
        /// </summary>
        public bool SkipDiskAllocation { get; set; } = false;

        /// <summary>
        /// </summary>
        public bool AutoFixValidationErrors { get; set; } = true;

        /// <summary>
        /// </summary>
        private int Internal_StateDirtyCount;

        public int StateDirtyCount
        {
            get => Internal_StateDirtyCount;
            set
            {
                Internal_StateDirtyCount = value;
                AreStatesDirty = true;
            }
        }

        /// <summary>
        /// </summary>
        private int Internal_StateBlockDirtyCount;

        public int StateBlockDirtyCount
        {
            get => Internal_StateBlockDirtyCount;
            set
            {
                Internal_StateBlockDirtyCount = value;
                AreStatesDirty = true;
            }
        }

        /// <summary>
        /// </summary>
        public ManifestDownloadStateCollection States => StateCollection;

        /// <summary>
        /// </summary>
        private readonly List<Guid> BlockedDownloadManifestIds = new List<Guid>();

        /// <summary>
        /// </summary>
        private ulong SplitIndexLastUpdatedTimer;

        /// <summary>
        /// </summary>
        private int SplitIndex = -1;

        /// <summary>
        /// </summary>
        private const int SplitIndexUpdateInterval = 3 * 60 * 1000;

        /// <summary>
        /// </summary>
        private bool[] AvailableBlockQueue = new bool[0];

        /// <summary>
        /// </summary>
        private bool[] CurrentBlockQueue = new bool[0];

        /// <summary>
        /// </summary>
        private bool[] ToDownloadBlockQueue = new bool[0];

        /// <summary>
        /// </summary>
        private struct LastFileWriteTimeCacheEntry
        {
            public DateTime LastModified;
            public ulong LastCacheUpdate;
        }

        /// <summary>
        /// </summary>
        private readonly Dictionary<string, LastFileWriteTimeCacheEntry> LastFileWriteCache = new Dictionary<string, LastFileWriteTimeCacheEntry>();

        /// <summary>
        /// </summary>
        private const int MaxLastFileWriteCacheEntryDuration = 5 * 1000;

        /// <summary>
        /// </summary>
        private readonly List<ManifestDownloadQueue> ManifestQueues = new List<ManifestDownloadQueue>();

        /// <summary>
        /// 
        /// </summary>
        private bool StateWaitingForFinalize = false;

        /// <summary>
        /// 
        /// </summary>
        private List<int> ValidateFailedBlocks = new List<int>();

        /// <summary>
        /// 
        /// </summary>
        Dictionary<BuildManifestFileInfo, string> DeltaCopyFilesToCopy = new Dictionary<BuildManifestFileInfo, string>();

        /// <summary>
        /// 
        /// </summary>
        private bool InitializeFailed = false;

        /// <summary>
        /// 
        /// </summary>
        private bool InstallFailed = false;

        /// <summary>
        /// 
        /// </summary>
        private bool DeltaCopyFailed = false;

        /// <summary>
        /// </summary>
        public void RecordBlockChange(Guid ManifestId, int BlockIndex, ManifestBlockChangeType Type)
        {
            lock (RecentBlockChanges)
            {
                ManifestRecentBlockChange Change;
                Change.ManifestId = ManifestId;
                Change.BlockIndex = BlockIndex;
                Change.Timestamp = TimeUtils.Ticks;
                Change.Type = Type;

                RecentBlockChanges.Add(Change);
            }
        }

        /// <summary>
        /// </summary>
        public void PruneBlockChanges()
        {
            ulong CurrentTime = TimeUtils.Ticks;
            lock (RecentBlockChanges)
            {
                for (int i = 0; i < RecentBlockChanges.Count; i++)
                {
                    ManifestRecentBlockChange Change = RecentBlockChanges[i];
                    if (CurrentTime - Change.Timestamp >= RecentBlockChangeDuration)
                    {
                        RecentBlockChanges.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
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
                ManifestState.IsActive = Downloader.State == ManifestDownloadProgressState.Downloading && !Downloader.Paused;
                ManifestState.BlockState = Downloader.BlockStates;

                Result.States[i] = ManifestState;
            }

            return Result;
        }

        /// <summary>
        /// </summary>
        /// <param name="ManifestDownloader"></param>
        /// <param name="Config"></param>
        public void Start(StorageManager InStorageManager, ManifestDownloadStateCollection ResumeStateCollection, BuildManifestRegistry Registry, AsyncIOQueue InIOQueue)
        {
            StorageManager = InStorageManager;
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
                    StateBlockDirtyCount++;

                    State.State = ManifestDownloadProgressState.RetrievingManifest;
                }
            }

            StateDirtyCount++;
        }

        /// <summary>
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
        /// </summary>
        /// <param name="ManifestId"></param>
        public bool IsDownloadBlocked(Guid ManifestId)
        {
            lock (BlockedDownloadManifestIds)
            {
                return BlockedDownloadManifestIds.Contains(ManifestId);
            }
        }

        /// <summary>
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
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Priority"></param>
        public ManifestDownloadState AddLocalDownload(BuildManifest Manifest, string StoragePath, bool Available = true)
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
            State.State = ManifestDownloadProgressState.Complete;
            State.LocalFolder = StoragePath;
            State.LastActive = DateTime.Now;

            // We have everything.
            State.BlockStates.Resize((int) Manifest.BlockCount);
            State.BlockStates.SetAll(Available);

            StoreFileCompletedStates(State);

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Added local download of manifest: {0}", Manifest.Guid.ToString());
            StateCollection.States.Add(State);

            StateDirtyCount++;

            return State;
        }

        /// <summary>
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
            State.LocalFolder = "";
            State.LastActive = DateTime.Now;

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Started download of manifest: {0}", ManifestId.ToString());
            StateCollection.States.Add(State);

            StateDirtyCount++;

            return State;
        }

        /// <summary>
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
        /// <param name="DeviceName"></param>
        /// <param name="InstallLocation"></param>
        public bool PerformInstallation(Guid ManifestId, string DeviceName, string InstallLocation, ScriptBuildProgressDelegate Callback)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State != null)
            {
                PerformInstallation(State, DeviceName, InstallLocation, Callback);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <param name="DeviceName"></param>
        /// <param name="InstallLocation"></param>
        private void PerformInstallation(ManifestDownloadState State, string DeviceName, string InstallLocation, ScriptBuildProgressDelegate Callback)
        {
            if (DeviceName == "")
            {
                string Trimmed = "localhost";
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Installing on device {0}, to location {1}, build directory: {2}", Trimmed, InstallLocation, State.LocalFolder);
                PerformInstallationInternal(State, Trimmed, InstallLocation, Callback);
            }
            else
            {
                string[] Devices = DeviceName.Split(',');
                foreach (string Device in Devices)
                {
                    string Trimmed = Device.Trim();
                    if (Trimmed.Length > 0)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Installing on device {0}, to location {1}, build directory: {2}", Trimmed, InstallLocation, State.LocalFolder);
                        PerformInstallationInternal(State, Trimmed, InstallLocation, Callback);
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="State"></param>
        private void PerformInstallationInternal(ManifestDownloadState State, string DeviceName, string InstallLocation, ScriptBuildProgressDelegate Callback)
        {
            string ConfigFilePath = Path.Combine(State.LocalFolder, "buildsync.json");
            bool IsScript = false;
            if (!File.Exists(ConfigFilePath))
            {
                ConfigFilePath = Path.Combine(State.LocalFolder, "buildsync.cs");
                IsScript = true;

                if (!File.Exists(ConfigFilePath))
                {
                    return;
                }
            }

            BuildSettings Settings = null;
            if (!IsScript)
            {
                if (!SettingsBase.Load(ConfigFilePath, out Settings))
                {
                    throw new Exception("The included buildsync.json file could not be loaded, it may be malformed.");
                }
            }
            else
            {
                Settings = new BuildSettings();
                try
                {
                    Settings.ScriptSource = File.ReadAllText(ConfigFilePath);
                }
                catch (Exception Ex)
                {
                    throw new Exception(string.Format("Failed to read file '{0}' due to exception: {1}", ConfigFilePath, Ex.Message));
                }
            }

            List<BuildLaunchMode> Modes;
            try
            {
                Modes = Settings.Compile();

                // Add various internal variables to pass in bits of info.
                foreach (BuildLaunchMode Mode in Modes)
                {
                    Mode.AddStringVariable("INSTALL_DEVICE_NAME", DeviceName);
                    Mode.AddStringVariable("INSTALL_LOCATION", InstallLocation);
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
                if (!Mode.Install(State.LocalFolder, ref ErrorMessage, Callback))
                {
                    throw new Exception("Error encountered while installing:\n\n" + ErrorMessage);
                }
            }

            State.Installed = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="State"></param>
        private void StoreFileCompletedStates(ManifestDownloadState State)
        {
            // Store timestamps of files so we can determine if someone has modified the files after our validation.
            lock (State.FileCompletedStates)
            {
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
        }

        /// <summary>
        /// </summary>
        /// <param name="State"></param>
        private void ClearFileCompletedStates(ManifestDownloadState State)
        {
            State.FileCompletedStates.Clear();
            if (State.LocalFolder != "")
            {
                ClearFileWriteCache(State.LocalFolder);
            }
        }

        /// <summary>
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
                        State.BlockStates.Resize((int) State.Manifest.BlockCount);

                        StateBlockDirtyCount++;
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
                    if (State.LocalFolder == "")
                    {
                        string StorageFolder = State.LocalFolder;
                        if (StorageManager.AllocateSpace(State.Manifest.GetTotalSize(), State.ManifestId, out StorageFolder))
                        {
                            State.LocalFolder = StorageFolder;
                        }
                        else
                        {
                            Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to allocate storage folder for build, {0}, storage directories likely out of capacity.", State.ManifestId.ToString());
                                
                            ChangeState(State, ManifestDownloadProgressState.DiskError, true);
                        }
                    }
                    else if (StateWaitingForFinalize)
                    {
                        if (InitializeFailed)
                        {
                            ChangeState(State, ManifestDownloadProgressState.InitializeFailed, true);
                        }
                        else
                        {
                            ChangeState(State, ManifestDownloadProgressState.DeltaCopying);
                        }
                    }
                    else if (State.InitializeTask == null)
                    {
                        InitializeFailed = false;
                        State.InitializeTask = Task.Run(
                            () =>
                            {
                                IOQueue.CloseAllStreamsInDirectory(State.LocalFolder);

                                try
                                {
                                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Initializing directory: {0}", State.LocalFolder);
                                    State.InitializeRateStats.Reset();
                                    State.Manifest.InitializeDirectory(
                                        State.LocalFolder, IOQueue, !SkipDiskAllocation, (BytesWritten, TotalBytes) =>
                                        {
                                            State.InitializeProgress = BytesWritten / (float) TotalBytes;
                                            State.InitializeRateStats.In(BytesWritten - State.InitializeRateStats.TotalIn);
                                            State.InitializeBytesRemaining = TotalBytes - BytesWritten;
                                            return !State.Paused;
                                        }
                                    );
                                }
                                catch (Exception Ex)
                                {
                                    Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to intialize directory with error: {0}", Ex.Message);
                                    InitializeFailed = true;
                                }
                                finally
                                {
                                    State.InitializeTask = null; 
                                    StateWaitingForFinalize = true;
                                }
                            }
                        );
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

                // Finding any manifests with files containing the same checksum. If found, copy over and mark blocks as available.
                case ManifestDownloadProgressState.DeltaCopying:
                {
                    if (StateWaitingForFinalize)
                    {
                        if (!DeltaCopyFailed)
                        {
                            // All blocks that purely contain these files should be marked as downloaded.
                            long TotalBytesSaved = 0;

                            foreach (var Pair in DeltaCopyFilesToCopy)
                            {
                                BuildManifestFileInfo DstInfo = Pair.Key;
                                for (int i = DstInfo.FirstBlockIndex; i <= DstInfo.LastBlockIndex; i++)
                                {
                                    BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
                                    if (State.Manifest.GetBlockInfo(i, ref BlockInfo))
                                    {
                                        bool AllSubBlocksAreThisFile = true;
                                        foreach (BuildManifestSubBlockInfo SubBlockInfo in BlockInfo.SubBlocks)
                                        {
                                            if (SubBlockInfo.File != DstInfo)
                                            {
                                                AllSubBlocksAreThisFile = false;
                                                break;
                                            }
                                        }

                                        if (AllSubBlocksAreThisFile)
                                        {
                                            State.BlockStates.Set(i, true);
                                            TotalBytesSaved += BlockInfo.TotalSize;
                                        }
                                    }
                                }
                            }
                                            
                            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Delta copying saved downloading {0}.", StringUtils.FormatAsSize(TotalBytesSaved));
                                            
                            StateDirtyCount++;

                            // Start downloading.
                            ChangeState(State, ManifestDownloadProgressState.Downloading);
                        }
                        else
                        {                                
                            ChangeState(State, ManifestDownloadProgressState.InitializeFailed, true);
                        }
                    }
                    else if (State.DeltaCopyTask == null)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Finding existing files: {0}", State.LocalFolder);

                        Dictionary<string, string> ExistingFiles = new Dictionary<string, string>();
                        Dictionary<string, string> WantedFiles = new Dictionary<string, string>();

                        foreach (BuildManifestFileInfo File in State.Manifest.Files)
                        {
                            if (File.Checksum.Length > 32) // Make sure we aren't using CRC for this.
                            {
                                WantedFiles.Add(File.Path, "");
                            }
                        }

                        foreach (ManifestDownloadState OtherState in StateCollection.States)
                        {
                            if (OtherState.Manifest != null && State != OtherState && OtherState.State == ManifestDownloadProgressState.Complete)
                            {
                                foreach (BuildManifestFileInfo File in OtherState.Manifest.Files)
                                {
                                    if (File.Checksum.Length > 32 && !ExistingFiles.ContainsKey(File.Checksum) && WantedFiles.ContainsKey(File.Path)) // Make sure we aren't using CRC for this.
                                    {
                                        string LocalFile = Path.Combine(OtherState.LocalFolder, File.Path);
                                        bool IsValid = false;

                                        if (System.IO.File.Exists(LocalFile))
                                        {
                                            // Ensure file has not been changed.
                                            lock (OtherState.FileCompletedStates)
                                            {
                                                foreach (ManifestFileCompletedState CompletedState in OtherState.FileCompletedStates)
                                                {
                                                    if (File.Path == CompletedState.Path)
                                                    {
                                                        if (GetFileLastWriteTime(LocalFile) == CompletedState.ModifiedTimestampOnCompleted)
                                                        {
                                                            IsValid = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (IsValid)
                                        {
                                            ExistingFiles.Add(File.Checksum, LocalFile);
                                        }
                                    }
                                }
                            }
                        }

                        DeltaCopyFilesToCopy = new Dictionary<BuildManifestFileInfo, string>();
                        long TotalSize = 0;
                            
                        foreach (BuildManifestFileInfo File in State.Manifest.Files)
                        {
                            if (File.Checksum.Length > 32 && ExistingFiles.ContainsKey(File.Checksum)) // Make sure we aren't using CRC for this.
                            {
                                DeltaCopyFilesToCopy.Add(File, ExistingFiles[File.Checksum]);
                                TotalSize += File.Size;
                            }
                        }
                            
                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Found {0} pre-existing files.", DeltaCopyFilesToCopy.Count);

                        State.DeltaCopyTask = Task.Run(
                            () =>
                            {
                                IOQueue.CloseAllStreamsInDirectory(State.LocalFolder);

                                DeltaCopyFailed = false;

                                try
                                {
                                    long BytesCopied = 0;
                                    foreach (var Pair in DeltaCopyFilesToCopy)
                                    {
                                        if (State.Paused)
                                        {
                                            break;   
                                        }

                                        string Src = Pair.Value;
                                        BuildManifestFileInfo DstInfo = Pair.Key;
                                        string Dst = Path.Combine(State.LocalFolder, DstInfo.Path);

                                        Logger.Log(LogLevel.Info, LogCategory.Manifest, "Copying existing files: {0} -> {1}", Src, Dst);

                                        long FileLength = (new FileInfo(Dst)).Length;
                                        long BytesCopiedStart = BytesCopied;

                                        string Dir = Path.GetDirectoryName(Dst);
                                        if (!Directory.Exists(Dir))
                                        {
                                            Directory.CreateDirectory(Dir);
                                        }

                                        FileCopyEx.Copy(Src, Dst, true, true, (o, pce) =>
                                        {
                                            BytesCopied = BytesCopiedStart + (long)(FileLength * (pce.ProgressPercentage / 100.0f));

                                            State.DeltaCopyProgress = BytesCopied / (float)TotalSize;
                                            State.DeltaCopyRateStats.In(BytesCopied - State.DeltaCopyRateStats.TotalIn);
                                            State.DeltaCopyBytesRemaining = TotalSize - BytesCopied;
                                        });

                                        BytesCopied = BytesCopiedStart + FileLength;

                                        State.DeltaCopyProgress = BytesCopied / (float)TotalSize;
                                        State.DeltaCopyRateStats.In(BytesCopied - State.DeltaCopyRateStats.TotalIn);
                                        State.DeltaCopyBytesRemaining = TotalSize - BytesCopied;
                                    }

                                    State.DeltaCopyRateStats.Reset();
                                }
                                catch (Exception Ex)
                                {
                                    DeltaCopyFailed = true;
                                    Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to delta copy with error: {0}", Ex.Message);
                                }
                                finally
                                {
                                    State.DeltaCopyTask = null;
                                    StateWaitingForFinalize = true;
                                }
                            }
                        );
                    }

                    break;
                }

                // Installing all launch modes on device.
                case ManifestDownloadProgressState.Installing:
                {
                    if (StateWaitingForFinalize)
                    {
                        if (InstallFailed)
                        {
                            ChangeState(State, ManifestDownloadProgressState.InstallFailed, true);
                        }
                        else
                        {
                            ChangeState(State, ManifestDownloadProgressState.Complete);
                        }
                    }
                    else if (State.InstallTask == null)
                    {
                        State.InstallTask = Task.Run(
                            () =>
                            {
                                try
                                {
                                    State.InstallProgress = -1.0f;
                                    ScriptBuildProgressDelegate Callback = (string InState, float InProgress) =>
                                    {
                                        State.InstallProgress = InProgress;
                                    };

                                    PerformInstallation(State, State.InstallDeviceName, State.InstallLocation, Callback);
                                }
                                catch (Exception Ex)
                                {
                                    Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to install with error: {0}", Ex.Message);
                                    InstallFailed = true;
                                }
                                finally
                                {
                                    State.InstallTask = null;
                                    StateWaitingForFinalize = true;
                                }
                            }
                        );
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
                    if (State.DiskError)
                    {
                        ChangeState(State, ManifestDownloadProgressState.DiskError, true);
                    }

                    if (State.BlockStates.AreAllSet(true))
                    {
#if CONSTANT_REDOWNLOAD
                        State.BlockStates.SetAll(false);
#else
                        if (SkipValidation)
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
                            ChangeState(State, ManifestDownloadProgressState.Validating);
                        }
#endif
                    }

                    break;
                }

                // Restarting from an download disk error, try again.
                case ManifestDownloadProgressState.DiskError:
                {
                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Retrying download after resume from error.");
                    ChangeState(State, ManifestDownloadProgressState.Downloading);
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
                    if (StateWaitingForFinalize)
                    {
                        if (ValidateFailedBlocks.Count == 0)
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
                            foreach (int Block in ValidateFailedBlocks)
                            {
                                MarkBlockAsUnavailable(State.ManifestId, Block);
                                //MarkFileAsUnavailable(State.ManifestId, File);
                            }

                            if (AutoFixValidationErrors)
                            {
                                ChangeState(State, ManifestDownloadProgressState.Downloading);
                            }
                            else
                            {
                                ChangeState(State, ManifestDownloadProgressState.ValidationFailed, true);
                            }
                        }
                    }
                    else if (State.ValidationTask == null)
                    {
                        // Close all async io streams to the download we are working on.
                        State.ValidationTask = Task.Run(
                            () =>
                            {
                                IOQueue.CloseAllStreamsInDirectory(State.LocalFolder);

                                ValidateFailedBlocks.Clear();

                                try
                                {
                                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Validating directory: {0}", State.LocalFolder);
                                    State.ValidateRateStats.Reset();
                                    ValidateFailedBlocks = State.Manifest.Validate(
                                        State.LocalFolder, State.ValidateRateStats, IOQueue, (BytesRead, TotalBytes, ManifestId, BlockIndex) =>
                                        {
                                            State.ValidateProgress = BytesRead / (float) TotalBytes;
                                            State.ValidateRateStats.Out(BytesRead - State.ValidateRateStats.TotalOut);
                                            State.ValidateBytesRemaining = TotalBytes - BytesRead;
                                            RecordBlockChange(ManifestId, BlockIndex, ManifestBlockChangeType.Validate);
                                            return !State.Paused;
                                        }
                                    );
                                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Validation finished, {0} blocks failed.", ValidateFailedBlocks.Count);
                                }
                                catch (Exception Ex)
                                {
                                    Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to validate directory with error: {0}", Ex.Message);

                                    for (int i = 0; i < State.Manifest.BlockCount; i++)
                                    {
                                        ValidateFailedBlocks.Add(i);
                                    }
                                }
                                finally
                                {
                                    State.ValidationTask = null;
                                    StateWaitingForFinalize = true;
                                }
                            }
                        );
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

        /// <summary>
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
                foreach (KeyValuePair<string, LastFileWriteTimeCacheEntry> pair in LastFileWriteCache)
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
        /// </summary>
        /// <param name="NewState"></param>
        private void ChangeState(ManifestDownloadState State, ManifestDownloadProgressState NewState, bool IsError = false)
        {
            State.State = NewState;
            State.DiskError = false;
            StateWaitingForFinalize = false;
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

            PruneBlockChanges();
        }

        /// <summary>
        /// </summary>
        /// <param name="NewDirectory"></param>
        public void UpdateStoragePath(List<string> OldDirectories, List<string> NewDirectories)
        {
            List<string> NormalizedOldDirectories = new List<string>();
            foreach (string Value in OldDirectories)
            {
                NormalizedOldDirectories.Add(FileUtils.NormalizePath(Value));
            }

            foreach (ManifestDownloadState State in StateCollection.States)
            {
                int Index = NormalizedOldDirectories.IndexOf(FileUtils.NormalizePath(State.LocalFolder));
                if (Index >= 0)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Updating storage path of '{0}' to '{1}'", State.LocalFolder, NewDirectories[Index]);

                    State.LocalFolder = NewDirectories[Index];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        public void PruneManifest(Guid Id)
        {
            foreach (ManifestDownloadState State in StateCollection.States)
            {
                if (State.ManifestId == Id)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Deleting download to prune storage space: {0}", State.ManifestId.ToString());

                    // Remove state from our state collection.
                    StateCollection.States.Remove(State);
                    StateDirtyCount++;

                    // Ask IO queue to delete the folder.
                    IOQueue.DeleteDir(State.LocalFolder, null);

                    // Remove the manifest from the registry.
                    ManifestRegistry.UnregisterManifest(State.ManifestId);

                    break;
                }
            }
        }

        /// <summary>
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
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="BlockIndex"></param>
        /// <param name="Data"></param>
        public bool GetBlockData(Guid ManifestId, int BlockIndex, ref NetCachedArray Data, BlockAccessCompleteHandler Callback, out bool FailedOutOfMemory)
        {
            FailedOutOfMemory = false;

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
            WriteState.SubBlocksRemaining = BlockInfo.SubBlocks.Length;
            WriteState.WasSuccess = true;

            if (!Data.Resize((int) BlockInfo.TotalSize, (int)State.Manifest.BlockSize, true))
            {
                FailedOutOfMemory = true;
                return false;
            }

            byte[] DataBuffer = Data.Data;

            for (int i = 0; i < BlockInfo.SubBlocks.Length; i++)
            {
                BuildManifestSubBlockInfo SubBlockInfo = BlockInfo.SubBlocks[i];

                string LocalFile = Path.Combine(State.LocalFolder, SubBlockInfo.File.Path);
                IOQueue.Read(
                    LocalFile, SubBlockInfo.FileOffset, SubBlockInfo.FileSize, Data.Data, SubBlockInfo.OffsetInBlock, bSuccess =>
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
                        lock (State.FileCompletedStates)
                        {
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
                        }

                        if (Interlocked.Decrement(ref WriteState.SubBlocksRemaining) == 0)
                        {
                            // Checksum block to make sure nobody has balls it up.
#if CHECKSUM_EACH_BLOCK
                            if (State.Manifest.BlockChecksums != null)
                            {                            
                                uint Checksum = 0;
                                if (State.Manifest.Version >= 2)
                                {
                                    Checksum = Crc32Fast.Compute(DataBuffer, 0, (int)BlockInfo.TotalSize);
                                }
                                else
                                {
                                    Checksum = Crc32Slow.Compute(DataBuffer, (int)BlockInfo.TotalSize);
                                }

                                //uint Checksum = Crc32.Compute(DataBuffer, (int)BlockInfo.TotalSize);
                                uint ExpectedChecksum = State.Manifest.BlockChecksums[BlockIndex];
                                if (Checksum != ExpectedChecksum)
                                {
                                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Block index {0} in manifest {1} failed checksum (got {2} expected {3}), failed to get block data.", BlockIndex, ManifestId.ToString(), Checksum, ExpectedChecksum);
                                    WriteState.WasSuccess = false;
                                }
                            }
#elif SPARSE_CHECKSUM_EACH_BLOCK
                            if (State.Manifest.SparseBlockChecksums != null)
                            {
                                uint Checksum = Crc32Slow.ComputeSparse(DataBuffer, (int)BlockInfo.TotalSize);
                                uint ExpectedChecksum = State.Manifest.SparseBlockChecksums[BlockIndex];
                                if (Checksum != ExpectedChecksum)
                                {
                                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Block index {0} in manifest {1} failed checksum (got {2} expected {3}), failed to get block data.", BlockIndex, ManifestId.ToString(), Checksum, ExpectedChecksum);
                                    WriteState.WasSuccess = false;
                                }
                                else
                                {
                                //    Logger.Log(LogLevel.Info, LogCategory.Manifest, "GOOD READ: {0} in manifest {1}", BlockIndex, ManifestId.ToString());
                                }
                            }
#endif

                            RecordBlockChange(ManifestId, BlockIndex, ManifestBlockChangeType.Upload);

                            Callback?.Invoke(WriteState.WasSuccess);
                        }
                    }
                );
            }

            return true;
        }

        /// <summary>
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
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to set block data for block we already have, block {0} in {1}.", BlockIndex, ManifestId.ToString());
                Callback?.Invoke(false);
                return false;
            }

            BlockIOState WriteState = new BlockIOState();
            WriteState.SubBlocksRemaining = BlockInfo.SubBlocks.Length;
            WriteState.WasSuccess = true;

#if CHECKSUM_EACH_BLOCK
            if (State.Manifest.BlockChecksums != null)
            {                                        
                uint Checksum = 0;
                if (State.Manifest.Version >= 2)
                {
                    Checksum = Crc32Fast.Compute(Data.Data, 0, (int)Data.Length);
                }
                else
                {
                    Checksum = Crc32Slow.Compute(Data.Data, (int)Data.Length);
                }

                //uint Checksum = Crc32.Compute(Data.Data, (int)Data.Length);
                if (Checksum != State.Manifest.BlockChecksums[BlockIndex])
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Manifest, "FAILED: Block index {0} in manifest {1} failed checksum (got {2} expected {3}), failed to set block data.", BlockIndex, ManifestId.ToString(), State.Manifest.BlockChecksums[BlockIndex], Checksum);
                    WriteState.WasSuccess = false;
                    Callback?.Invoke(WriteState.WasSuccess);
                    return false;
                }
            }
#elif SPARSE_CHECKSUM_EACH_BLOCK
            if (State.Manifest.SparseBlockChecksums != null)
            {
                uint Checksum = Crc32Slow.ComputeSparse(Data.Data, (int)Data.Length);
                if (Checksum != State.Manifest.SparseBlockChecksums[BlockIndex])
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Manifest, "Block index {0} in manifest {1} failed checksum (got {2} expected {3}), failed to set block data.", BlockIndex, ManifestId.ToString(), State.Manifest.SparseBlockChecksums[BlockIndex], Checksum);
                    WriteState.WasSuccess = false;
                    Callback?.Invoke(WriteState.WasSuccess);
                    return false;
                }
                else
                {
                //    Logger.Log(LogLevel.Info, LogCategory.Manifest, "GOOD WRITE: {0} in manifest {1}", BlockIndex, ManifestId.ToString());
                }
            }
#endif

            for (int i = 0; i < BlockInfo.SubBlocks.Length; i++)
            {
                BuildManifestSubBlockInfo SubBlockInfo = BlockInfo.SubBlocks[i];

                string LocalFile = Path.Combine(State.LocalFolder, SubBlockInfo.File.Path);
                IOQueue.Write(
                    LocalFile, SubBlockInfo.FileOffset, SubBlockInfo.FileSize, Data.Data, SubBlockInfo.OffsetInBlock, bSuccess =>
                    {
                        if (bSuccess)
                        {
                            State.BandwidthStats.In(SubBlockInfo.FileSize);
                        }
                        else
                        {
                            State.DiskError = true;
                            WriteState.WasSuccess = false;
                        }

                        if (Interlocked.Decrement(ref WriteState.SubBlocksRemaining) == 0)
                        {
                            Callback?.Invoke(WriteState.WasSuccess);
                        }
                    }
                );
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="ManifestId"></param>
        public void MarkBlockAsComplete(Guid ManifestId, int BlockIndex, bool OnlyIfDownloading = false)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to set block from manifest we do not have.");
                return;
            }

            if (State.State != ManifestDownloadProgressState.Downloading && OnlyIfDownloading)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Attempting to mark manifest block as complete, but manifest is not in a download state.");
                return;
            }

            BuildManifestBlockInfo BlockInfo = new BuildManifestBlockInfo();
            if (!State.Manifest.GetBlockInfo(BlockIndex, ref BlockInfo))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Request to set block which we don't have info for.");
                return;
            }

            // Keep track of when this manifest last recieved data.
            State.LastRecievedData = DateTime.UtcNow;

            // Mark block as having block.
            State.BlockStates.Set(BlockIndex, true);
            StateBlockDirtyCount++;
        }

        /// <summary>
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

            StateBlockDirtyCount++;

            if (State.State == ManifestDownloadProgressState.Complete && BlockIndices.Count > 0)
            {
                ChangeState(State, ManifestDownloadProgressState.Downloading);
            }
        }

        /// <summary>
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
                    StateBlockDirtyCount++;
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
            StateBlockDirtyCount++;

            if (State.State == ManifestDownloadProgressState.Complete && !State.BlockStates.AreAllSet(true))
            {
                ChangeState(State, ManifestDownloadProgressState.Downloading);
            }
        }

        /// <summary>
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
                if (State != null && State.Manifest == null)
                {
                    State.BlockStates.SetAll(false);
                    StateBlockDirtyCount++;
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

            StateBlockDirtyCount++;

            if (State.State == ManifestDownloadProgressState.Complete && !State.BlockStates.AreAllSet(true))
            {
                ChangeState(State, ManifestDownloadProgressState.Downloading);
            }
        }

        /// <summary>
        /// </summary>
        public void SetAvailableToDownloadBlocks(BlockListState InAvailableBlocks)
        {
            AvailableBlocks = InAvailableBlocks;
        }

        /// <summary>
        /// </summary>
        public void UpdateBlockQueue()
        {
            if (AvailableBlocks == null || !TrafficEnabled)
            {
                return;
            }

            Random Randomiser = new Random(Environment.TickCount);

            // Update split index periodically.
            if (SplitIndex == -1 || TimeUtils.Ticks - SplitIndexLastUpdatedTimer > SplitIndexUpdateInterval)
            {
                SplitIndex = Randomiser.Next();
                SplitIndexLastUpdatedTimer = TimeUtils.Ticks;
            }

            // Create queues of blocks that we want.
            foreach (ManifestDownloadQueue Queue in ManifestQueues)
            {
                Queue.ToDownloadBlocks.Clear();
                Queue.InUse = false;
            }

            foreach (ManifestDownloadState State in StateCollection.States)
            {
                if (State.State == ManifestDownloadProgressState.Downloading && !State.Paused)
                {
                    // Create queue for this manifest if it doesn't exist.
                    ManifestDownloadQueue DownloadQueue = null;
                    foreach (ManifestDownloadQueue Queue in ManifestQueues)
                    {
                        if (Queue.ManifestId == State.ManifestId)
                        {
                            DownloadQueue = Queue;
                            break;
                        }
                    }

                    if (DownloadQueue == null)
                    {
                        DownloadQueue = new ManifestDownloadQueue();
                        DownloadQueue.ToDownloadBlocks = new List<ManifestDownloadRequiredBlock>();
                        ManifestQueues.Add(DownloadQueue);
                    }

                    DownloadQueue.HighestPriority = State.Priority;
                    DownloadQueue.SortOrder = (State.Priority * 1000) + (Randomiser.Next() % 100);
                    DownloadQueue.ManifestId = State.ManifestId;
                    DownloadQueue.InUse = true;

                    int BlockCount = 0;

                    // Grab a list of available blocks for this 
                    bool bFound = false;
                    foreach (ManifestBlockListState AState in AvailableBlocks.States)
                    {
                        if (AState.Id == State.ManifestId)
                        {
                            AState.BlockState.ToArray(ref AvailableBlockQueue, ref BlockCount);
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound || BlockCount != State.BlockStates.Size)
                    {
                        continue;
                    }

                    State.BlockStates.ToArray(ref CurrentBlockQueue, ref BlockCount);
                    if (ToDownloadBlockQueue.Length < BlockCount)
                    {
                        Array.Resize(ref ToDownloadBlockQueue, BlockCount);
                    }

                    // Make list of blocks that are available and blocks that we don't have. aka. our download list.
                    for (int i = 0; i < BlockCount; i++)
                    {
                        ToDownloadBlockQueue[i] = AvailableBlockQueue[i] && !CurrentBlockQueue[i];
                    }

                    // Generate the ranges to download.
                    SparseStateArray DownloadRanges = new SparseStateArray();
                    DownloadRanges.Size = 0;
                    DownloadRanges.Ranges = null;

                    // Split at a random place and download the second segment first. This speeds up the rate at which peers have blocks available to seed. And stops
                    // the first seeder being hammered.
                    int ThisSplitIndex = SplitIndex % BlockCount;
                    DownloadRanges.AddArray(ToDownloadBlockQueue, ThisSplitIndex, BlockCount - ThisSplitIndex);
                    DownloadRanges.AddArray(ToDownloadBlockQueue, 0, ThisSplitIndex);

                    if (DownloadRanges.Ranges != null)
                    {
                        foreach (SparseStateArray.Range range in DownloadRanges.Ranges)
                        {
                            if (range.State)
                            {
                                ManifestDownloadRequiredBlock RequiredBlock = new ManifestDownloadRequiredBlock
                                {
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

            // Remove unused queues.
            for (int i = 0; i < ManifestQueues.Count; i++)
            {
                if (!ManifestQueues[i].InUse || ManifestQueues[i].ToDownloadBlocks.Count == 0)
                {
                    ManifestQueues.RemoveAt(i);
                    i--;
                }
            }

            // Sort by priority.
            ManifestQueues.Sort((Item1, Item2) => -Item1.SortOrder.CompareTo(Item2.SortOrder));

            // Now to generate the actual download priority queue.
            // To do this we go through each manifest in priority order. The higher the priority the more blocks they get to add to the queue.
            // Keep going until queue is at max size.
            DownloadQueue.Clear();

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

                        DownloadQueue.Add(new ManifestPendingDownloadBlock {BlockIndex = Block.RangeStart, ManifestId = Queue.ManifestId});
                        BlocksAdded = true;

                        bool Result = GetBlockInfo(Queue.ManifestId, Block.RangeStart, ref BlockInfo);
                        Debug.Assert(Result);

                        TotalSize += BlockInfo.TotalSize;

                        Block.RangeStart++;
                        Queue.ToDownloadBlocks[0] = Block; // It's a struct so reassign.

                        if (TotalSize >= IdealDownloadQueueSizeBytes || DownloadQueue.Count >= MaxDownloadQueueItems)
                        {
                            break;
                        }

                        if (Block.RangeStart > Block.RangeEnd)
                        {
                            Queue.ToDownloadBlocks.RemoveAt(0);
                        }
                    }

                    if (TotalSize >= IdealDownloadQueueSizeBytes || DownloadQueue.Count >= MaxDownloadQueueItems)
                    {
                        break;
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