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
using BuildSync.Core.Manifests;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// </summary>
    public class NetMessage_GetBlockResponse : NetMessage
    {
        /// <summary>
        /// </summary>
        public int BlockIndex;

        /// <summary>
        /// </summary>
        public NetCachedArray Data = new NetCachedArray();

        /// <summary>
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        /// 
        /// </summary>
        public override bool DoesRecieverHandleCleanup => true;

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
            serializer.Serialize(ref BlockIndex);

            //  Stopwatch totalstop = new Stopwatch();
            //  totalstop.Start();

            // TODO: Modify this so it stores a reference to the deserializing buffer
            // rather than doing a pointless and time consuming copy.
            if (!serializer.Serialize(ref Data, (int) BuildManifest.BlockSize, true))
            {
            }

            // totalstop.Stop();
            // Logger.Log(LogLevel.Info, LogCategory.Transport, "Elapsed ms to {0} data: {1}", serializer.IsLoading ? "read" : "write", ((float)totalstop.ElapsedTicks / (Stopwatch.Frequency / 1000.0)));
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void Cleanup()
        {
            Data.SetNull();
        }
    }
}