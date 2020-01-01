using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Networking
{
    /// <summary>
    /// 
    /// </summary>
    public class NetCachedArray
    {
        public class Bucket
        {
            public int Size;
            public List<byte[]> Buffers = new List<byte[]>();
        }

        private static List<Bucket> Buckets = new List<Bucket>();

        public byte[] Data
        {
            get;
            internal set;
        }

        public int Length
        {
            get;
            internal set;
        }

        private byte[] AllocBuffer(int Size)
        {
            lock (Buckets)
            {
                while (true)
                {
                    foreach (Bucket bucket in Buckets)
                    {
                        if (Size < bucket.Size)
                        {
                            if (bucket.Buffers.Count > 0)
                            {
                                byte[] Result = bucket.Buffers[bucket.Buffers.Count - 1];
                                bucket.Buffers.RemoveAt(bucket.Buffers.Count - 1);
                                return Result;
                            }
                            else
                            {
                                byte[] Result = new byte[bucket.Size];
                                return Result;
                            }
                        }
                    }

                    int NextBucketSize = (Buckets.Count > 0) ? (Buckets[Buckets.Count - 1].Size * 2) : 128;
                    Buckets.Add(new Bucket() { Size = NextBucketSize });
                }
            }
        }

        private void ReleaseBuffer(byte[] Buffer)
        {
            lock (Buckets)
            {
                foreach (Bucket bucket in Buckets)
                {
                    if (Buffer.Length == bucket.Size)
                    {
                        bucket.Buffers.Add(Buffer);
                        return;
                    }
                }

                Debug.Assert(false);
            }
        }

        public void Resize(int NewSize)
        {
            if (Data != null && Data.Length < NewSize)
            {
                Length = NewSize;
                return;
            }

            byte[] OldData = Data;
            Data = AllocBuffer(NewSize);
            if (OldData != null)
            {
                ReleaseBuffer(OldData);
            }
            Length = NewSize;
        }

        public void SetNull()
        {
            if (Data != null)
            {
                ReleaseBuffer(Data);
                Data = null;
            }
        }

        ~NetCachedArray()
        {
            SetNull();
        }
    }
}
