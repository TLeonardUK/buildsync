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
    public sealed class SHA1Extended : SHA1Managed
    {
        public const int BufferSize = 256 * 1024;

        /// <summary>
        /// </summary>
        /// <param name="Algo"></param>
        public byte[] ComputeLargeStreamHash(Stream Stream, RateTracker Tracker, ChecksumProgressEventHandler Callback)
        {
            // Buffer size optimized for reading massive files.
            byte[] buffer = MemoryPool.AllocBuffer(BufferSize);
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
            byte[] Tmp = (byte[]) HashValue.Clone();
            Initialize();

            MemoryPool.ReleaseBuffer(buffer);
            return Tmp;
        }
    }
}