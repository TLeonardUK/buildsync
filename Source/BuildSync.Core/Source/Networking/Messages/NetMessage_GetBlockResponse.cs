using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetBlockResponse : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        /// 
        /// </summary>
        public int BlockIndex;

        /// <summary>
        /// 
        /// </summary>
        public NetCachedArray Data = new NetCachedArray();

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
            serializer.Serialize(ref BlockIndex);

          //  Stopwatch totalstop = new Stopwatch();
          //  totalstop.Start();

            // TODO: Modify this so it stores a reference to the deserializing buffer
            // rather than doing a pointless and time consuming copy.
            serializer.Serialize(ref Data);

           // totalstop.Stop();
           // Logger.Log(LogLevel.Info, LogCategory.Transport, "Elapsed ms to {0} data: {1}", serializer.IsLoading ? "read" : "write", ((float)totalstop.ElapsedTicks / (Stopwatch.Frequency / 1000.0)));
        }

        internal override void Cleanup()
        {
            Data.SetNull();
        }

    }
}
