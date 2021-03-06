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

//#define EMULATE_IO
//#define DISABLE_IO_BUFFERING

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// </summary>
    public delegate void IOCompleteCallbackHandler(bool bSuccess);

    /// <summary>
    /// </summary>
    public class Statistic_DiskIn : Statistic
    {
        public Statistic_DiskIn()
        {
            Name = @"IO\Disk Write";
            MaxLabel = "32 MB/s";
            MaxValue = 32.0f * 1024.0f * 1024.0f;
            DefaultShown = true;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsTransferRate = true;
        }

        public override void Gather()
        {
            AddSample(AsyncIOQueue.GlobalBandwidthStats.RateIn);
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_DiskOut : Statistic
    {
        public Statistic_DiskOut()
        {
            Name = @"IO\Disk Read";
            MaxLabel = "32 MB/s";
            MaxValue = 32.0f * 1024.0f * 1024.0f;
            DefaultShown = true;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsTransferRate = true;
        }

        public override void Gather()
        {
            AddSample(AsyncIOQueue.GlobalBandwidthStats.RateOut);
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_DiskInQueue : Statistic
    {
        public Statistic_DiskInQueue()
        {
            Name = @"IO\Disk Write Queue";
            MaxLabel = "32 MB";
            MaxValue = 32.0f * 1024.0f * 1024.0f;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsTransferRate = true;
        }

        public override void Gather()
        {
            AddSample(AsyncIOQueue.QueuedIn);
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_DiskOutQueue : Statistic
    {
        public Statistic_DiskOutQueue()
        {
            Name = @"IO\Disk Read Queue";
            MaxLabel = "32 MB";
            MaxValue = 32.0f * 1024.0f * 1024.0f;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsTransferRate = true;
        }

        public override void Gather()
        {
            AddSample(AsyncIOQueue.QueuedOut / 1024.0f / 1024.0f);
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_DiskOutLatency : Statistic
    {
        public Statistic_DiskOutLatency()
        {
            Name = @"IO\Disk Read Latency (ms)";
            MaxLabel = "100";
            MaxValue = 100.0f;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }

        public override void Gather()
        {
            AddSample((float) AsyncIOQueue.OutLatency.Get());
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_DiskInLatency : Statistic
    {
        public Statistic_DiskInLatency()
        {
            Name = @"IO\Disk Write Latency (ms)";
            MaxLabel = "100";
            MaxValue = 100.0f;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }

        public override void Gather()
        {
            AddSample((float) AsyncIOQueue.InLatency.Get());
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_IOQueueSize : Statistic
    {
        public Statistic_IOQueueSize()
        {
            Name = @"IO\Task Queue Size";
            MaxLabel = "10";
            MaxValue = 10.0f;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }

        public override void Gather()
        {
            AddSample(AsyncIOQueue.TaskQueueCount);
        }
    }

    /// <summary>
    /// </summary>
    public class Statistic_IOTaskProcessTime : Statistic
    {
        public Statistic_IOTaskProcessTime()
        {
            Name = @"IO\Task Process Time (ms)";
            MaxLabel = "10";
            MaxValue = 10.0f;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsInteger = true;
        }

        public override void Gather()
        {
            AddSample((float) AsyncIOQueue.TaskProcessTime.Get());
        }
    }

    /// <summary>
    /// </summary>
    public class AsyncIOQueue
    {
        public class ActiveStream
        {
            public int ActiveOperations;
            public bool CanWrite;
            public ulong LastAccessed;
            public string Path;
            public Stream Stream;
        }

        private class Task
        {
            public IOCompleteCallbackHandler Callback;
            public byte[] Data;
            public long DataOffset;
            public long Offset;
            public string Path;
            public ulong QueueTime;
            public long Size;
            public TaskType Type;
        }

        public enum TaskType
        {
            Read,
            Write,
            DeleteDir,
            CloseStreamsInDir
        }

        public static RateTracker GlobalBandwidthStats = new RateTracker(10000);
        public static RollingAverage InLatency = new RollingAverage(25);

        public static RollingAverage OutLatency = new RollingAverage(25);
        public static RollingAverage TaskProcessTime = new RollingAverage(5);
        public static int TaskQueueCount;
        private const int AverageStreamOpSizeHeuristic = 1 * 1024 * 1024;
        private static long GlobalQueuedIn;

        private static long GlobalQueuedOut;

        private const int MaxStreamAge = 10 * 1000;
        private const int MaxStreams = 128;

        private const int StreamBufferSize = 64 * 1024; // StreamMaxConcurrentOps * AverageStreamOpSizeHeuristic;

        //private const int StreamMaxConcurrentOps = 20;
        private const int StreamMaxConcurrentOps = 16;

        private readonly List<ActiveStream> ActiveStreams = new List<ActiveStream>();
        private readonly Dictionary<string, ActiveStream> ActiveStreamsByPath = new Dictionary<string, ActiveStream>();

        private int ActiveTasks;
        private bool IsRunning;

        private List<string> LockedPaths = new List<string>();

        private readonly ConcurrentQueue<Task> TaskQueue = new ConcurrentQueue<Task>();

        private readonly Thread TaskThread;
        private readonly object WakeObject = new object();

        /// <summary>
        /// </summary>
        public bool IsEmpty => TaskQueue.IsEmpty && ActiveTasks == 0;

        /// <summary>
        /// </summary>
        public static long QueuedIn => GlobalQueuedIn;

        /// <summary>
        /// </summary>
        public static long QueuedOut => GlobalQueuedOut;

        /// <summary>
        /// </summary>
        public AsyncIOQueue()
        {
            IsRunning = true;

            TaskThread = new Thread(ThreadEntry);
            TaskThread.IsBackground = true;
            TaskThread.Name = "Async IO";
            TaskThread.Start();
        }

        /// <summary>
        /// </summary>
        /// <param name="Folder"></param>
        public void CloseAllStreamsInDirectory(string Folder)
        {
            AutoResetEvent Event = new AutoResetEvent(false);

            Interlocked.Increment(ref TaskQueueCount);
            TaskQueue.Enqueue(new Task { Type = TaskType.CloseStreamsInDir, Path = Folder, Offset = 0, Size = 0, Data = null, DataOffset = 0, Callback = (bool bSuccess) => {
                Event.Set();
            }, QueueTime = TimeUtils.Ticks });
            WakeThread();

            Event.WaitOne();
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public void DeleteDir(string InPath, IOCompleteCallbackHandler InCallback)
        {
            Interlocked.Increment(ref TaskQueueCount);
            TaskQueue.Enqueue(new Task {Type = TaskType.DeleteDir, Path = InPath, Callback = InCallback, QueueTime = TimeUtils.Ticks});
            WakeThread();
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public void Read(string InPath, long InOffset, long InSize, byte[] InData, long InDataOffset, IOCompleteCallbackHandler InCallback)
        {
            Interlocked.Add(ref GlobalQueuedOut, InSize);
            Interlocked.Increment(ref TaskQueueCount);
            TaskQueue.Enqueue(new Task {Type = TaskType.Read, Path = InPath, Offset = InOffset, Size = InSize, Data = InData, DataOffset = InDataOffset, Callback = InCallback, QueueTime = TimeUtils.Ticks});
            WakeThread();
        }

        /// <summary>
        /// </summary>
        public void Stop()
        {
            lock (WakeObject)
            {
                IsRunning = false;
                Monitor.Pulse(WakeObject);
            }

            TaskThread.Join();
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public void Write(string InPath, long InOffset, long InSize, byte[] InData, long InDataOffset, IOCompleteCallbackHandler InCallback)
        {
            Interlocked.Add(ref GlobalQueuedIn, InSize);
            Interlocked.Increment(ref TaskQueueCount);
            TaskQueue.Enqueue(new Task {Type = TaskType.Write, Path = InPath, Offset = InOffset, Size = InSize, Data = InData, DataOffset = InDataOffset, Callback = InCallback, QueueTime = TimeUtils.Ticks});
            WakeThread();
        }

        /// <summary>
        /// </summary>
        private void CloseOldStreams()
        {
            ulong CurrentTicks = TimeUtils.Ticks;

            lock (ActiveStreams)
            {
                for (int i = 0; i < ActiveStreams.Count; i++)
                {
                    ActiveStream Stm = ActiveStreams[i];

                    ulong Elapsed = CurrentTicks - Stm.LastAccessed;
                    if (Elapsed > MaxStreamAge && Stm.ActiveOperations == 0)
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.IO, "Closing stream for async queue: {0}", Stm.Path);

                        Stm.Stream.Close();

                        ActiveStreams.RemoveAt(i);
                        ActiveStreamsByPath.Remove(Stm.Path);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseStreamsWithStall(string Dir)
        {
            Logger.Log(LogLevel.Info, LogCategory.IO, "Stalling for directory close: '{0}'", Dir);

            lock (ActiveStreams)
            {
                for (int i = 0; i < ActiveStreams.Count; i++)
                {
                    ActiveStream Stm = ActiveStreams[i];
                    if (Stm.Path.StartsWith(Dir))
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.IO, "Closing stream for directory close: '{0}'", Stm.Path);

                        // This is not ideal, we should remove this if practical, it wastes processing time.
                        while (Stm.ActiveOperations > 0)
                        {
                            Thread.Sleep(0);
                        }

                        Stm.Stream.Close();
                        ActiveStreams.Remove(Stm);
                        ActiveStreamsByPath.Remove(Stm.Path);

                        i--;
                    }
                }
            }

            Logger.Log(LogLevel.Info, LogCategory.IO, "Finished closing directory: '{0}'", Dir);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private ActiveStream GetActiveStream(string Path, bool AllowPermissionFixes = true, bool RequireWrite = true)
        {
            ulong CurrentTicks = TimeUtils.Ticks;

            lock (ActiveStreams)
            {
                ActiveStream ExistingStream = null;
                if (ActiveStreamsByPath.ContainsKey(Path))
                {
                    ExistingStream = ActiveStreamsByPath[Path];

                    // If we need to write to this file and we are only open for read, drain event queue and reopen.
                    if (!ExistingStream.CanWrite && RequireWrite)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.IO, "Stalling while draining read queue and reopening for write: '{0}'", Path);

                        // This is not ideal, we should remove this if practical, it wastes processing time.
                        while (ExistingStream.ActiveOperations > 0)
                        {
                            Thread.Sleep(0);
                        }

                        ExistingStream.Stream.Close();
                        ActiveStreams.Remove(ExistingStream);
                        ActiveStreamsByPath.Remove(ExistingStream.Path);
                    }
                    else
                    {
                        ExistingStream.LastAccessed = CurrentTicks;
                        return ExistingStream;
                    }
                }

                // We need to remove one stream to max space.
                while (ActiveStreams.Count >= MaxStreams)
                {
                    ActiveStream OldestStream = null;

                    // Find oldest thats not doing anything.
                    foreach (ActiveStream Stm in ActiveStreams)
                    {
                        if (Stm.ActiveOperations == 0)
                        {
                            if (OldestStream == null || Stm.LastAccessed < OldestStream.LastAccessed)
                            {
                                OldestStream = Stm;
                            }
                        }
                    }

                    if (OldestStream == null)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.IO, "Closing stream for async queue (as we have reached maximum stream count): {0}", OldestStream.Path);

                        OldestStream.Stream.Close();

                        ActiveStreams.Remove(OldestStream);
                        ActiveStreamsByPath.Remove(OldestStream.Path);
                    }
                }

                try
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.IO, "Opening stream for async queue (can write = {0}): {1}", RequireWrite, Path);

                    string DirPath = System.IO.Path.GetDirectoryName(Path);
                    if (!Directory.Exists(DirPath))
                    {
                        Directory.CreateDirectory(DirPath);
                    }

                    ActiveStream NewStm = new ActiveStream();
                    NewStm.LastAccessed = CurrentTicks;
                    NewStm.Path = Path;
                    NewStm.CanWrite = RequireWrite;
#if DISABLE_IO_BUFFERING
                    FileOptions FileFlagNoBuffering = (FileOptions)0x20000000;
                    NewStm.Stream = new FileStream(Path, FileMode.OpenOrCreate, RequireWrite ? FileAccess.ReadWrite : FileAccess.Read, FileShare.ReadWrite, StreamBufferSize, FileOptions.Asynchronous | FileOptions.WriteThrough | FileFlagNoBuffering);
#else
                    NewStm.Stream = new FileStream(Path, FileMode.OpenOrCreate, RequireWrite ? FileAccess.ReadWrite : FileAccess.Read, FileShare.ReadWrite, StreamBufferSize, FileOptions.Asynchronous);
#endif
                    ActiveStreams.Add(NewStm);
                    ActiveStreamsByPath.Add(NewStm.Path, NewStm);

                    return NewStm;
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to open stream '{0}' with error {1}", Path, Ex.Message);

                    if (AllowPermissionFixes && TryFixFileAttributes(Path))
                    {
                        return GetActiveStream(Path, false, RequireWrite);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Stm"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        private void ReadWithOffset(ActiveStream Stm, Task Work, int Offset)
        {
            lock (Stm)
            {
                try
                {
                    Stm.Stream.Seek(Work.Offset + Offset, SeekOrigin.Begin);
                    Stm.Stream.BeginRead(
                        Work.Data, (int) Work.DataOffset + Offset, (int) Work.Size - Offset, Result =>
                        {
                            try
                            {
                                int BytesRead = Stm.Stream.EndRead(Result);
                                if (BytesRead == 0)
                                {
                                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to read file {0} at offset {1} size {2}. Encountered end of file.", Stm.Path, Work.Offset + Offset, Work.Size - Offset);
                                    Work.Callback?.Invoke(false);
                                }
                                else
                                {
                                    GlobalBandwidthStats.Out(BytesRead);

                                    if (BytesRead < Work.Size)
                                    {
                                        ReadWithOffset(Stm, Work, Offset + BytesRead);
                                        return;
                                    }

                                    ulong ElapsedTime = TimeUtils.Ticks - Work.QueueTime;
                                    OutLatency.Add(ElapsedTime);

                                    Work.Callback?.Invoke(true);
                                }
                            }
                            catch (Exception Ex)
                            {
                                Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to read file {0} with error: {1}", Stm.Path, Ex.Message);

                                Work.Callback?.Invoke(false);
                            }
                            finally
                            {
                                Interlocked.Add(ref GlobalQueuedOut, -Work.Size);
                                Interlocked.Decrement(ref Stm.ActiveOperations);

                                WakeThread();
                            }
                        }, null
                    );
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to read to file {0} with error: {1}", Stm.Path, Ex.Message);

                    Work.Callback?.Invoke(false);
                    Interlocked.Add(ref GlobalQueuedOut, -Work.Size);
                    Interlocked.Decrement(ref Stm.ActiveOperations);

                    WakeThread();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private bool IsPathLocked(string Path)
        {
            lock (LockedPaths)
            {
                foreach (string Dir in LockedPaths)
                {
                    if (Path.StartsWith(Dir))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        private void LockPath(string Path)
        {
            lock (LockedPaths)
            {
                LockedPaths.Add(Path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        private void UnlockPath(string Path)
        {
            lock (LockedPaths)
            {
                LockedPaths.Remove(Path);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Work"></param>
        private bool RunTask(Task Work)
        {
            ActiveStream Stm = null;
            if (Work.Type == TaskType.Write || Work.Type == TaskType.Read)
            {
                Stm = GetActiveStream(Work.Path, true, Work.Type == TaskType.Write);
                if (Stm == null)
                {
                    Work.Callback?.Invoke(false);
                    return true;
                }
            }

            // Limit number of ops per stream.
            if (Stm != null && Stm.ActiveOperations >= StreamMaxConcurrentOps)
            {
                return false;
            }

            // Should this be a failure or a deferal? Going to
            // go with failure now as the only thing we use path locking for is deleting files.
            if (IsPathLocked(Work.Path))
            {
                Work.Callback?.Invoke(false);
                return true;
            }

            if (Work.Type == TaskType.Write)
            {
                Interlocked.Increment(ref Stm.ActiveOperations);
                lock (Stm)
                {
#if EMULATE_IO
                    GlobalBandwidthStats.In(Work.Size);

                    Work.Callback?.Invoke(true);
                    Interlocked.Add(ref GlobalQueuedIn, -Work.Size);
                    Interlocked.Decrement(ref Stm.ActiveOperations);
#else

                    try
                    {
                        //Console.WriteLine("####### WRITING[offset={0} size={1}, stream={2}] {3}", Work.Offset, Work.Size, Stm.Stream.ToString(), Work.Path);

                        // TODO: Lock?
                        Stm.Stream.Seek(Work.Offset, SeekOrigin.Begin);
                        Stm.Stream.BeginWrite(
                            Work.Data, (int) Work.DataOffset, (int) Work.Size, Result =>
                            {
                                try
                                {
                                    GlobalBandwidthStats.In(Work.Size);

                                    Stm.Stream.EndWrite(Result);

                                    ulong ElapsedTime = TimeUtils.Ticks - Work.QueueTime;
                                    InLatency.Add(ElapsedTime);

                                    Work.Callback?.Invoke(true);
                                }
                                catch (Exception Ex)
                                {
                                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to write to file {0} with error: {1}", Stm.Path, Ex.Message);
                                    Work.Callback?.Invoke(false);
                                }
                                finally
                                {
                                    Interlocked.Add(ref GlobalQueuedIn, -Work.Size);
                                    Interlocked.Decrement(ref Stm.ActiveOperations);

                                    WakeThread();
                                }
                            }, null
                        );
                    }
                    catch (Exception Ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to write to file {0} with error: {1}", Stm.Path, Ex.Message);

                        Work.Callback?.Invoke(false);
                        Interlocked.Add(ref GlobalQueuedIn, -Work.Size);
                        Interlocked.Decrement(ref Stm.ActiveOperations);
                    }
#endif
                }
            }
            else if (Work.Type == TaskType.Read)
            {
                Interlocked.Increment(ref Stm.ActiveOperations);

                //Logger.Log(LogLevel.Info, LogCategory.IO, "Reading: offset={0} length={1} path={2}", Work.Offset, Work.Size, Work.Path);

#if EMULATE_IO
                GlobalBandwidthStats.Out(Work.Size);

                Work.Callback?.Invoke(true);
                Interlocked.Add(ref GlobalQueuedOut, -Work.Size);
                Interlocked.Decrement(ref Stm.ActiveOperations);
#else
                ReadWithOffset(Stm, Work, 0);
#endif
            }
            else if (Work.Type == TaskType.DeleteDir)
            {
                try
                {
                    CloseStreamsWithStall(Work.Path);

                    LockPath(Work.Path);

                    // Shifted into its own thread as it can take forever for large folders.
                    System.Threading.Tasks.Task.Run(
                        () =>
                        {
                            try
                            {
                                FileUtils.DeleteDirectory(Work.Path);
                                Work.Callback?.Invoke(true);
                            }
                            catch (Exception Ex)
                            {
                                Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to delete directory {0} with error: {1}", Work.Path, Ex.Message);
                                Work.Callback?.Invoke(false);
                            }
                            finally
                            {
                                UnlockPath(Work.Path);
                            }
                        }
                    );
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to delete directory {0} with error: {1}", Work.Path, Ex.Message);
                    Work.Callback?.Invoke(false);
                }
            }
            else if (Work.Type == TaskType.CloseStreamsInDir)
            {
                try
                {
                    CloseStreamsWithStall(Work.Path);
                    Work.Callback?.Invoke(true);
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to close streams in directory {0} with error: {1}", Work.Path, Ex.Message);
                    Work.Callback?.Invoke(false);
                }
            }

            return true;
        }

        /// <summary>
        /// </summary>
        private void ThreadEntry()
        {
            while (IsRunning)
            {
                Task NewTask;

                lock (WakeObject)
                {
                    if (TaskQueue.Count > 0)
                    {
                        if (!TaskQueue.TryDequeue(out NewTask))
                        {
                            continue;
                        }

                        Interlocked.Decrement(ref TaskQueueCount);
                    }
                    else
                    {
                        Monitor.Wait(WakeObject, 1000);
                        continue;
                    }

                    Interlocked.Increment(ref ActiveTasks);
                }

                Stopwatch watch = new Stopwatch();
                watch.Start();

                if (!RunTask(NewTask))
                {
                    lock (WakeObject)
                    {
                        Interlocked.Increment(ref TaskQueueCount);
                        TaskQueue.Enqueue(NewTask);
                        Monitor.Wait(WakeObject, 1000);
                    }
                }

                CloseOldStreams();
                Interlocked.Decrement(ref ActiveTasks);

                watch.Stop();
                TaskProcessTime.Add(watch.ElapsedMilliseconds);

                if (watch.ElapsedMilliseconds > 1000)
                {
                    Console.WriteLine("Took {0}ms to run: {1}", watch.ElapsedMilliseconds, NewTask.Type.ToString());
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private bool TryFixFileAttributes(string Path)
        {
            try
            {
                if (FileUtils.FileExistsFast(Path))
                {
                    FileAttributes attributes = File.GetAttributes(Path);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.IO, "Removing read-only flag on file '{0}'", Path);
                        File.SetAttributes(Path, attributes & ~FileAttributes.ReadOnly);
                        return true;
                    }
                }
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to fix permissions on file '{0}' with error {1}", Path, Ex.Message);
            }

            return false;
        }

        /// <summary>
        /// </summary>
        private void WakeThread()
        {
            lock (WakeObject)
            {
                Monitor.Pulse(WakeObject);
            }
        }

        /// <summary>
        /// </summary>
        ~AsyncIOQueue()
        {
            Stop();
        }
    }
}