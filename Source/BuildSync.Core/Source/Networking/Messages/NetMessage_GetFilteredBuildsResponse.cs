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
    ///     Sent by a server in response to a <see cref="NetMessage_GetFilteredBuilds" /> 
    ///     providing the data that was requested by the original sender.
    /// </summary>
    public class NetMessage_GetFilteredBuildsResponse : NetMessage
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
        }

        /// <summary>
        ///     Array of all child builds that were retrieved.
        /// </summary>
        public BuildInfo[] Builds = new BuildInfo[0];

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            int BuildCount = Builds.Length;
            serializer.Serialize(ref BuildCount);

            Array.Resize(ref Builds, BuildCount);

            for (int i = 0; i < BuildCount; i++)
            {
                serializer.Serialize(ref Builds[i].VirtualPath);
                serializer.Serialize(ref Builds[i].Guid);
            }
        }
    }
}