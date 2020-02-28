﻿/*
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Manifests
{
    /// <summary>
    /// </summary>
    /// <param name="CurrentFile"></param>
    /// <param name="OverallProgress"></param>
    public delegate void BuildManfiestProgressCallbackHandler(string CurrentFile, float OverallProgress);

    /// <summary>
    /// </summary>
    /// <param name="BytesProcessed"></param>
    public delegate bool BuildManfiestValidateProgressCallbackHandler(long BytesRead, long TotalBytes, Guid ManifestId, int BlockIndex);

    /// <summary>
    /// </summary>
    /// <param name="BytesProcessed"></param>
    public delegate bool BuildManfiestInitProgressCallbackHandler(long BytesWritten, long TotalBytes);

    /// <summary>
    /// </summary>
    [Serializable]
    public class BuildManifestFileInfo
    {
        /// <summary>
        /// </summary>
        public string Checksum;

        /// <summary>
        /// </summary>
        public string Path;

        /// <summary>
        /// </summary>
        public long Size;

        /// <summary>
        /// </summary>
        [NonSerialized]
        internal int FirstBlockIndex;

        /// <summary>
        /// </summary>
        [NonSerialized]
        internal int LastBlockIndex;
    }

    /// <summary>
    /// </summary>
    public struct BuildManifestSubBlockInfo
    {
        /// <summary>
        /// </summary>
        public BuildManifestFileInfo File;

        /// <summary>
        /// </summary>
        public long FileOffset;

        /// <summary>
        /// </summary>
        public long FileSize;

        /// <summary>
        /// </summary>
        public long OffsetInBlock;
    }

    /// <summary>
    /// </summary>
    public struct BuildManifestBlockInfo
    {
        /// <summary>
        /// </summary>
        public List<BuildManifestSubBlockInfo> SubBlocks;

        /// <summary>
        /// </summary>
        public long TotalSize;
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class BuildManifest
    {
        /// <summary>
        /// </summary>
        public static long BlockSize = 1 * 1024 * 1024;

        /// <summary>
        /// </summary>
        public uint[] BlockChecksums;

        /// <summary>
        /// </summary>
        public long BlockCount;

        /// <summary>
        /// </summary>
        public DateTime CreateTime = DateTime.UtcNow;

        /// <summary>
        /// </summary>
        public List<BuildManifestFileInfo> Files = new List<BuildManifestFileInfo>();

        /// <summary>
        /// </summary>
        public Guid Guid;

        /// <summary>
        /// </summary>
        public string VirtualPath;

        /// <summary>
        /// </summary>
        [NonSerialized]
        private BuildManifestBlockInfo[] BlockInfo = new BuildManifestBlockInfo[0];

        /// <summary>
        /// </summary>
        [NonSerialized]
        private Dictionary<string, BuildManifestFileInfo> FilesByPath = new Dictionary<string, BuildManifestFileInfo>();

        /// <summary>
        /// </summary>
        [NonSerialized]
        private long TotalSize = -1;

        /// <summary>
        /// </summary>
        public static BuildManifest BuildFromDirectory(Guid NewManifestId, string RootPath, string VirtualPath, AsyncIOQueue IOQueue, BuildManfiestProgressCallbackHandler Callback = null)
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
                long BytesRemaining = (Info.Length % BlockSize) == 0 ? 0 : BlockSize - Info.Length % BlockSize;
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
                    }
                }
            }

            // Our final list!
            FileInfo[] FileInfos = OrderedList.ToArray();

            BuildManifest Manifest = new BuildManifest();
            Manifest.Guid = NewManifestId;
            Manifest.VirtualPath = VirtualPath;
            Manifest.BlockCount = BlockIndex;
            Manifest.BlockChecksums = new uint[Manifest.BlockCount];

            List<Task> ChecksumTasks = new List<Task>();
            int FileCounter = 0;
            int BlockCounter = 0;

            for (int i = 0; i < FileInfos.Length; i++)
            {
                Manifest.Files.Add(new BuildManifestFileInfo());
            }

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                ChecksumTasks.Add(
                    Task.Run(
                        () =>
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
                                        float Progress = (FileCounter + BlockCounter) / (float) (FileInfos.Length + Manifest.BlockCount);
                                        Callback(RelativePath, Progress * 100);
                                    }
                                }

                                BuildManifestFileInfo ManifestFileInfo = new BuildManifestFileInfo();
                                ManifestFileInfo.Path = RelativePath;
                                ManifestFileInfo.Size = new FileInfo(SubFile).Length;
                                ManifestFileInfo.Checksum = FileUtils.GetChecksum(SubFile, null);
                                lock (Manifest)
                                {
                                    Manifest.Files[FileIndex] = ManifestFileInfo;
                                }
                            }
                        }
                    )
                );
            }

            foreach (Task task in ChecksumTasks)
            {
                task.Wait();
            }

            ChecksumTasks.Clear();

            // Calculate which data goes in eahc block.
            Manifest.CacheBlockInfo();
            Manifest.CacheSizeInfo();
            Manifest.DebugCheck();

            // Calculate checksum for each individual block.
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                ChecksumTasks.Add(
                    Task.Run(
                        () =>
                        {
                            byte[] Buffer = new byte[BlockSize];

                            while (true)
                            {
                                int CalculateBlockIndex = Interlocked.Increment(ref BlockCounter) - 1;
                                if (CalculateBlockIndex >= Manifest.BlockCount)
                                {
                                    break;
                                }

                                long BufferLength = 0;
                                Manifest.GetBlockData(CalculateBlockIndex, RootPath, IOQueue, Buffer, out BufferLength);

                                Manifest.BlockChecksums[CalculateBlockIndex] = Crc32.Compute(Buffer, (int) BufferLength);

                                if (Callback != null)
                                {
                                    lock (Callback)
                                    {
                                        float Progress = (FileCounter + BlockCounter) / (float) (FileInfos.Length + Manifest.BlockCount);
                                        Callback("Checksuming blocks", Progress * 100);
                                    }
                                }
                            }
                        }
                    )
                );
            }

            foreach (Task task in ChecksumTasks)
            {
                task.Wait();
            }

            return Manifest;
        }

        /// <summary>
        /// </summary>
        public void DebugCheck()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static BuildManifest FromByteArray(byte[] ByteArray)
        {
            BuildManifest Manifest = FileUtils.ReadFromArray<BuildManifest>(ByteArray);
            if (Manifest != null)
            {
                Manifest.CacheBlockInfo();
                Manifest.CacheSizeInfo();
            }

            return Manifest;
        }

        /// <summary>
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public void GetBlockData(int Index, string RootPath, AsyncIOQueue IOQueue, byte[] Data, out long DataLength)
        {
            DataLength = 0;

            if (Index < 0 || Index >= BlockCount)
            {
                throw new ArgumentOutOfRangeException("Index", "Block index out of range.");
            }

            BuildManifestBlockInfo Info = BlockInfo[Index];
            Debug.Assert(Data.Length >= Info.TotalSize);
            DataLength = Info.TotalSize;

            ManualResetEvent CompleteEvent = new ManualResetEvent(false);
            int QueuedReads = Info.SubBlocks.Count;

            foreach (BuildManifestSubBlockInfo SubBlock in Info.SubBlocks)
            {
                string PathName = Path.Combine(RootPath, SubBlock.File.Path);
                IOQueue.Read(
                    PathName, SubBlock.FileOffset, SubBlock.FileSize, Data, SubBlock.OffsetInBlock, bSuccess =>
                    {
                        if (!bSuccess)
                        {
                            throw new IOException("Failed to read data for block from file: " + PathName + ".");
                        }

                        if (Interlocked.Decrement(ref QueuedReads) == 0)
                        {
                            CompleteEvent.Set();
                        }
                    }
                );
            }

            CompleteEvent.WaitOne();
        }

        /// <summary>
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
        /// </summary>
        /// <returns></returns>
        public long GetTotalSize()
        {
            return TotalSize;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public long GetTotalSizeOfBlocks(SparseStateArray Blocks)
        {
            long Result = 0;

            foreach (SparseStateArray.Range Range in Blocks.Ranges)
            {
                if (Range.State)
                {
                    for (int i = Range.Start; i <= Range.End; i++)
                    {
                        Result += BlockInfo[i].TotalSize;
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// </summary>
        public void InitializeDirectory(string RootPath, AsyncIOQueue IOQueue, bool AllocateFiles, BuildManfiestInitProgressCallbackHandler Callback = null)
        {
            const int WriteChunkSize = 16 * 1024 * 1024;
            byte[] ChunkArray = new byte[WriteChunkSize];
            byte[] ChunkPattern = {0xDE, 0xAD, 0xBE, 0xEF};

            for (int i = 0; i < WriteChunkSize; i++)
            {
                ChunkArray[i] = ChunkPattern[i % ChunkPattern.Length];
            }

            long TotalBytes = 0;
            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                TotalBytes += FileInfo.Size;
            }

            long BytesWritten = 0;
            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                string FilePath = Path.Combine(RootPath, FileInfo.Path);
                string FileDir = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(FileDir))
                {
                    Directory.CreateDirectory(FileDir);
                }

                if (AllocateFiles)
                {
                    using (FileStream Stream = File.OpenWrite(FilePath))
                    {
                        long BytesRemaining = FileInfo.Size;
                        while (BytesRemaining > 0)
                        {
                            long Size = Math.Min(BytesRemaining, WriteChunkSize);
                            Stream.Write(ChunkArray, 0, (int) Size);
                            BytesWritten += Size;
                            BytesRemaining -= Size;

                            AsyncIOQueue.GlobalBandwidthStats.In(Size);

                            if (Callback != null)
                            {
                                if (!Callback.Invoke(BytesWritten, TotalBytes))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static BuildManifest ReadFromFile(string FilePath, bool CacheDownloadInfo)
        {
            BuildManifest Manifest = FileUtils.ReadFromBinaryFile<BuildManifest>(FilePath);
            if (Manifest != null && CacheDownloadInfo)
            {
                Manifest.CacheBlockInfo();
            }
            Manifest.CacheSizeInfo();

            return Manifest;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return FileUtils.WriteToArray(this);
        }

        /// <summary>
        /// </summary>
        public List<int> Validate(string RootPath, RateTracker Tracker, AsyncIOQueue IOQueue, BuildManfiestValidateProgressCallbackHandler Callback = null)
        {
            List<int> FailedBlocks = new List<int>();

            int TaskCount = Environment.ProcessorCount;
            Task[] FileTasks = new Task[TaskCount];
            int BlockCounter = 0;

            long BytesValidated = 0;
            bool Aborted = false;

            for (int i = 0; i < TaskCount; i++)
            {
                FileTasks[i] = Task.Run(
                    () =>
                    {
                        byte[] Buffer = new byte[BlockSize];

                        while (!Aborted)
                        {
                            int BlockIndex = Interlocked.Increment(ref BlockCounter) - 1;
                            if (BlockIndex >= BlockCount)
                            {
                                break;
                            }

                            BuildManifestBlockInfo BInfo = BlockInfo[BlockIndex];

                            long BufferLength = 0;
                            GetBlockData(BlockIndex, RootPath, IOQueue, Buffer, out BufferLength);

                            if (BlockChecksums[BlockIndex] != Crc32.Compute(Buffer, (int) BufferLength))
                            {
                                Logger.Log(LogLevel.Warning, LogCategory.Manifest, "Block {0} failed checksum, block contains following sub-blocks:", BlockIndex);

                                for (int SubBlock = 0; SubBlock < BInfo.SubBlocks.Count; SubBlock++)
                                {
                                    BuildManifestSubBlockInfo SubBInfo = BInfo.SubBlocks[SubBlock];
                                    Logger.Log(LogLevel.Warning, LogCategory.Manifest, "\tfile={0} offset={1} size={2}", SubBInfo.File.Path, SubBInfo.FileOffset, SubBInfo.FileSize);
                                }

                                lock (FailedBlocks)
                                {
                                    FailedBlocks.Add(BlockIndex);
                                }
                            }

                            Interlocked.Add(ref BytesValidated, BInfo.TotalSize);

                            if (Callback != null)
                            {
                                if (!Callback.Invoke(BytesValidated, TotalSize, Guid, BlockIndex))
                                {
                                    Aborted = true;
                                }
                            }
                        }
                    }
                );
            }

            Task.WaitAll(FileTasks);

            return FailedBlocks;
        }

        /// <summary>
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public void WriteToFile(string FilePath)
        {
            FileUtils.WriteToBinaryFile(FilePath, this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CacheSizeInfo()
        {
            // Calcualte total size.
            TotalSize = 0;
            foreach (BuildManifestFileInfo FileInfo in Files)
            {
                TotalSize += FileInfo.Size;

                if (FilesByPath != null)
                {
                    FilesByPath.Add(FileInfo.Path, FileInfo);
                }
            }
        }

        /// <summary>
        /// </summary>
        private void CacheBlockInfo()
        {
            // Store block information.
            BlockInfo = new BuildManifestBlockInfo[BlockCount];
            FilesByPath = new Dictionary<string, BuildManifestFileInfo>();

            // Try and add in optimal block packing format.
            //Console.WriteLine("======================================== CACHING BLOCK INFO ================================");
            long BlockIndex = 0;
            for (int fi = 0; fi < Files.Count;)
            {
                BuildManifestFileInfo Info = Files[fi];
                Info.FirstBlockIndex = (int) BlockIndex;
                fi++;

                long BlockCount = (Info.Size + (BlockSize - 1)) / BlockSize;
                long BytesRemaining = (Info.Size % BlockSize) == 0 ? 0 : BlockSize - Info.Size % BlockSize; 

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

                    Debug.Assert(BlockInfo[BlockIndex].TotalSize <= BuildManifest.BlockSize);

                    Total += SubBlock.FileSize;

                    BlockIndex++;
                }

                Debug.Assert(Total == Info.Size);

                Info.LastBlockIndex = (int) BlockIndex - 1;

                int LastBlockIndex = Info.LastBlockIndex;

                // Fill remaining space with blocks.
                Debug.Assert(BlockInfo[LastBlockIndex].TotalSize <= BuildManifest.BlockSize);
                while (BytesRemaining > 0 && fi < Files.Count)
                {
                    BuildManifestFileInfo NextInfo = Files[fi];
                    if (NextInfo.Size > BytesRemaining)
                    {
                        break;
                    }

                    if (NextInfo.Size > 0)
                    {
                        BuildManifestSubBlockInfo SubBlock;
                        SubBlock.File = NextInfo;
                        SubBlock.FileOffset = 0;
                        SubBlock.FileSize = NextInfo.Size;
                        SubBlock.OffsetInBlock = BlockInfo[LastBlockIndex].TotalSize;
                        BlockInfo[LastBlockIndex].SubBlocks.Add(SubBlock);
                        BlockInfo[LastBlockIndex].TotalSize += SubBlock.FileSize;

                        Debug.Assert(BlockInfo[LastBlockIndex].TotalSize <= BuildManifest.BlockSize);

                        //Console.WriteLine("\tSubblock[{0}] {1}", BlockIndex, SubBlock.File.Path);
                    }

                    NextInfo.FirstBlockIndex = (int) BlockIndex - 1;
                    NextInfo.LastBlockIndex = (int) BlockIndex - 1;

                    BytesRemaining -= NextInfo.Size;
                    //Console.WriteLine("\tRemaining: {0}", BytesRemaining);
                    fi++;
                }
            }
        }
    }
}