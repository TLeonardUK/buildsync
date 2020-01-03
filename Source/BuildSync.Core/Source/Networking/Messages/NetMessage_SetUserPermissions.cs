using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core;
using BuildSync.Core.Users;

namespace BuildSync.Core.Networking.Messages
{
    public class NetMessage_SetUserPermissions : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string Username = "";

        /// <summary>
        /// 
        /// </summary>
        public UserPermissionCollection Permissions = new UserPermissionCollection();

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref Username);

            if (serializer.IsLoading)
            {
                Permissions = new UserPermissionCollection();
            }
            Permissions.Serialize(serializer);
        }
    }
}
