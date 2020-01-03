using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Users;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_GetUsers : NetMessage
    {
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
        }
    }
}
