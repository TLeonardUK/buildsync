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

using System.Collections.Generic;
using System.Diagnostics;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking
{
    /// <summary>
    ///     WARNING: Make sure you understand the alloc/release pattern for this class when recieved in netmessages, missues
    ///     will result in hard to find leaks and memory stomping as the buffers are recycled.
    /// </summary>
    public class NetCachedArray
    {
        public class Bucket
        {
            public List<byte[]> Buffers = new List<byte[]>();
            public int Size;
        }

        private static readonly List<Bucket> Buckets = new List<Bucket>();

        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NewSize"></param>
        /// <param name="Capacity"></param>
        /// <param name="FailIfNoMemory"></param>
        /// <returns></returns>
        public bool Resize(int NewSize, int Capacity, bool FailIfNoMemory)
        {
            if (Data != null && Data.Length < NewSize)
            {
                Length = NewSize;
                return true;
            }

            Debug.Assert(NewSize <= Capacity);

            byte[] OldData = Data;
            Data = AllocBuffer(Capacity, FailIfNoMemory);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="FailIfNoMemory"></param>
        /// <returns></returns>
        private byte[] AllocBuffer(int Size, bool FailIfNoMemory)
        {
            return MemoryPool.AllocBuffer(Size, FailIfNoMemory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
        private void ReleaseBuffer(byte[] Buffer)
        {
            MemoryPool.ReleaseBuffer(Buffer);
        }
    }
}