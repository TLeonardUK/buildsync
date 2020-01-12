using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Text;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void IOCompleteCallbackHandler(bool bSuccess);

    /// <summary>
    /// 
    /// </summary>
    public class AsyncIOQueue
    {
        public enum TaskType
        {
            Read,
            Write,
            DeleteDir
        }

        private struct Task
        {
            public TaskType Type;
            public string Path;
            public long Offset;
            public long Size;
            public long DataOffset;
            public byte[] Data;
            public IOCompleteCallbackHandler Callback;
        }

        public class ActiveStream
        {
            public bool CanWrite = false;
            public string Path;
            public Stream Stream;
            public ulong LastAccessed = 0;
            public int ActiveOperations = 0;
        }

        private ConcurrentQueue<Task> TaskQueue = new ConcurrentQueue<Task>();
        private Thread TaskThread = null;
        private bool IsRunning = false;
        private object WakeObject = new object();

        private const int MaxStreamAge = 10 * 1000;
        private const int MaxStreams = 16;
        private const int StreamBufferSize = 256 * 1024;
        private const int StreamMaxConcurrentOps = 8;

        private List<ActiveStream> ActiveStreams = new List<ActiveStream>();

        public BandwidthTracker BandwidthStats = new BandwidthTracker();

        private long InternalQueuedOut = 0;
        private long InternalQueuedIn = 0;

        private int ActiveTasks = 0;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return TaskQueue.IsEmpty && ActiveTasks == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long QueuedOut
        {
            get { return InternalQueuedOut; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long QueuedIn
        {
            get { return InternalQueuedIn; }
        }

        /// <summary>
        /// 
        /// </summary>
        public AsyncIOQueue()
        {
            IsRunning = true;

            TaskThread = new Thread(ThreadEntry);
            TaskThread.Name = "Async IO";
            TaskThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        ~AsyncIOQueue()
        {
            Stop();
        }

        /// <summary>
        /// 
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
                    }
                    else
                    {
                        Monitor.Wait(WakeObject, 1000);
                        continue;
                    }

                    Interlocked.Increment(ref ActiveTasks);
                }

                if (!RunTask(NewTask))
                {
                    TaskQueue.Enqueue(NewTask);
                }

                Interlocked.Decrement(ref ActiveTasks);

                CloseOldStreams();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void WakeThread()
        {
            lock (WakeObject)
            {
                Monitor.Pulse(WakeObject);
            }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private bool TryFixFileAttributes(string Path)
        {
            try
            {
                if (File.Exists(Path))
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
        /// 
        /// </summary>
        /// <returns></returns>
        private ActiveStream GetActiveStream(string Path, bool AllowPermissionFixes = true, bool RequireWrite = true)
        {
            lock (ActiveStreams)
            {
                foreach (ActiveStream Stm in ActiveStreams)
                {
                    if (Stm.Path == Path)
                    {
                        // If we need to write to this file and we are only open for read, drain event queue and reopen.
                        if (!Stm.CanWrite && RequireWrite)
                        {
                            while (Stm.ActiveOperations > 0)
                            {
                                Thread.Sleep(0);
                            }

                            Stm.Stream.Close();
                            ActiveStreams.Remove(Stm);
                            break;
                        }
                        else
                        {
                            Stm.LastAccessed = TimeUtils.Ticks;
                            return Stm;
                        }
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
                        Thread.Sleep(1);
                    }
                    else
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.IO, "Closing stream for async queue (as we have reached maximum stream count): {0}", OldestStream.Path);

                        OldestStream.Stream.Close();

                        ActiveStreams.Remove(OldestStream);
                    }
                }

                try
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.IO, "Opening stream for async queue (can write = {0}): {1}", RequireWrite, Path);

                    ActiveStream NewStm = new ActiveStream();
                    NewStm.LastAccessed = TimeUtils.Ticks;
                    NewStm.Path = Path;
                    NewStm.Stream = new FileStream(Path, FileMode.Open, RequireWrite ? FileAccess.ReadWrite : FileAccess.Read, FileShare.ReadWrite, StreamBufferSize, FileOptions.Asynchronous/* | FileOptions.WriteThrough*/);
                    ActiveStreams.Add(NewStm);

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
        /// 
        /// </summary>
        private void CloseOldStreams()
        {
            lock (ActiveStreams)
            {
                for (int i = 0; i < ActiveStreams.Count; i++)
                {
                    ActiveStream Stm = ActiveStreams[i];

                    ulong Elapsed = TimeUtils.Ticks - Stm.LastAccessed;
                    if (Elapsed > MaxStreamAge && Stm.ActiveOperations == 0)
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.IO, "Closing stream for async queue: {0}", Stm.Path);

                        Stm.Stream.Close();

                        ActiveStreams.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder"></param>
        public bool CloseAllStreamsInDirectory(string Folder)
        {
            bool AllClosed = true;

            lock (ActiveStreams)
            {
                for (int i = 0; i < ActiveStreams.Count; i++)
                {
                    ActiveStream Stm = ActiveStreams[i];
                    if (Stm.Path.StartsWith(Folder))
                    {
                        if (Stm.ActiveOperations == 0)
                        {
                            Logger.Log(LogLevel.Info, LogCategory.IO, "Force closing stream for async queue: {0}", Stm.Path);

                            Stm.Stream.Close();

                            ActiveStreams.RemoveAt(i);
                            i--;
                            continue;
                        }
                        else
                        {
                            AllClosed = false;
                        }
                    }
                }
            }

            return AllClosed;
        }

        /// <summary>
        /// 
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

            if (Work.Type == TaskType.Write)
            {
                Interlocked.Increment(ref Stm.ActiveOperations);
                lock (Stm)
                {
                    try
                    {
                        //Console.WriteLine("####### WRITING[offset={0} size={1}, stream={2}] {3}", Work.Offset, Work.Size, Stm.Stream.ToString(), Work.Path);

                        // TODO: Lock?
                        Stm.Stream.Seek(Work.Offset, SeekOrigin.Begin);
                        Stm.Stream.BeginWrite(Work.Data, (int)Work.DataOffset, (int)Work.Size, Result =>
                        {

                            try
                            {
                                BandwidthStats.BytesIn(Work.Size);

                                Stm.Stream.EndWrite(Result);

                                Work.Callback?.Invoke(true);
                            }
                            catch (Exception Ex)
                            {
                                Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to write to file {0} with error: {1}", Stm.Path, Ex.Message);
                                Work.Callback?.Invoke(false);
                            }
                            finally
                            {
                                Interlocked.Add(ref InternalQueuedIn, -Work.Size);
                                Interlocked.Decrement(ref Stm.ActiveOperations);
                            }

                        }, null);
                    }
                    catch (Exception Ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to write to file {0} with error: {1}", Stm.Path, Ex.Message);

                        Work.Callback?.Invoke(false);
                        Interlocked.Add(ref InternalQueuedIn, -Work.Size);
                        Interlocked.Decrement(ref Stm.ActiveOperations);
                    }
                }
            }
            else if (Work.Type == TaskType.Read)
            {
                Interlocked.Increment(ref Stm.ActiveOperations);
                ReadWithOffset(Stm, Work, 0);
            }
            else if (Work.Type == TaskType.DeleteDir)
            {
                try
                {
                    CloseAllStreamsInDirectory(Work.Path);

                    FileUtils.DeleteDirectory(Work.Path);

                    Work.Callback?.Invoke(true);
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to delete directory {0} with error: {1}", Work.Path, Ex.Message);
                    Work.Callback?.Invoke(false);
                }
            }

            return true;
        }

        /// <summary>
        /// 
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
                    Stm.Stream.BeginRead(Work.Data, (int)Work.DataOffset + Offset, (int)Work.Size - Offset, Result =>
                    {

                        try
                        {
                            int BytesRead = Stm.Stream.EndRead(Result);
                            BandwidthStats.BytesOut(BytesRead);

                            if (BytesRead < Work.Size)
                            {
                                ReadWithOffset(Stm, Work, Offset + BytesRead);
                                return;
                            }

                            Work.Callback?.Invoke(true);
                        }
                        catch (Exception Ex)
                        {
                            Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to read file {0} with error: {1}", Stm.Path, Ex.Message);

                            Work.Callback?.Invoke(false);
                        }
                        finally
                        {
                            Interlocked.Add(ref InternalQueuedOut, -Work.Size);
                            Interlocked.Decrement(ref Stm.ActiveOperations);
                        }

                    }, null);
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to read to file {0} with error: {1}", Stm.Path, Ex.Message);

                    Work.Callback?.Invoke(false);
                    Interlocked.Add(ref InternalQueuedOut, -Work.Size);
                    Interlocked.Decrement(ref Stm.ActiveOperations);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public void DeleteDir(string InPath, IOCompleteCallbackHandler InCallback)
        {
            TaskQueue.Enqueue(new Task { Type = TaskType.DeleteDir, Path = InPath, Callback = InCallback });
            WakeThread();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public void Write(string InPath, long InOffset, long InSize, byte[] InData, long InDataOffset, IOCompleteCallbackHandler InCallback)
        {
            Interlocked.Add(ref InternalQueuedIn, InSize);
            //Console.WriteLine("####### START WRITE[offset={0} size={1}] {2}", InOffset, InSize, InPath);

            TaskQueue.Enqueue(new Task { Type = TaskType.Write, Path = InPath, Offset = InOffset, Size = InSize, Data = InData, DataOffset = InDataOffset, Callback = InCallback });
            WakeThread();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public void Read(string InPath, long InOffset, long InSize, byte[] InData, long InDataOffset, IOCompleteCallbackHandler InCallback)
        {
            Interlocked.Add(ref InternalQueuedOut, InSize);
            TaskQueue.Enqueue(new Task { Type = TaskType.Read, Path = InPath, Offset = InOffset, Size = InSize, Data = InData, DataOffset = InDataOffset, Callback = InCallback });
            WakeThread();
        }
    }
}
