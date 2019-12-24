using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public long BlockSize = 10 * 1024 * 1024;

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
        /// <returns></returns>
        public long GetTotalSize()
        {
            long Result = 0;
            for (int i = 0; i < Files.Count; i++)
            {
                BuildManifestFileInfo FileInfo = Files[i];
                Result += FileInfo.Size;
            }
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public long GetTotalSizeOfBlocks(SparseStateArray Blocks)
        {
            long Result = 0;

            int BlockIndex = 0;
            for (int i = 0; i < Files.Count; i++)
            {
                BuildManifestFileInfo FileInfo = Files[i];
                long FileOffset = 0;

                while (true)
                {
                    long BytesRemaining = (FileInfo.Size - FileOffset);
                    long CurrentBlockSize = Math.Min(BytesRemaining, BlockSize);

                    if (Blocks.Get(BlockIndex))
                    {
                        Result += CurrentBlockSize;
                    }

                    BlockIndex++;
                    FileOffset += BlockSize;

                    // Got to end of this file, onto the next one.
                    if (FileOffset >= FileInfo.Size)
                    {
                        break;
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public List<int> GetFileBlocks(string Path)
        {
            List<int> Result = new List<int>();

            int BlockIndex = 0;
            for (int i = 0; i < Files.Count; i++)
            {
                BuildManifestFileInfo FileInfo = Files[i];
                long FileOffset = 0;

                bool IsCorrectFile = (FileInfo.Path == Path);

                while (true)
                {
                    if (IsCorrectFile)
                    {
                        Result.Add(BlockIndex);
                    }

                    BlockIndex++;
                    FileOffset += BlockSize;

                    // Got to end of this file, onto the next one.
                    if (FileOffset >= FileInfo.Size)
                    {
                        break;
                    }
                }

                if (IsCorrectFile)
                {
                    break;
                }
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

            int BlockIndex = 0;
            for (int i = 0; i < Files.Count; i++)
            {
                BuildManifestFileInfo FileInfo = Files[i];
                long FileOffset = 0;

                while (true)
                {
                    // Found our block!
                    if (BlockIndex == Index)
                    {
                        long BytesRemaining = (FileInfo.Size - FileOffset);

                        File = FileInfo;
                        Offset = FileOffset;
                        Size = Math.Min(BytesRemaining, BlockSize);

                        return true;
                    }

                    BlockIndex++;
                    FileOffset += BlockSize;

                    // Got to end of this file, onto the next one.
                    if (FileOffset >= FileInfo.Size)
                    {
                        break;
                    }
                }
            }

            throw new ArgumentOutOfRangeException("Index", "Block index was invalid. Manifest is possibly corrupt?");
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
                    for (int Offset = 0; Offset < FileInfo.Size; Offset += WriteChunkSize)
                    {
                        long BytesRemaining = FileInfo.Size - Offset;
                        long ChunkSize = Math.Min(WriteChunkSize, BytesRemaining);
                        Stream.Write(ChunkArray, 0, (int)ChunkSize);
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

            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                string FilePath = Path.Combine(RootPath, FileInfo.Path);

                if (!File.Exists(FilePath))
                {
                    Console.WriteLine("File '" + FilePath + "' does not exist in folder, file system may have been modified externally.");
                    FailedFiles.Add(FileInfo.Path);
                    continue;
                }

                string Checksum = FileUtils.GetChecksum(FilePath);
                if (FileInfo.Checksum != Checksum)
                {
                    Console.WriteLine("File '" + FilePath + "' has an invalid checksum (got {0} expected {1}), file system may have been modified externally.", Checksum, FileInfo.Checksum);
                    FailedFiles.Add(FileInfo.Path);
                    continue;
                }
            }

            return FailedFiles;
        }

        /// <summary>
        /// 
        /// </summary>
        public static BuildManifest BuildFromDirectory(string RootPath, string VirtualPath, BuildManfiestProgressCallbackHandler Callback = null)
        {
            string[] Files = Directory.GetFiles(RootPath, "*", SearchOption.AllDirectories);

            BuildManifest Manifest = new BuildManifest();
            Manifest.Guid = System.Guid.NewGuid();
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

                        long FileBlockCount = (ManifestFileInfo.Size + (Manifest.BlockSize - 1)) / Manifest.BlockSize;
                        Interlocked.Add(ref Manifest.BlockCount, FileBlockCount);
                    }
                }));
            }

            foreach (Task task in FileTasks)
            {
                task.Wait();
            }

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
            return FileUtils.ReadFromArray<BuildManifest>(ByteArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static BuildManifest ReadFromFile(string FilePath)
        {
            return FileUtils.ReadFromBinaryFile<BuildManifest>(FilePath);
        }
    }
}
