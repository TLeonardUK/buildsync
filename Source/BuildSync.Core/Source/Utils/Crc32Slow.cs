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
using Crc32C;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// </summary>
    /// <param name="BytesProcessed"></param>
    public delegate void ChecksumProgressEventHandler(long BytesProcessed);

    // Based on: https://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc32.cs
    /// <summary>
    ///     Implements a 32-bit CRC hash algorithm compatible with Zip etc.
    /// </summary>
    /// <remarks>
    ///     Crc32 should only be used for backward compatibility with older file formats
    ///     and algorithms. It is not secure enough for new applications.
    ///     If you need to call multiple times for the same data either use the HashAlgorithm
    ///     interface or remember that the result of one Compute call needs to be ~ (XOR) before
    ///     being passed in as the seed for the next Compute call.
    /// </remarks>
    public sealed class Crc32Slow : HashAlgorithm
    {
        public const uint DefaultPolynomial = 0xedb88320u;
        public const uint DefaultSeed = 0xffffffffu;
        public const int SparseStepInterval = 100;
        private static uint[] defaultTable;
        private uint hash;
        private readonly uint seed;
        private readonly uint[] table;

        public override int HashSize => 32;

        public Crc32Slow()
            : this(DefaultPolynomial, DefaultSeed)
        {
        }

        public Crc32Slow(uint polynomial, uint seed)
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new PlatformNotSupportedException("Not supported on Big Endian processors");
            }

            table = InitializeTable(polynomial);
            this.seed = hash = seed;
        }

        public static uint Compute(byte[] buffer)
        {
            return Compute(buffer, buffer.Length);
        }

        public static uint Compute(byte[] buffer, int Length)
        {
            return Compute(DefaultSeed, buffer, Length);
        }

        public static uint Compute(uint seed, byte[] buffer, int Length)
        {
            return Compute(DefaultPolynomial, seed, buffer, Length);
        }

        public static uint Compute(uint polynomial, uint seed, byte[] buffer, int Length)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, Length);
        }

        public static uint ComputeSparse(byte[] buffer)
        {
            return ComputeSparse(buffer, buffer.Length);
        }

        public static uint ComputeSparse(byte[] buffer, int Length)
        {
            return ComputeSparse(DefaultSeed, buffer, Length);
        }

        public static uint ComputeSparse(uint seed, byte[] buffer, int Length)
        {
            return ComputeSparse(DefaultPolynomial, seed, buffer, Length);
        }

        public static uint ComputeSparse(uint polynomial, uint seed, byte[] buffer, int Length)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, Length, Length / SparseStepInterval); 
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
            byte[] hashBuffer = UInt32ToBigEndianBytes(~hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        private static uint CalculateHash(uint[] table, uint seed, IList<byte> buffer, int start, int size, int step = 1)
        {
            uint hash = seed;
            for (int i = start; i < start + size; i += step)
            {
                hash = (hash >> 8) ^ table[buffer[i] ^ (hash & 0xff)];
            }
            return hash;
        }

        private static uint[] InitializeTable(uint polynomial)
        {
            if (polynomial == DefaultPolynomial && defaultTable != null)
            {
                return defaultTable;
            }

            uint[] createTable = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                uint entry = (uint) i;
                for (int j = 0; j < 8; j++)
                {
                    if ((entry & 1) == 1)
                    {
                        entry = (entry >> 1) ^ polynomial;
                    }
                    else
                    {
                        entry >>= 1;
                    }
                }

                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
            {
                defaultTable = createTable;
            }

            return createTable;
        }

        private static byte[] UInt32ToBigEndianBytes(uint uint32)
        {
            byte[] result = BitConverter.GetBytes(uint32);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(result);
            }

            return result;
        }
    }

    // Fast implementation of CRC32.
    public sealed class Crc32Fast : Crc32CAlgorithm
    {
        private static uint[] defaultTable;
        private uint hash;
        private readonly uint seed;
        private readonly uint[] table;

        public Crc32Fast()
        {
        }

        /*public static uint ComputeSparse(byte[] buffer)
        {
            return ComputeSparse(buffer, 0, buffer.Length);
        }

        public static uint ComputeSparse(byte[] buffer, int Offset, int Length)
        {
            for (int i = 0; i < Length )

        }*/
    }
}