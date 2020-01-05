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

            // Make a list of all possible sub-blocks.


            // Take largest sub-block, then fill remaining space with other blocks.

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

            // Calcualte total size.
            TotalSize = 0;
            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                TotalSize += FileInfo.Size;
                FilesByPath.Add(FileInfo.Path, FileInfo);
            }

            Console.WriteLine("Blocks:{0}/{1} Size:{2}/{3}", BlockIndex, BlockCount, TotalBlockSize, TotalSize);
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
            FileInfo[] FileInfos = new FileInfo[FileNames.Length];
            long TotalSize = 0;
            for (int i = 0; i < FileNames.Length; i++)
            {
                FileInfos[i] = new FileInfo(FileNames[i]);
                TotalSize += FileInfos[i].Length;
            }

            // Order files by size so they fit into sub-blocks more optimally.
            Array.Sort(FileInfos, (Item1, Item2) => Item1.Length.CompareTo(Item2.Length));

            BuildManifest Manifest = new BuildManifest();
            Manifest.Guid = NewManifestId;
            Manifest.VirtualPath = VirtualPath;
            Manifest.BlockCount = Math.Max(1, (TotalSize + (BuildManifest.BlockSize - 1)) / BuildManifest.BlockSize);

            List<Task> FileTasks = new List<Task>();
            int FileCounter = 0;

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
                            Manifest.Files.Add(ManifestFileInfo);
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
