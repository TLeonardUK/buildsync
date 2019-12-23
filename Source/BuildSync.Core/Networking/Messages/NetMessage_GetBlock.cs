using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetBlock : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        /// 
        /// </summary>
        public int BlockIndex;

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
            serializer.Serialize(ref BlockIndex);
        }
    }
}
