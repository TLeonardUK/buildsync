using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetServerMaxBandwidth : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public long BandwidthLimit = 0;

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref BandwidthLimit);
        }
    }
}
