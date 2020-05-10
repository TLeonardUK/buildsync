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
    /// </summary>
    public class NetMessage_GetServerStateResponse : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public class ClientState
        {
            public string Address;
            public int ConnectedPeerCount;
            public long DiskUsage;
            public long DiskQuota;
            public long DownloadRate;
            public long TotalDownloaded;
            public long TotalUploaded;
            public long UploadRate;
            public string Version;
            public string Username;
            public List<Guid> TagIds = new List<Guid>();
        }

        /// <summary>
        /// </summary>
        public long BandwidthLimit;

        /// <summary>
        /// </summary>
        public List<ClientState> ClientStates = new List<ClientState>();

        /// <summary>
        /// 
        /// </summary>
        public override bool HasLargePayload => true;

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref BandwidthLimit);

            int Count = ClientStates.Count;
            serializer.Serialize(ref Count);

            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    ClientStates.Add(new ClientState());
                }

                if (serializer.Version > 100000584)
                {
                    serializer.Serialize(ref ClientStates[i].Username);
                }

                serializer.Serialize(ref ClientStates[i].Address);
                serializer.Serialize(ref ClientStates[i].DownloadRate);
                serializer.Serialize(ref ClientStates[i].UploadRate);
                serializer.Serialize(ref ClientStates[i].TotalDownloaded);
                serializer.Serialize(ref ClientStates[i].TotalUploaded);
                serializer.Serialize(ref ClientStates[i].ConnectedPeerCount);
                serializer.Serialize(ref ClientStates[i].DiskUsage);
                serializer.Serialize(ref ClientStates[i].Version);

                if (serializer.Version >= 100000613)
                {
                    serializer.Serialize(ref ClientStates[i].DiskQuota);
                }

                int TagCount = ClientStates[i].TagIds.Count;
                serializer.Serialize(ref TagCount);

                for (int j = 0; j < TagCount; j++)
                {
                    if (serializer.IsLoading)
                    {
                        ClientStates[i].TagIds.Add(new Guid());
                    }
                    
                    Guid id = ClientStates[i].TagIds[j];
                    serializer.Serialize(ref id);
                    ClientStates[i].TagIds[j] = id;
                }
            }
        }
    }
}