using System;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_BlockListUpdate : NetMessage
    {        
        /// <summary>
        /// 
        /// </summary>
        public BlockListState BlockState = new BlockListState();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            BlockState.Serialize(serializer);
        }
    }
}
