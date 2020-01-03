using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core;
using BuildSync.Core.Users;

namespace BuildSync.Core.Networking.Messages
{
    public class NetMessage_DeleteUser : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string Username = "";

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref Username);
        }
    }
}
