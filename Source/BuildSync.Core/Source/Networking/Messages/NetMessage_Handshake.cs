﻿/*
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

using System;
using System.Collections.Generic;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    /// </summary>
    public class NetMessage_Handshake : NetMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public int Version = AppVersion.ProtocolVersion;

        /// <summary>
        /// 
        /// </summary>
        public List<Guid> TagIds = new List<Guid>();

        /// <summary>
        ///     Fully qualified name of user using the tool (eg. MY-DOMAIN\Username).
        /// </summary>
        public string Username = "";

        /// <summary>
        ///     Name of the machine.
        /// </summary>
        public string MachineName = "";

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref Version);

            // Serialize users.
            int Count = TagIds.Count;
            serializer.Serialize(ref Count);

            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    TagIds.Add(new Guid());
                }

                Guid value = TagIds[i];
                serializer.Serialize(ref value);
                TagIds[i] = value;
            }

            if (serializer.Version > 100000584)
            {
                serializer.Serialize(ref Username);
            }

            if (serializer.Version >= 100000690 && Version >= 10)
            {
                serializer.Serialize(ref MachineName);
            }
        }
    }
}