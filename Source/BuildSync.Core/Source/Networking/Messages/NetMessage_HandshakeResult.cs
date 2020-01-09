using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public enum HandshakeResultType
    { 
        Success,
        InvalidVersion,
        MaxSeatsExceeded,
        Unknown
    }

    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_HandshakeResult : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public HandshakeResultType ResultType = HandshakeResultType.Unknown;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.SerializeEnum<HandshakeResultType>(ref ResultType);
        }
    }
}
