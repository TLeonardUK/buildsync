using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_Ping : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public ulong Timestamp = TimeUtils.Ticks;

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref Timestamp);
        }
    }
}
