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
using System.IO;
using System.Security.Cryptography;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="BytesProcessed"></param>
    public delegate void ChecksumProgressEventHandler(long BytesProcessed);

    // Based on: https://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc32.cs
    /// <summary>
    /// Implements a 32-bit CRC hash algorithm compatible with Zip etc.
    /// </summary>
    /// <remarks>
    /// Crc32 should only be used for backward compatibility with older file formats
    /// and algorithms. It is not secure enough for new applications.
    /// If you need to call multiple times for the same data either use the HashAlgorithm
    /// interface or remember that the result of one Compute call needs to be ~ (XOR) before
    /// being passed in as the seed for the next Compute call.
    /// </remarks>
    public sealed class Crc32 : HashAlgorithm
    {
        public const UInt32 DefaultPolynomial = 0xedb88320u;
        public const UInt32 DefaultSeed = 0xffffffffu;

        public const int BufferSize = 256 * 1024;

        static UInt32[] defaultTable;

        readonly UInt32 seed;
        readonly UInt32[] table;
        UInt32 hash;

        public Crc32()
            : this(DefaultPolynomial, DefaultSeed)
        {
        }

        public Crc32(UInt32 polynomial, UInt32 seed)
        {
            if (!BitConverter.IsLittleEndian)
                throw new PlatformNotSupportedException("Not supported on Big Endian processors");

            table = InitializeTable(polynomial);
            this.seed = hash = seed;
        }

        public override void Initialize()
        {
            hash = seed;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            hash = CalculateHash(table, hash, array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt32ToBigEndianBytes(~hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        public override int HashSize { get { return 32; } }

        public static UInt32 Compute(byte[] buffer)
        {
            return Compute(buffer, buffer.Length);
        }

        public static UInt32 Compute(byte[] buffer, int Length)
        {
            return Compute(DefaultSeed, buffer, Length);
        }

        public static UInt32 Compute(UInt32 seed, byte[] buffer, int Length)
        {
            return Compute(DefaultPolynomial, seed, buffer, Length);
        }

        public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer, int Length)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, Length);
        }

        static UInt32[] InitializeTable(UInt32 polynomial)
        {
            if (polynomial == DefaultPolynomial && defaultTable != null)
                return defaultTable;

            var createTable = new UInt32[256];
            for (var i = 0; i < 256; i++)
            {
                var entry = (UInt32)i;
                for (var j = 0; j < 8; j++)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry >>= 1;
                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
                defaultTable = createTable;

            return createTable;
        }

        static UInt32 CalculateHash(UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size)
        {
            var hash = seed;
            for (var i = start; i < start + size; i++)
                hash = (hash >> 8) ^ table[buffer[i] ^ hash & 0xff];
            return hash;
        }

        static byte[] UInt32ToBigEndianBytes(UInt32 uint32)
        {
            var result = BitConverter.GetBytes(uint32);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Algo"></param>
        public byte[] ComputeLargeStreamHash(Stream Stream, RateTracker Tracker, ChecksumProgressEventHandler Callback)
        {
            // Buffer size optimized for reading massive files.
            byte[] buffer = MemoryPool.AllocBuffer(BufferSize, false);
            int bytesRead;
            do
            {
                bytesRead = Stream.Read(buffer, 0, BufferSize);
                if (bytesRead > 0)
                {
                    HashCore(buffer, 0, bytesRead);

                    if (Callback != null)
                    {
                        Callback?.Invoke(bytesRead);
                    }

                    if (Tracker != null)
                    {
                        Tracker.Out(bytesRead);
                    }
                }
            } while (bytesRead > 0);

            HashValue = HashFinal();
            byte[] Tmp = (byte[])HashValue.Clone();
            Initialize();

            MemoryPool.ReleaseBuffer(buffer);
            return (Tmp);
        }
    }
}
