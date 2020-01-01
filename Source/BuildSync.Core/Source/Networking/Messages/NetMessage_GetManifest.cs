using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetManifest : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId;

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
        }
    }
}
