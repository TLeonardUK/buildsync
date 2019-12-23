using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Text;

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
        private struct Task
        {
            public bool IsWrite;
            public string Path;
            public long Offset;
            public long Size;
            public byte[] Data;
            public IOCompleteCallbackHandler Callback;
        }

        public class ActiveStream
        {
            public string Path;
            public Stream Stream;
            public int LastAccessed = 0;
            public int ActiveOperations = 0;
        }

        private ConcurrentQueue<Task> TaskQueue = new ConcurrentQueue<Task>();
        private Thread TaskThread = null;
        private bool IsRunning = false;
        private object WakeObject = new object();

        private const int MaxStreamAge = 10 * 1000;

        private List<ActiveStream> ActiveStreams = new List<ActiveStream>();

        public BandwidthTracker BandwidthStats = new BandwidthTracker();

        private long InternalQueuedOut = 0;
        private long InternalQueuedIn = 0;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return TaskQueue.IsEmpty;
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
                }

                RunTask(NewTask);
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
            IsRunning = false;
            TaskThread.Join();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ActiveStream GetActiveStream(string Path)
        {
            lock (ActiveStreams)
            {
                foreach (ActiveStream Stm in ActiveStreams)
                {
                    if (Stm.Path == Path)
                    {
                        Stm.LastAccessed = Environment.TickCount;
                        return Stm;
                    }
                }

                try
                {
                    Console.WriteLine("Opening stream for async queue: {0}", Path);

                    ActiveStream NewStm = new ActiveStream();
                    NewStm.LastAccessed = Environment.TickCount;
                    NewStm.Path = Path;
                    NewStm.Stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 1 * 1024 * 1024, FileOptions.Asynchronous/* | FileOptions.WriteThrough*/);
                    ActiveStreams.Add(NewStm);

                    return NewStm;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Failed to open stream '{0}' with error {1}", Path, Ex.Message);
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

                    int Elapsed = Environment.TickCount - Stm.LastAccessed;
                    if (Elapsed > MaxStreamAge && Stm.ActiveOperations == 0)
                    {
                        Console.WriteLine("Closing stream for async queue: {0}", Stm.Path);

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
                            Console.WriteLine("Force closing stream for async queue: {0}", Stm.Path);

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
        private void RunTask(Task Work)
        {
            ActiveStream Stm = GetActiveStream(Work.Path);
            if (Stm == null)
            {
                return;
            }

            Interlocked.Increment(ref Stm.ActiveOperations);

            if (Work.IsWrite)
            {
                Stm.Stream.Seek(Work.Offset, SeekOrigin.Begin);
                Stm.Stream.BeginWrite(Work.Data, 0, (int) Work.Size, Result => {

                    try
                    {
                        BandwidthStats.BytesIn(Work.Size);

                        Stm.Stream.EndWrite(Result);

                        Work.Callback?.Invoke(true);
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("Failed to write to file {0} with error: {1}", Stm.Path, Ex.Message);
                        Work.Callback?.Invoke(false);
                    }
                    finally
                    {
                        Interlocked.Add(ref InternalQueuedOut, -Work.Size);
                        Interlocked.Decrement(ref Stm.ActiveOperations);
                        Stm.LastAccessed = Environment.TickCount;
                    }

                }, null);
            }
            else
            {
                ReadWithOffset(Stm, Work, 0);
            }
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
            Stm.Stream.Seek(Work.Offset + Offset, SeekOrigin.Begin);
            Stm.Stream.BeginRead(Work.Data, Offset, (int)Work.Size - Offset, Result => {

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
                    Interlocked.Add(ref InternalQueuedIn, -Work.Size);
                    Interlocked.Decrement(ref Stm.ActiveOperations);
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Failed to read file {0} with error: {1}", Stm.Path, Ex.Message);

                    Work.Callback?.Invoke(false);
                    Interlocked.Add(ref InternalQueuedIn, -Work.Size);
                    Interlocked.Decrement(ref Stm.ActiveOperations);
                }
                finally
                {
                    Stm.LastAccessed = Environment.TickCount;
                }

            }, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public void Write(string InPath, long InOffset, long InSize, byte[] InData, IOCompleteCallbackHandler InCallback)
        {
            Interlocked.Add(ref InternalQueuedOut, InSize);
            TaskQueue.Enqueue(new Task { IsWrite = true, Path = InPath, Offset = InOffset, Size = InSize, Data = InData, Callback = InCallback });
            WakeThread();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public void Read(string InPath, long InOffset, long InSize, byte[] InData, IOCompleteCallbackHandler InCallback)
        {
            Interlocked.Add(ref InternalQueuedIn, InSize);
            TaskQueue.Enqueue(new Task { IsWrite = false, Path = InPath, Offset = InOffset, Size = InSize, Data = InData, Callback = InCallback });
            WakeThread();
        }
    }
}
