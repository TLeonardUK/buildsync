using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Manifests
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="CurrentFile"></param>
    /// <param name="OverallProgress"></param>
    public delegate void BuildManfiestProgressCallbackHandler(string CurrentFile, float OverallProgress);

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BuildManifestFileInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Path;

        /// <summary>
        /// 
        /// </summary>
        public long Size;

        /// <summary>
        /// 
        /// </summary>
        public string Checksum;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        internal int FirstBlockIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        internal int LastBlockIndex = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct BuildManifestSubBlockInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public BuildManifestFileInfo File;

        /// <summary>
        /// 
        /// </summary>
        public long FileOffset;

        /// <summary>
        /// 
        /// </summary>
        public long FileSize;

        /// <summary>
        /// 
        /// </summary>
        public long OffsetInBlock;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct BuildManifestBlockInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<BuildManifestSubBlockInfo> SubBlocks;

        /// <summary>
        /// 
        /// </summary>
        public long TotalSize;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BuildManifest
    {
        /// <summary>
        /// 
        /// </summary>
        public System.Guid Guid;

        /// <summary>
        /// 
        /// </summary>
        public string VirtualPath;

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime = DateTime.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        public static long BlockSize = 1 * 1024 * 1024;

        /// <summary>
        /// 
        /// </summary>
        public long BlockCount;

        /// <summary>
        /// 
        /// </summary>
        public List<BuildManifestFileInfo> Files = new List<BuildManifestFileInfo>();

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private long TotalSize = -1;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private BuildManifestBlockInfo[] BlockInfo = new BuildManifestBlockInfo[0];

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private Dictionary<string, BuildManifestFileInfo> FilesByPath = new Dictionary<string, BuildManifestFileInfo>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetTotalSize()
        {
            return TotalSize;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CacheBlockInfo()
        {
            // Store block information.
            BlockInfo = new BuildManifestBlockInfo[BlockCount];
            FilesByPath = new Dictionary<string, BuildManifestFileInfo>();

            // Try and add in optimal block packing format.
            //Console.WriteLine("======================================== CACHING BLOCK INFO ================================");
            long BlockIndex = 0;
            long TotalBlockSize = 0;
            for (int fi = 0; fi < Files.Count; )
            {
                BuildManifestFileInfo Info = Files[fi];
                Info.FirstBlockIndex = (int)BlockIndex;
                fi++;

                long BlockCount = (Info.Size + (BlockSize - 1)) / BlockSize;
                long BytesRemaining = BlockSize - (Info.Size % BlockSize);

                //Console.WriteLine("Block[{0}] {1}", BlockIndex, Info.Path);

                // Fill info for all the "full blocks" for this file.
                long Total = 0;
                for (int i = 0; i < BlockCount; i++)
                {
                    BuildManifestSubBlockInfo SubBlock;
                    SubBlock.File = Info;
                    SubBlock.FileOffset = i * BlockSize;
                    SubBlock.FileSize = Math.Min(BlockSize, Info.Size - SubBlock.FileOffset);
                    SubBlock.OffsetInBlock = 0;

                    Debug.Assert(SubBlock.FileOffset + SubBlock.FileSize <= Info.Size);

                    if (BlockInfo[BlockIndex].SubBlocks == null)
                    {
                        BlockInfo[BlockIndex].SubBlocks = new List<BuildManifestSubBlockInfo>();
                    }
                    BlockInfo[BlockIndex].SubBlocks.Add(SubBlock);
                    BlockInfo[BlockIndex].TotalSize += SubBlock.FileSize;

                    TotalBlockSize += SubBlock.FileSize;

                    Total += SubBlock.FileSize;

                    BlockIndex++;
                }
                Debug.Assert(Total == Info.Size);
                
                Info.LastBlockIndex = (int)BlockIndex -  1;

                int LastBlockIndex = Info.LastBlockIndex;

                // Fill remaining space with blocks.
                while (BytesRemaining > 0 && fi < Files.Count)
                {
                    BuildManifestFileInfo NextInfo = Files[fi];
                    if (NextInfo.Size > BytesRemaining)
                    {
                        break;
                    }
                    else
                    {
                        if (NextInfo.Size > 0)
                        {
                            BuildManifestSubBlockInfo SubBlock;
                            SubBlock.File = NextInfo;
                            SubBlock.FileOffset = 0;
                            SubBlock.FileSize = NextInfo.Size;
                            SubBlock.OffsetInBlock = BlockInfo[LastBlockIndex].TotalSize;
                            BlockInfo[LastBlockIndex].SubBlocks.Add(SubBlock);
                            BlockInfo[LastBlockIndex].TotalSize += SubBlock.FileSize;

                            //Console.WriteLine("\tSubblock[{0}] {1}", BlockIndex, SubBlock.File.Path);
                        }

                        NextInfo.FirstBlockIndex = (int)BlockIndex - 1;
                        NextInfo.LastBlockIndex = (int)BlockIndex - 1;

                        TotalBlockSize += NextInfo.Size;
                        BytesRemaining -= NextInfo.Size;
                        fi++;
                    }
                }
            }

            /*
            int BlockIndex = 0;
            long TotalBlockSize = 0;
            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                long FileOffset = 0;

                FileInfo.FirstBlockIndex = BlockIndex;

                while (true)
                {
                    long BytesLeftInBlock = (BlockSize - BlockInfo[BlockIndex].TotalSize);

                    long BytesLeftInFile = (FileInfo.Size - FileOffset);
                    long BytesInSubBlock = Math.Min(BytesLeftInFile, BytesLeftInBlock);

                    BuildManifestSubBlockInfo SubBlock;
                    SubBlock.File = FileInfo;
                    SubBlock.FileOffset = FileOffset;
                    SubBlock.FileSize = BytesInSubBlock;
                    SubBlock.OffsetInBlock = BlockInfo[BlockIndex].TotalSize;

                    if (BlockInfo[BlockIndex].SubBlocks == null)
                    {
                        BlockInfo[BlockIndex].SubBlocks = new List<BuildManifestSubBlockInfo>();
                    }
                    BlockInfo[BlockIndex].SubBlocks.Add(SubBlock);
                    BlockInfo[BlockIndex].TotalSize += BytesInSubBlock;
                    TotalBlockSize += BytesInSubBlock;
                    FileInfo.LastBlockIndex = BlockIndex;

                    if (BlockInfo[BlockIndex].TotalSize >= BlockSize)
                    {
                        BlockIndex++;
                    }

                    FileOffset += BytesInSubBlock;

                    // Got to end of this file, onto the next one.
                    if (FileOffset >= FileInfo.Size)
                    {
                        break;
                    }
                }                
            }
            */

            // Calcualte total size.
            TotalSize = 0;
            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                TotalSize += FileInfo.Size;
                FilesByPath.Add(FileInfo.Path, FileInfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public long GetTotalSizeOfBlocks(SparseStateArray Blocks)
        {
            long Result = 0;
            for (int i = 0; i < BlockCount; i++)
            {
                if (Blocks.Get(i))
                {
                    Result += BlockInfo[i].TotalSize;
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DebugCheck()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public BuildManifestFileInfo GetFileInfo(string Path)
        {
            if (FilesByPath.ContainsKey(Path))
            {
                return FilesByPath[Path];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public List<int> GetFileBlocks(string Path)
        {
            List<int> Result = new List<int>();

            BuildManifestFileInfo FileInfo = GetFileInfo(Path);
            if (FileInfo == null)
            {
                return Result;
            }

            for (int i = FileInfo.FirstBlockIndex; i <= FileInfo.LastBlockIndex; i++)
            {
                Result.Add(i);
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public bool GetBlockInfo(int Index, ref BuildManifestBlockInfo Info)
        {
            if (Index < 0 || Index >= BlockCount)
            {
                throw new ArgumentOutOfRangeException("Index", "Block index out of range.");
            }

            Info = BlockInfo[Index];
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeDirectory(string RootPath)
        {
            const int WriteChunkSize = 1 * 1024 * 1024;
            byte[] ChunkArray = new byte[WriteChunkSize];
            byte[] ChunkPattern = { 0xDE, 0xAD, 0xBE, 0xEF };

            for (int i = 0; i < WriteChunkSize; i++)
            {
                ChunkArray[i] = ChunkPattern[i % ChunkPattern.Length];
            }

            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                string FilePath = Path.Combine(RootPath, FileInfo.Path);
                string FileDir = System.IO.Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(FileDir))
                {
                    Directory.CreateDirectory(FileDir);
                }

                using (FileStream Stream = File.OpenWrite(FilePath))
                {
                    Stream.SetLength(FileInfo.Size);
                    /*
                    for (int Offset = 0; Offset < FileInfo.Size; Offset += WriteChunkSize)
                    {
                        long BytesRemaining = FileInfo.Size - Offset;
                        long ChunkSize = Math.Min(WriteChunkSize, BytesRemaining);
                        Stream.Write(ChunkArray, 0, (int)ChunkSize);
                    }
                    */
                    /*if (FileInfo.Size > 0)
                    {
                        Stream.Seek(FileInfo.Size - 1, SeekOrigin.Begin);
                        Stream.Write(ChunkArray, 0, 1);
                    }*/
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Validate(string RootPath, BandwidthTracker Tracker)
        {
            List<string> FailedFiles = new List<string>();

            int TaskCount = Environment.ProcessorCount * 16;
            Task[] FileTasks = new Task[TaskCount];
            int FileCounter = 0;

            for (int i = 0; i < TaskCount; i++)
            {
                FileTasks[i] = (Task.Run(() =>
                {
                    while (true)
                    {
                        int FileIndex = Interlocked.Increment(ref FileCounter) - 1;
                        if (FileIndex >= Files.Count)
                        {
                            break;
                        }

                        BuildManifestFileInfo FileInfo = Files[FileIndex];

                        string FilePath = Path.Combine(RootPath, FileInfo.Path);

                        if (!File.Exists(FilePath))
                        {
                            Logger.Log(LogLevel.Warning, LogCategory.Manifest, "File '" + FilePath + "' does not exist in folder, file system may have been modified externally.");

                            lock (FailedFiles)
                            {
                                FailedFiles.Add(FileInfo.Path);
                            }
                        }

                        string Checksum = FileUtils.GetChecksum(FilePath, Tracker);
                        if (FileInfo.Checksum != Checksum)
                        {
                            Logger.Log(LogLevel.Warning, LogCategory.Manifest, "File '" + FilePath + "' has an invalid checksum (got {0} expected {1}), file system may have been modified externally.", Checksum, FileInfo.Checksum);

                            lock (FailedFiles)
                            {
                                FailedFiles.Add(FileInfo.Path);
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(FileTasks);

            return FailedFiles;
        }

        /// <summary>
        /// 
        /// </summary>
        public static BuildManifest BuildFromDirectory(Guid NewManifestId, string RootPath, string VirtualPath, BuildManfiestProgressCallbackHandler Callback = null)
        {
            string[] FileNames = Directory.GetFiles(RootPath, "*", SearchOption.AllDirectories);
            long TotalSize = 0;

            // Try and order in the most efficient packing order.
            List<FileInfo> RemainingFiles = new List<FileInfo>();
            List<FileInfo> OrderedList = new List<FileInfo>();

            for (int i = 0; i < FileNames.Length; i++)
            {
                FileInfo Info = new FileInfo(FileNames[i]);
                RemainingFiles.Add(Info);
                TotalSize += Info.Length;
            }

            // Order main list from largest to smallest.
            RemainingFiles.Sort((Item1, Item2) => -Item1.Length.CompareTo(Item2.Length));

            // Try and add in optimal block packing format.
            long BlockIndex = 0;
            while (RemainingFiles.Count > 0)
            {
                FileInfo Info = RemainingFiles[0];
                RemainingFiles.RemoveAt(0);
                OrderedList.Add(Info);
                //Console.WriteLine("Block[{0}] {1}", BlockIndex, Info.Name);

                long BlockCount = (Info.Length + (BlockSize - 1)) / BlockSize;
                long BytesRemaining = BlockSize - (Info.Length % BlockSize);
                BlockIndex += BlockCount;

                // Try and fit some smaller files into the remaining block space.
                for (int i = 0; i < RemainingFiles.Count && BytesRemaining > 0; i++)
                {
                    FileInfo PotentialFile = RemainingFiles[i];
                    if (PotentialFile.Length <= BytesRemaining)
                    {
                        BytesRemaining -= PotentialFile.Length;

                        //Console.WriteLine("\tSubblock[{0}] {1}", BlockIndex, PotentialFile.Name);

                        RemainingFiles.RemoveAt(i);
                        OrderedList.Add(PotentialFile);
                        i--;
                        continue;
                    }
                }
            }

            // Our final list!
            FileInfo[] FileInfos = OrderedList.ToArray();

            BuildManifest Manifest = new BuildManifest();
            Manifest.Guid = NewManifestId;
            Manifest.VirtualPath = VirtualPath;
            Manifest.BlockCount = BlockIndex;

            List<Task> FileTasks = new List<Task>();
            int FileCounter = 0;

            for (int i = 0; i < FileInfos.Length; i++)
            {
                Manifest.Files.Add(new BuildManifestFileInfo());
            }

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                FileTasks.Add(Task.Run(() =>
                {
                    while (true)
                    {
                        int FileIndex = Interlocked.Increment(ref FileCounter) - 1;
                        if (FileIndex >= FileInfos.Length)
                        {
                            break;
                        }

                        FileInfo SubFileInfo = FileInfos[FileIndex];
                        string SubFile = SubFileInfo.FullName;
                        string RelativePath = SubFile.Substring(RootPath.Length).Trim('\\', '/');

                        if (Callback != null)
                        {
                            lock (Callback)
                            {
                                Callback(RelativePath, ((float)FileIndex / (float)FileInfos.Length) * 100);
                            }
                        }

                        BuildManifestFileInfo ManifestFileInfo = new BuildManifestFileInfo();
                        ManifestFileInfo.Path = RelativePath;
                        ManifestFileInfo.Size = (new FileInfo(SubFile)).Length;
                        ManifestFileInfo.Checksum = FileUtils.GetChecksum(SubFile, null);
                        lock (Manifest)
                        {
                            Manifest.Files[FileIndex] = ManifestFileInfo;
                        }
                    }
                }));
            }

            foreach (Task task in FileTasks)
            {
                task.Wait();
            }

            Manifest.CacheBlockInfo();
            Manifest.DebugCheck();

            return Manifest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return FileUtils.WriteToArray<BuildManifest>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public void WriteToFile(string FilePath)
        {
            FileUtils.WriteToBinaryFile<BuildManifest>(FilePath, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static BuildManifest FromByteArray(byte[] ByteArray)
        {
            BuildManifest Manifest = FileUtils.ReadFromArray<BuildManifest>(ByteArray);
            if (Manifest != null)
            {
                Manifest.CacheBlockInfo();
            }
            return Manifest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static BuildManifest ReadFromFile(string FilePath)
        {
            BuildManifest Manifest = FileUtils.ReadFromBinaryFile<BuildManifest>(FilePath);
            if (Manifest != null)
            {
                Manifest.CacheBlockInfo();
            }
            return Manifest;
        }
    }
}
