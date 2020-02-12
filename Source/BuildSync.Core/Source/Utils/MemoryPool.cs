/*
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
using System.Threading;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// </summary>
    public class Statistic_MemoryBlocksFree : Statistic
    {
        public Statistic_MemoryBlocksFree()
        {
            Name = @"Memory\Blocks Free";
            MaxLabel = "512";
            MaxValue = 512.0f;
            DefaultShown = true;
        }

        public override void Gather()
        {
            AddSample(MemoryPool.BlocksFree);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MemoryPool
    {
        public class Bucket
        {
            public List<byte[]> Buffers = new List<byte[]>();
            public int Size;
            public int TotalAllocated;
        }

        private static readonly List<Bucket> Buckets = new List<Bucket>();
        private static long MemoryAllocated;

        /// <summary>
        /// 
        /// </summary>
        public static int BlocksFree
        {
            get
            {
                int Result = 0;
                lock (Buckets)
                {
                    foreach (Bucket bucket in Buckets)
                    {
                        Result += bucket.Buffers.Count;
                    }
                }

                return Result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="FailIfNoMemory"></param>
        /// <returns></returns>
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

                            if (FailIfNoMemory)
                            {
                                return null;
                            }

                            {
                                byte[] Result = new byte[bucket.Size];
                                bucket.TotalAllocated++;
                                Interlocked.Add(ref MemoryAllocated, bucket.Size);

                                return Result;
                            }
                        }
                    }

                    int NextBucketSize = Buckets.Count > 0 ? Buckets[Buckets.Count - 1].Size * 2 : 128;
                    Buckets.Add(new Bucket {Size = NextBucketSize});
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="Count"></param>
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

                    int NextBucketSize = Buckets.Count > 0 ? Buckets[Buckets.Count - 1].Size * 2 : 128;
                    Buckets.Add(new Bucket {Size = NextBucketSize});
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
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
    }
}