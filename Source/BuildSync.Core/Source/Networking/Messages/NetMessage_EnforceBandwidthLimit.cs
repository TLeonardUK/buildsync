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
using BuildSync.Core.Routes;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    ///     Server->Client
    ///
    ///     Sent by the server to enforce a global bandwidth limit on the client. This
    ///     is used by the server for congestion management.
    /// </summary>
    public class NetMessage_EnforceBandwidthLimit : NetMessage
    {
        /// <summary>
        ///     Maximum global bandwidth client should use in bytes per second.
        /// </summary>
        public long BandwidthLimitGlobal;

        /// <summary>
        ///     Maximum bandwidth per route
        /// </summary>
        public List<RoutePair> BandwidthLimitRoutes = new List<RoutePair>();

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref BandwidthLimitGlobal);

            int Count = BandwidthLimitRoutes.Count;
            serializer.Serialize(ref Count);
            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    BandwidthLimitRoutes.Add(new RoutePair());
                }

                RoutePair Pair = BandwidthLimitRoutes[i];

                serializer.Serialize(ref Pair.Bandwidth);
                serializer.Serialize(ref Pair.DestinationTagId);
                serializer.Serialize(ref Pair.SourceTagId);

                BandwidthLimitRoutes[i] = Pair;
            }
        }
    }
}