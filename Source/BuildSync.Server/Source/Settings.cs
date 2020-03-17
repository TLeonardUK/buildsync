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
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using BuildSync.Core.Manifests;

namespace BuildSync.Server
{
    /// <summary>
    ///     Encapsulates all persistent settings for the server that are serialized to disk.
    /// </summary>
    [Serializable]
    public class Settings : SettingsBase
    {
        /// <summary>
        ///     Dictionary of manifest-id's and the last date and time a client with a full copy of it was connected.
        /// </summary>
        public Dictionary<string, DateTime> ManifestLastSeenTimes { get; set; } = new Dictionary<string, DateTime>();

        /// <summary>
        ///     Maximum bandwidth across the entire network in bytes a second.
        /// </summary>
        public long MaxBandwidth { get; set; } = 0;

        /// <summary>
        ///     Maximum number of manifests the server should manage at one time. If this number is exceeded the oldest manifests will be pruned until under the limit.
        /// </summary>
        public int MaximumManifests { get; set; } = 500;

        /// <summary>
        ///     Maximum number of days that a manifest remains unseen on any clients before the manfiest no longer is managed by the server.
        /// </summary>
        public int MaximumManifestUnseenDays { get; set; } = 14;
        
        /// <summary>
        ///     If true the server will begin when the computer starts.
        /// </summary>
        public bool RunOnStartup { get; set; } = true;

        /// <summary>
        ///     Port the server will listen on for client connections.
        /// </summary>
        public int ServerPort { get; set; } = 12341;

        /// <summary>
        ///     Path on the local device that manifests and configuration data will be stored.
        /// </summary>
        public string StoragePath { get; set; } = "";

        /// <summary>
        ///     List of all users registered on the server and their associated user groups.
        /// </summary>
        public List<User> Users { get; set; } = new List<User>();

        /// <summary>
        ///     List of all user groups registered on the server and their associated permissions.
        /// </summary>
        public List<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

        /// <summary>
        ///     List of all build tags that have been registered on the server.
        /// </summary>
        public List<BuildManifestTag> Tags { get; set; } = new List<BuildManifestTag>();
    }
}