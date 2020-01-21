using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Licensing;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetServerStateResponse : NetMessage
    {
        public class ClientState
        {
            public string Address;
            public long DownloadRate;
            public long UploadRate;
            public long TotalDownloaded;
            public long TotalUploaded;
            public int ConnectedPeerCount;
            public long DiskUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<ClientState> ClientStates = new List<ClientState>();

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            int Count = ClientStates.Count;
            serializer.Serialize(ref Count);

            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    ClientStates.Add(new ClientState());
                }

                serializer.Serialize(ref ClientStates[i].Address);
                serializer.Serialize(ref ClientStates[i].DownloadRate);
                serializer.Serialize(ref ClientStates[i].UploadRate);
                serializer.Serialize(ref ClientStates[i].TotalDownloaded);
                serializer.Serialize(ref ClientStates[i].TotalUploaded);
                serializer.Serialize(ref ClientStates[i].ConnectedPeerCount);
                serializer.Serialize(ref ClientStates[i].DiskUsage);
            }
        }
    }
}
