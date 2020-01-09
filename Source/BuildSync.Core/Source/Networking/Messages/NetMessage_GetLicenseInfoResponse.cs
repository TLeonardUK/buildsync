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
    public class NetMessage_GetLicenseInfoResponse : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public License License = new License();

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref License.LicensedTo);
            serializer.Serialize(ref License.ExpirationTime);
            serializer.Serialize(ref License.MaxSeats);
        }
    }
}
