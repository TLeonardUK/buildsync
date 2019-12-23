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
    public class NetMessage_RelevantPeerListUpdate : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public List<IPEndPoint> PeerAddresses = new List<IPEndPoint>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            int Count = PeerAddresses.Count;
            serializer.Serialize(ref Count);

            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    PeerAddresses.Add(new IPEndPoint(IPAddress.Any, 0));
                }

                string AddressStr = PeerAddresses[i].Address.ToString();
                serializer.Serialize(ref AddressStr);

                PeerAddresses[i].Address = IPAddress.Parse(AddressStr);

                int Port = PeerAddresses[i].Port;
                serializer.Serialize(ref Port);
                PeerAddresses[i].Port = Port;
            }
        }
    }
}
