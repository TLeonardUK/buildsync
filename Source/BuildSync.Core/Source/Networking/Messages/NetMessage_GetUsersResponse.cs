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
    public class NetMessage_GetUsersResponse : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public List<User> Users = new List<User>();

        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            int Count = Users.Count;
            serializer.Serialize(ref Count);

            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    Users.Add(new User());
                }

                string Username = Users[i].Username;
                serializer.Serialize(ref Username);

                Users[i].Username = Username;
                Users[i].Permissions.Serialize(serializer);
            }
        }
    }
}
