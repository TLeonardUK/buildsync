using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetBuilds : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string RootPath = "";

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref RootPath);
        }
    }
}
