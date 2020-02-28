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

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    ///     Server->Client
    ///
    ///     Sent by a server in response to a <see cref="NetMessage_GetBuilds" /> 
    ///     providing the data that was requested by the original sender.
    /// </summary>
    public class NetMessage_GetBuildsResponse : NetMessage
    {
        /// <summary>
        ///     Represents an individual build returned by this message.
        /// </summary>
        public struct BuildInfo
        {
            /// <summary>
            ///     Full path within the manifest registry of this build.
            /// </summary>
            public string VirtualPath;

            /// <summary>
            ///     Unique id of this build manifest.
            /// </summary>
            public Guid Guid;
            
            /// <summary>
            ///     Time that build was added to the manifest registry.
            /// </summary>
            public DateTime CreateTime;

            /// <summary>
            ///     Size of entire build.
            /// </summary>
            public ulong TotalSize;

            /// <summary>
            ///     How many peers are connected with parts of this build.
            /// </summary>
            public ulong AvailablePeers;

            /// <summary>
            ///     Last time a copy of this build was seen available.
            /// </summary>
            public DateTime LastSeenOnPeer;
        }

        /// <summary>
        ///     Array of all child builds that were retrieved.
        /// </summary>
        public BuildInfo[] Builds = new BuildInfo[0];

        /// <summary>
        ///     Parent path from which all child builds were retrieved.
        /// </summary>
        public string RootPath = "";

        /// <summary>
        ///     If true the old format message without availability information will be sent.
        /// </summary>
        public bool SendLegacyVersion = false;

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            int BuildCount = Builds.Length;
            serializer.Serialize(ref RootPath);
            serializer.Serialize(ref BuildCount);

            Array.Resize(ref Builds, BuildCount);

            for (int i = 0; i < BuildCount; i++)
            {
                serializer.Serialize(ref Builds[i].VirtualPath);
                serializer.Serialize(ref Builds[i].Guid);
                serializer.Serialize(ref Builds[i].CreateTime);

                if (!SendLegacyVersion)
                {
                    serializer.Serialize(ref Builds[i].TotalSize);
                    serializer.Serialize(ref Builds[i].AvailablePeers);
                    serializer.Serialize(ref Builds[i].LastSeenOnPeer);
                }
            }
        }
    }
}