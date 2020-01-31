using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking
{
    /// <summary>
    ///     WARNING: Make sure you understand the alloc/release pattern for this class when recieved in netmessages, missues
    ///              will result in hard to find leaks and memory stomping as the buffers are recycled.
    /// </summary>
    public class NetCachedArray
    {
        public class Bucket
        {
            public int Size;
            public List<byte[]> Buffers = new List<byte[]>();
        }

        private static List<Bucket> Buckets = new List<Bucket>();
        private static long MemoryAllocated = 0;

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

        private byte[] AllocBuffer(int Size, bool FailIfNoMemory)
        {
            return MemoryPool.AllocBuffer(Size, FailIfNoMemory);
        }

        private void ReleaseBuffer(byte[] Buffer)
        {
            MemoryPool.ReleaseBuffer(Buffer);
        }

        public bool Resize(int NewSize, bool FailIfNoMemory)
        {
            if (Data != null && Data.Length < NewSize)
            {
                Length = NewSize;
                return true;
            }

            byte[] OldData = Data;
            Data = AllocBuffer(NewSize, FailIfNoMemory);
            if (Data == null)
            {
                return false;
            }
            if (OldData != null)
            {
                ReleaseBuffer(OldData);
            }
            Length = NewSize;
            return true;
        }

        public void SetNull()
        {
            if (Data != null)
            {
                ReleaseBuffer(Data);
                Data = null;
            }
        }
    }
}
