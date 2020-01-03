using System;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Users;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class NetMessage_PermissionUpdate : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public UserPermissionCollection Permissions = new UserPermissionCollection();

        /// <summary>
        /// 
        /// </summary>
        public NetCachedArray Data = new NetCachedArray();

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            if (serializer.IsLoading)
            {
                Permissions = new UserPermissionCollection();
            }
            Permissions.Serialize(serializer);
        }
    }
}
