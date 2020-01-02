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
    public struct BuildManifestBlockInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public BuildManifestFileInfo File;

        /// <summary>
        /// 
        /// </summary>
        public long Offset;

        /// <summary>
        /// 
        /// </summary>
        public long Size;
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
        public long BlockSize = 512 * 1024;

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

            int BlockIndex = 0;
            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                long FileOffset = 0;

                FileInfo.FirstBlockIndex = BlockIndex;

                while (true)
                {
                    long BytesRemaining = (FileInfo.Size - FileOffset);
                    long BytesBlock = Math.Min(BytesRemaining, BlockSize);

                    BlockInfo[BlockIndex].File = FileInfo;
                    BlockInfo[BlockIndex].Offset = FileOffset;
                    BlockInfo[BlockIndex].Size = BytesBlock;

                    FileInfo.LastBlockIndex = BlockIndex;

                    BlockIndex++;
                    FileOffset += BlockSize;

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
                    Result += BlockInfo[i].Size;
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DebugCheck()
        {
            /*for (int i = 0; i < BlockCount; i++)
            {*/
                BuildManifestFileInfo FileInfo = new BuildManifestFileInfo();
                long BlockOffset = 0;
                long BlockSize = 0;
                GetBlockInfo((int)(BlockCount - 1), ref FileInfo, ref BlockOffset, ref BlockSize);
            //}

            for (int i = 0; i < Files.Count; i++)
            {
                GetFileBlocks(Files[i].Path);
            }
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
        public bool GetBlockInfo(int Index, ref BuildManifestFileInfo File, ref long Offset, ref long Size)
        {
            if (Index < 0 || Index >= BlockCount)
            {
                throw new ArgumentOutOfRangeException("Index", "Block index out of range.");
            }

            BuildManifestBlockInfo Block = BlockInfo[Index];

            File = Block.File;
            Offset = Block.Offset;
            Size = Block.Size;

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
                    /*
                    for (int Offset = 0; Offset < FileInfo.Size; Offset += WriteChunkSize)
                    {
                        long BytesRemaining = FileInfo.Size - Offset;
                        long ChunkSize = Math.Min(WriteChunkSize, BytesRemaining);
                        Stream.Write(ChunkArray, 0, (int)ChunkSize);
                    }
                    */
                    if (FileInfo.Size > 0)
                    {
                        Stream.Seek(FileInfo.Size - 1, SeekOrigin.Begin);
                        Stream.Write(ChunkArray, 0, 1);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RootPath"></param>
        /// <param name="BlockIndex"></param>
        /// <param name="Data"></param>
        public void StoreBlock(string RootPath, int BlockIndex, byte[] Data)
        {
            BuildManifestFileInfo FileInfo = null;
            long BlockOffset = 0;
            long BlockSize = 0;

            GetBlockInfo(BlockIndex, ref FileInfo, ref BlockOffset, ref BlockSize);

            string FilePath = Path.Combine(RootPath, FileInfo.Path);

            if (File.Exists(FilePath))
            {
                throw new ArgumentOutOfRangeException("RootPath", "File to store block in does not exists. File system may have been modified externally?");
            }

            if (Data.Length < BlockSize)
            {
                throw new ArgumentOutOfRangeException("Data", "Data is smaller than the expected size of the block.");
            }

            using (FileStream Stream = File.OpenWrite(FilePath))
            {
                Stream.Seek(BlockOffset, SeekOrigin.Begin);
                Stream.Write(Data, 0, (int)BlockSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Validate(string RootPath)
        {
            List<string> FailedFiles = new List<string>();

            List<Task> FileTasks = new List<Task>();
            int FileCounter = 0;

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                FileTasks.Add(Task.Run(() =>
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

                        string Checksum = FileUtils.GetChecksum(FilePath);
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

            foreach (Task task in FileTasks)
            {
                task.Wait();
            }
           
            return FailedFiles;
        }

        /// <summary>
        /// 
        /// </summary>
        public static BuildManifest BuildFromDirectory(Guid NewManifestId, string RootPath, string VirtualPath, BuildManfiestProgressCallbackHandler Callback = null)
        {
            string[] Files = Directory.GetFiles(RootPath, "*", SearchOption.AllDirectories);

            BuildManifest Manifest = new BuildManifest();
            Manifest.Guid = NewManifestId;
            Manifest.VirtualPath = VirtualPath;
            Manifest.BlockCount = 0;

            List<Task> FileTasks = new List<Task>();
            int FileCounter = 0;

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                FileTasks.Add(Task.Run(() =>
                {
                    while (true)
                    {
                        int FileIndex = Interlocked.Increment(ref FileCounter) - 1;
                        if (FileIndex >= Files.Length)
                        {
                            break;
                        }

                        string SubFile = Files[FileIndex];
                        string RelativePath = SubFile.Substring(RootPath.Length).Trim('\\', '/');

                        if (Callback != null)
                        {
                            lock (Callback)
                            {
                                Callback(RelativePath, ((float)FileIndex / (float)Files.Length) * 100);
                            }
                        }

                        BuildManifestFileInfo ManifestFileInfo = new BuildManifestFileInfo();
                        ManifestFileInfo.Path = RelativePath;
                        ManifestFileInfo.Size = (new FileInfo(SubFile)).Length;
                        ManifestFileInfo.Checksum = FileUtils.GetChecksum(SubFile);
                        lock (Manifest)
                        {
                            Manifest.Files.Add(ManifestFileInfo);
                        }

                        long FileBlockCount = Math.Max(1, (ManifestFileInfo.Size + (Manifest.BlockSize - 1)) / Manifest.BlockSize);
                        //Logger.Log(LogLevel.Info, LogCategory.Manifest, "File[{0}] Size={1} Blocks={2}", ManifestFileInfo.Path, ManifestFileInfo.Size, FileBlockCount);
                        Interlocked.Add(ref Manifest.BlockCount, FileBlockCount);
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
