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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Routes
{
    /// <summary>
    /// </summary>
    public struct RoutePair
    {
        public Guid SourceTagId;
        public Guid DestinationTagId;
        public long Bandwidth;

        public override bool Equals(object x)
        {
            if (!(x is RoutePair))
            {
                return false;
            }

            RoutePair pair = (RoutePair)x;

            return SourceTagId == pair.SourceTagId && 
                   DestinationTagId == pair.DestinationTagId;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + SourceTagId.GetHashCode();
            hash = hash * 31 + DestinationTagId.GetHashCode();
            return hash;
        }
    }
    
    /// <summary>
    /// </summary>
    [Serializable]
    public class Route
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// </summary>
        public Guid SourceTagId { get; set; } = Guid.Empty;

        /// <summary>
        /// </summary>
        public Guid DestinationTagId { get; set; } = Guid.Empty;

        /// <summary>
        /// </summary>
        public bool Blacklisted { get; set; } = false;

        /// <summary>
        /// </summary>
        public long BandwidthLimit { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        public void Serialize(Networking.NetMessageSerializer serializer)
        {
            {
                Guid Value = Id;
                serializer.Serialize(ref Value);
                Id = Value;
            }
            {
                Guid Value = SourceTagId;
                serializer.Serialize(ref Value);
                SourceTagId = Value;
            }
            {
                Guid Value = DestinationTagId;
                serializer.Serialize(ref Value);
                DestinationTagId = Value;
            }
            {
                bool Value = Blacklisted;
                serializer.Serialize(ref Value);
                Blacklisted = Value;
            }
            {
                long Value = BandwidthLimit;
                serializer.Serialize(ref Value);
                BandwidthLimit = Value;
            }
        }
    }
}