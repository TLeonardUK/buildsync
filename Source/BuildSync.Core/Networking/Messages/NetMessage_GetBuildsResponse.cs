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
