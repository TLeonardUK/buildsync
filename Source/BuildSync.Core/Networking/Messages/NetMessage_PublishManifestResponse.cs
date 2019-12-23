using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public enum PublishManifestResult
    {
        Failed,
        Success,
        VirtualPathAlreadyExists,
        GuidAlreadyExists
    }

    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_PublishManifestResponse : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        /// 
        /// </summary>
        public PublishManifestResult Result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
            serializer.SerializeEnum(ref Result);
        }
    }
}
