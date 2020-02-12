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
using System.Text;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetBuildsResponse : NetMessage
    {
        public struct BuildInfo
        {
            public string VirtualPath;
            public System.Guid Guid;
            public DateTime CreateTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public string RootPath = "";

        /// <summary>
        /// 
        /// </summary>
        public BuildInfo[] Builds = new BuildInfo[0];

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
            }
        }
    }
}
