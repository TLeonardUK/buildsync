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
        public List<UserPermission> Permissions = new List<UserPermission>(); 

        /// <summary>
        /// 
        /// </summary>
        public NetCachedArray Data = new NetCachedArray();

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            int Count = Permissions.Count;
            serializer.Serialize(ref Count);
            for (int i = 0; i < Count; i++)
            {
                UserPermission Permission = UserPermission.Unknown;
                if (!serializer.IsLoading)
                {
                    Permission = Permissions[i];
                    serializer.SerializeEnum<UserPermission>(ref Permission);
                }
                else
                {
                    serializer.SerializeEnum<UserPermission>(ref Permission);
                    Permissions.Add(Permission);
                }
            }
        }
    }
}
