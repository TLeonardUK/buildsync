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
using System.Linq;
using System.Net;
using BuildSync.Core.Client;
using System.Collections.Generic;

namespace BuildSync.Core.Networking.Messages.RemoteActions
{
    /// <summary>
    /// </summary>
    public class NetMessage_SolicitRemoteAction : NetMessage
    {
        /// <summary>
        /// </summary>
        public Guid ActionId;

        /// <summary>
        /// 
        /// </summary>
        public RemoteActionType Type;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Settings = new Dictionary<string, string>();

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ActionId);
            serializer.SerializeEnum(ref Type);
            serializer.Serialize(ref Settings);
        }
    }
}