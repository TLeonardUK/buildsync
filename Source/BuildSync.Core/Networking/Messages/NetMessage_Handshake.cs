using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_Handshake : NetMessage
    {
        public int Version = NetConnection.ProtocolVersion;

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref Version);
        }
    }
}
