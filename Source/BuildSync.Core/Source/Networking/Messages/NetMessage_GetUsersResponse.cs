/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System.Collections.Generic;
using BuildSync.Core.Users;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// </summary>
    public class NetMessage_GetUsersResponse : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public override bool HasLargePayload => true;

        /// <summary>
        /// </summary>
        public List<User> Users = new List<User>();

        /// <summary>
        /// </summary>
        public List<UserGroup> UserGroups = new List<UserGroup>();

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            // Serialize users.
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

                int GroupCount = Users[i].Groups.Count;
                serializer.Serialize(ref GroupCount);
                for (int j = 0; j < GroupCount; j++)
                {
                    if (serializer.IsLoading)
                    {
                        Users[i].Groups.Add("");
                    }

                    string GroupName = Users[i].Groups[j];
                    serializer.Serialize(ref GroupName);
                    Users[i].Groups[j] = GroupName;
                }
            }
            
            // Serialize usergroups
            Count = UserGroups.Count;
            serializer.Serialize(ref Count);

            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    UserGroups.Add(new UserGroup());
                }

                string Name = UserGroups[i].Name;
                serializer.Serialize(ref Name);

                UserGroups[i].Name = Name;
                UserGroups[i].Permissions.Serialize(serializer);
            }
        }
    }
}