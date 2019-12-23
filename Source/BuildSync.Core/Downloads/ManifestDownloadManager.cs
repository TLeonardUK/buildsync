using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking;
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
        private Task InitializeTask = null;

        /// <summary>
        /// 
        /// </summary>
        private Task ValidationTask = null;

        /// <summary>
        /// 
        /// </summary>
        private string StorageRootPath = "";

        /// <summary>
        /// 
        /// </summary>
        private long StorageMaxSize = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int ManifestRequestInterval = 30 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private const int IdealDownloadQueueSize = 100;

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
        public bool AreStatesDirty
        {
            get;
            set;
        }

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
        public BlockListState GetBlockListState()
        {
            BlockListState Result = new BlockListState();
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
            }

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
        public string GetManifestStorageDirectory(BuildManifest Manifest)
        {
            return Path.Combine(StorageRootPath, Manifest.Guid.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Priority"></param>
        public ManifestDownloadState AddLocalDownload(BuildManifest Manifest, string LocalPath)
        {
            ManifestDownloadState State = GetDownload(Manifest.Guid);
            if (State != null)
            {
                return State;
            }

            ManifestRegistry.RegisterManifest(Manifest);

            State = new ManifestDownloadState();
            State.Id = Guid.NewGuid();
            State.ManifestId = Manifest.Guid;
            State.Priority = 2;
            State.Manifest = Manifest;
            State.State = ManifestDownloadProgressState.Complete;
            State.LocalFolder = Path.Combine(StorageRootPath, State.ManifestId.ToString());

            // We have everything.
            State.BlockStates.Resize((int)Manifest.BlockCount);
            State.BlockStates.SetAll(true);

            Console.WriteLine("Added local download of manifest: {0}", Manifest.Guid.ToString());
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

            Console.WriteLine("Started download of manifest: {0}", ManifestId.ToString());
            StateCollection.States.Add(State);

            StateDirtyCount++;

            return State;
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

            Console.WriteLine("Paused download of manifest: {0}", ManifestId.ToString());
            State.Paused = true;

            StateDirtyCount++;
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

            Console.WriteLine("Resumed download of manifest: {0}", ManifestId.ToString());
            State.Paused = false;

            StateDirtyCount++;
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
                            Console.WriteLine("Retrieved manifest for download '{0}', starting download.", State.Id);
                            ChangeState(State, ManifestDownloadProgressState.Initializing);
                            State.BlockStates.Resize((int)State.Manifest.BlockCount);

                            StateDirtyCount++;
                        }
                        else
                        {
                            // Request manifest from server.
                            int Elapsed = Environment.TickCount - State.LastManifestRequestTime;
                            if (State.LastManifestRequestTime == 0 || Elapsed > ManifestRequestInterval)
                            {
                                OnManifestRequested?.Invoke(State.ManifestId);
                                State.LastManifestRequestTime = Environment.TickCount;
                            }
                        }

                        break;
                    }

                // Create all the files in the directory.
                case ManifestDownloadProgressState.Initializing:
                    {
                        if (InitializeTask == null)
                        {
                            InitializeTask = Task.Run(() =>
                            {
                                try
                                {
                                    System.Console.WriteLine("Initializing directory: {0}", State.LocalFolder);
                                    State.Manifest.InitializeDirectory(State.LocalFolder);
                                    ChangeState(State, ManifestDownloadProgressState.Downloading);
                                }
                                catch (Exception Ex)
                                {
                                    ChangeState(State, ManifestDownloadProgressState.InitializeFailed, true);
                                    System.Console.WriteLine("Failed to intialize directory with error: {0}", Ex.Message);
                                }
                                finally
                                {
                                    InitializeTask = null;
                                }
                            });
                        }

                        break;
                    }

                // Restarting from an initialization failure, try again.
                case ManifestDownloadProgressState.InitializeFailed:
                    {
                        System.Console.WriteLine("Retrying initialization after resume from error.");
                        ChangeState(State, ManifestDownloadProgressState.Initializing);
                        break;
                    }

                // Get downloading them blocks.
                case ManifestDownloadProgressState.Downloading:
                    {
                        if (State.BlockStates.AreAllSet(true))
                        {
                            ChangeState(State, ManifestDownloadProgressState.Validating);
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
                        if (ValidationTask == null)
                        {
                            // Close all async io streams to the download we are working on.
                            if (IOQueue.CloseAllStreamsInDirectory(State.LocalFolder))
                            {
                                ValidationTask = Task.Run(() =>
                                {
                                    try
                                    {
                                        System.Console.WriteLine("Validating directory: {0}", State.LocalFolder);
                                        List<string> FailedFiles = State.Manifest.Validate(State.LocalFolder);
                                        if (FailedFiles.Count == 0)
                                        {
                                            ChangeState(State, ManifestDownloadProgressState.Complete);
                                        }
                                        else
                                        {
                                            foreach (string File in FailedFiles)
                                            {
                                                MarkFileAsUnavailable(State.ManifestId, File);
                                            }

                                            ChangeState(State, ManifestDownloadProgressState.ValidationFailed, true);
                                        }
                                    }
                                    catch (Exception Ex)
                                    {
                                        ChangeState(State, ManifestDownloadProgressState.ValidationFailed, true);
                                        System.Console.WriteLine("Failed to validate directory with error: {0}", Ex.Message);
                                    }
                                    finally
                                    {
                                        ValidationTask = null;
                                    }
                                });
                            }
                        }

                        break;
                    }

                // Restarting from an validation failure, try again.
                case ManifestDownloadProgressState.ValidationFailed:
                    {
                        System.Console.WriteLine("Retrying downloading after resume from validation error.");
                        ChangeState(State, ManifestDownloadProgressState.Downloading);
                        break;
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
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            bool AnyValidating = false;
            bool AnyInitialising = false;

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
            }

            DownloadInitializationInProgress = AnyInitialising;
            DownloadValidationInProgress = AnyValidating;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="BlockIndex"></param>
        /// <param name="Data"></param>
        public bool GetBlockData(Guid ManifestId, int BlockIndex, ref byte[] Data, BlockAccessCompleteHandler Callback)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                Console.WriteLine("Request for block from manifest we do not have.");
                return false;
            }

            long BlockOffset = 0;
            long BlockSize = 0;
            BuildManifestFileInfo FileInfo = null;

            if (!State.Manifest.GetBlockInfo(BlockIndex, ref FileInfo, ref BlockOffset, ref BlockSize))
            {
                Console.WriteLine("Request for invalid block info.");
                return false;
            }

            // Ensure we have block.
            if (!State.BlockStates.Get(BlockIndex))
            {
                Console.WriteLine("Request for block data for block we do not have.");
                return false;
            }

            // Ensure the file exists locally.
            string LocalFile = Path.Combine(State.LocalFolder, FileInfo.Path);
            Data = new byte[BlockSize];
            IOQueue.Read(LocalFile, BlockOffset, BlockSize, Data, (bool bSuccess) =>
            {
                if (bSuccess)
                {
                    State.BandwidthStats.BytesOut(BlockSize);
                }

                Callback?.Invoke(bSuccess);
            });

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="BlockIndex"></param>
        /// <param name="Data"></param>
        public bool SetBlockData(Guid ManifestId, int BlockIndex, byte[] Data, BlockAccessCompleteHandler Callback)
        {
            ManifestDownloadState State = GetDownload(ManifestId);
            if (State == null)
            {
                Console.WriteLine("Request to set block from manifest we do not have.");
                return false;
            }

            long BlockOffset = 0;
            long BlockSize = 0;
            BuildManifestFileInfo FileInfo = null;

            if (!State.Manifest.GetBlockInfo(BlockIndex, ref FileInfo, ref BlockOffset, ref BlockSize))
            {
                Console.WriteLine("Request to set block which we don't have info for.");
                return false;
            }

            // Ensure data is valid.
            if (Data.Length != BlockSize)
            {
                Console.WriteLine("Request to write block with smaller than expected data.");
                return false;
            }

            // Ensure we have block.
            if (State.BlockStates.Get(BlockIndex))
            {
                Console.WriteLine("Request to set block data for block we not have.");
                return false;
            }

            // Ensure the file exists locally.
            string LocalFile = Path.Combine(State.LocalFolder, FileInfo.Path);
            IOQueue.Write(LocalFile, BlockOffset, BlockSize, Data, (bool bSuccess) =>
            {
                if (bSuccess)
                {
                    State.BandwidthStats.BytesIn(Data.Length);
                }

                Callback?.Invoke(bSuccess);
            });

            return false;
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
                Console.WriteLine("Request to set block from manifest we do not have.");
                return;
            }

            long BlockOffset = 0;
            long BlockSize = 0;
            BuildManifestFileInfo FileInfo = null;

            if (!State.Manifest.GetBlockInfo(BlockIndex, ref FileInfo, ref BlockOffset, ref BlockSize))
            {
                Console.WriteLine("Request to set block which we don't have info for.");
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
                Console.WriteLine("Request to unset block from manifest we do not have.");
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
            if (State == null)
            {
                Console.WriteLine("Request to unset block from manifest we do not have.");
                return;
            }

            long BlockOffset = 0;
            long BlockSize = 0;
            BuildManifestFileInfo FileInfo = null;

            if (!State.Manifest.GetBlockInfo(BlockIndex, ref FileInfo, ref BlockOffset, ref BlockSize))
            {
                Console.WriteLine("Request to unset block which we don't have info for.");
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
        public void SetAvailableToDownloadBlocks(BlockListState InAvailableBlocks)
        {
            AvailableBlocks = InAvailableBlocks;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateBlockQueue()
        {
            if (AvailableBlocks == null)
            {
                return;
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

                    // Make list of blcoks that are available and blocks that we don't have. aka. our download list.
                    for (int i = 0; i < Available.Length; i++)
                    {
                        ToDownload[i] = Available[i] && !Current[i];
                    }

                    // Generate the ranges to download.
                    SparseStateArray DownloadRanges;
                    DownloadRanges.Size = 0;
                    DownloadRanges.Ranges = null;
                    DownloadRanges.FromArray(ToDownload);

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
            ManifestQueues.Sort((Item1, Item2) => Item1.HighestPriority.CompareTo(Item2.HighestPriority));

            // Now to generate the actual download priority queue.
            // To do this we go through each manifest in priority order. The higher the priority the more blocks they get to add to the queue.
            // Keep going until queue is at max size.
            DownloadQueue = new List<ManifestPendingDownloadBlock>();
            while (DownloadQueue.Count < IdealDownloadQueueSize)
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
