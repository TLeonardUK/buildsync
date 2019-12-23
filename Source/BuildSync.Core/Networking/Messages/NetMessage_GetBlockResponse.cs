using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetBlockResponse : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        /// 
        /// </summary>
        public int BlockIndex;

        /// <summary>
        /// 
        /// </summary>
        public byte[] Data;

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
            serializer.Serialize(ref BlockIndex);
            serializer.Serialize(ref Data);
        }
    }
}
