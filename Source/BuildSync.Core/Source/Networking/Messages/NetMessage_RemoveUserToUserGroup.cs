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

using BuildSync.Core.Users;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    ///     Client->Server
    ///
    ///     Requests a user is removed from the given usergroup.
    ///     Can only be sent by users who have the ModifyUsers
    ///     permission.
    /// </summary>
    public class NetMessage_RemoveUserFromUserGroup : NetMessage
    {
        /// <summary>
        ///     Name of usergroup to add to.
        /// </summary>
        public string GroupName = "";

        /// <summary>
        ///     Name of user to add to group.
        /// </summary>
        public string Username = "";

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref GroupName);
            serializer.Serialize(ref Username);
        }
    }
}