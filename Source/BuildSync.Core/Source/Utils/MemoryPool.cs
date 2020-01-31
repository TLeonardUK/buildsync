using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BuildSync.Core.Utils
{
    public static class MemoryPool
    {
        public class Bucket
        {
            public int Size;
            public List<byte[]> Buffers = new List<byte[]>();
            public int TotalAllocated = 0;
        }

        private static List<Bucket> Buckets = new List<Bucket>();
        private static long MemoryAllocated = 0;

        public static void PreallocateBuffers(int Size, int Count)
        {
            lock (Buckets)
            {
                while (true)
                {
                    foreach (Bucket bucket in Buckets)
                    {
                        if (Size <= bucket.Size)
                        {
                            for (int i = 0; i < Count; i++)
                            {
                                byte[] Result = new byte[bucket.Size];
                                bucket.Buffers.Add(Result);
                                bucket.TotalAllocated++;
                                Interlocked.Add(ref MemoryAllocated, bucket.Size);
                            }

                            return;
                        }
                    }

                    int NextBucketSize = (Buckets.Count > 0) ? (Buckets[Buckets.Count - 1].Size * 2) : 128;
                    Buckets.Add(new Bucket() { Size = NextBucketSize });
                }
            }
        }

        public static byte[] AllocBuffer(int Size, bool FailIfNoMemory = false)
        {
            lock (Buckets)
            {
                while (true)
                {
                    foreach (Bucket bucket in Buckets)
                    {
                        if (Size <= bucket.Size)
                        {
                            if (bucket.Buffers.Count > 0)
                            {
                                byte[] Result = bucket.Buffers[bucket.Buffers.Count - 1];
                                bucket.Buffers.RemoveAt(bucket.Buffers.Count - 1);

                                //PrintStatus();
                                return Result;
                            }
                            else
                            {
                                if (FailIfNoMemory)
                                {
                                    return null;
                                }
                                else
                                {
                                    byte[] Result = new byte[bucket.Size];
                                    bucket.TotalAllocated++;
                                    Interlocked.Add(ref MemoryAllocated, bucket.Size);

                                    return Result;
                                }
                            }
                        }
                    }

                    int NextBucketSize = (Buckets.Count > 0) ? (Buckets[Buckets.Count - 1].Size * 2) : 128;
                    Buckets.Add(new Bucket() { Size = NextBucketSize });
                }
            }
        }

        public static void ReleaseBuffer(byte[] Buffer)
        {
            lock (Buckets)
            {
                foreach (Bucket bucket in Buckets)
                {
                    if (Buffer.Length == bucket.Size)
                    {
                        bucket.Buffers.Add(Buffer);

                        //PrintStatus();
                        return;
                    }
                }

                Debug.Assert(false);
            }
        }

        public static void PrintStatus()
        {
            Console.WriteLine("======= MEMORY POOL ========");
            lock (Buckets)
            {
                foreach (Bucket bucket in Buckets)
                {
                    if (bucket.TotalAllocated > 0)
                    {
                        Console.WriteLine("Size={0} Count={1} Free={2} Size={3}", StringUtils.FormatAsSize(bucket.Size), bucket.TotalAllocated, bucket.Buffers.Count, StringUtils.FormatAsSize(bucket.Size * bucket.TotalAllocated));
                    }
                }
            }
        }
    }
}
