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
using BuildSync.Core.Tags;

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

            /// <summary>
            /// 
            /// </summary>
            public Tag[] Tags;
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

                if (serializer.Version > 100000397)
                {
                    serializer.Serialize(ref Builds[i].TotalSize);
                    serializer.Serialize(ref Builds[i].AvailablePeers);
                    serializer.Serialize(ref Builds[i].LastSeenOnPeer);
                }
                if (serializer.Version > 100000546)
                {
                    if (Builds[i].Tags == null)
                    {
                        Builds[i].Tags = new Tag[0];
                    }

                    int TagCount = Builds[i].Tags.Length;
                    serializer.Serialize(ref TagCount);

                    if (serializer.IsLoading)
                    {
                        Builds[i].Tags = new Tag[TagCount];
                    }

                    for (int j = 0; j < TagCount; j++)
                    {
                        if (serializer.IsLoading)
                        {
                            Builds[i].Tags[j] = new Tag();
                        }
                        Builds[i].Tags[j].Serialize(serializer);
                    }
                }
            }
        }
    }
}