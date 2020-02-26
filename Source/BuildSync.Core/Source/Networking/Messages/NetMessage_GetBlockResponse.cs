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
    ///     Client->Client
    ///
    ///     Sent by a client in response to a <see cref="NetMessage_GetBlock" /> 
    ///     providing the data that was requested by the original sender.
    /// </summary>
    public class NetMessage_GetBlockResponse : NetMessage
    {
        /// <summary>
        ///     Block index of data contained.
        /// </summary>
        public int BlockIndex;

        /// <summary>
        ///     Id of manifest that block originated from.
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        ///     Buffer containing the blocks data.
        /// </summary>
        public NetCachedArray Data = new NetCachedArray();

        /// <summary>
        ///     Gets or sets if the reciver handles calling the Cleanup function at an appropriate time. If false
        ///     the Cleanup function will be called as soon as the message handler has returned.
        /// </summary>
        public override bool DoesRecieverHandleCleanup => true;

        /// <summary>
        ///     Gets or sets that this message can get large enough that no attempts should be made to fit it into small message buffers..
        /// </summary>
        public override bool HasLargePayload => true;

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
            serializer.Serialize(ref BlockIndex);

            // TODO: Modify this so it stores a reference to the deserializing buffer
            // rather than doing a pointless and time consuming copy.
            if (!serializer.Serialize(ref Data, (int) BuildManifest.BlockSize, true))
            {
            }
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