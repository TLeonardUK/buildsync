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
    public class NetMessage_ClientStateUpdate : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public long UploadRate = 0;

        /// <summary>
        /// 
        /// </summary>
        public long DownloadRate = 0;

        /// <summary>
        /// 
        /// </summary>
        public long TotalUploaded = 0;

        /// <summary>
        /// 
        /// </summary>
        public long TotalDownloaded = 0;

        /// <summary>
        /// 
        /// </summary>
        public int ConnectedPeerCount = 0;

        /// <summary>
        /// 
        /// </summary>
        public long DiskUsage = 0;

        /// <summary>
        /// 
        /// </summary>
        public string Version = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref UploadRate);
            serializer.Serialize(ref DownloadRate);
            serializer.Serialize(ref TotalUploaded);
            serializer.Serialize(ref TotalDownloaded);
            serializer.Serialize(ref ConnectedPeerCount);
            serializer.Serialize(ref DiskUsage);
            serializer.Serialize(ref Version);
        }
    }
}
