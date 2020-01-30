using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_Pong : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public ulong Timestamp = 0;

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref Timestamp);
        }
    }
}
