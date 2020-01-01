using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_ConnectionInfo : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint PeerConnectionAddress = new IPEndPoint(IPAddress.Any, 0);

        /// <summary>
        /// 
        /// </summary>
        public string Username = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            string Address = PeerConnectionAddress.Address.ToString();
            int Port = PeerConnectionAddress.Port;
            serializer.Serialize(ref Address);
            serializer.Serialize(ref Port);
            PeerConnectionAddress = new IPEndPoint(IPAddress.Parse(Address), Port);

            serializer.Serialize(ref Username);
        }
    }
}
