using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_PublishManifest : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        /// 
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
            serializer.Serialize(ref Data);
        }
    }
}
