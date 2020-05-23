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

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    ///     Client->Server
    ///
    ///     Periodically sent to server to inform server about the senders current status.
    /// </summary>
    public class NetMessage_ClientStateUpdate : NetMessage
    {
        /// <summary>
        ///     Numbers of peers the sender is currently connected to.
        /// </summary>
        public int ConnectedPeerCount;

        /// <summary>
        ///     The amount of disk space in bytes the sender is currently using to store builds.
        /// </summary>
        public long DiskUsage;

        /// <summary>
        ///     The amount of disk space allocated for storing builds.
        /// </summary>
        public long DiskQuota;

        /// <summary>
        ///     The rate in bytes per second the sender is currently downloading data.
        /// </summary>
        public long DownloadRate;

        /// <summary>
        ///     The rate in bytes per second the sender is currently uploading data.
        /// </summary>
        public long UploadRate;

        /// <summary>
        ///     The total amount in bytes that the sender has downloaded this session.
        /// </summary>
        public long TotalDownloaded;

        /// <summary>
        ///     The total amount in bytes that the sender has uploaded this session.
        /// </summary>
        public long TotalUploaded;

        /// <summary>
        ///     Semantic version of the client application the sender is currently running.
        /// </summary>
        public string Version = "";

        /// <summary>
        ///     If this client allows remote actions to run on it.
        /// </summary>
        public bool AllowRemoteActions = false;

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref UploadRate);
            serializer.Serialize(ref DownloadRate);
            serializer.Serialize(ref TotalUploaded);
            serializer.Serialize(ref TotalDownloaded);
            serializer.Serialize(ref ConnectedPeerCount);
            serializer.Serialize(ref DiskUsage);
            serializer.Serialize(ref Version);

            if (serializer.Version >= 100000613)
            {
                serializer.Serialize(ref DiskQuota);
            }

            if (serializer.Version >= 100000683)
            {
                serializer.Serialize(ref AllowRemoteActions);
            }
        }
    }
}