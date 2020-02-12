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
using BuildSync.Core.Utils;
using System;
using System.Collections.Generic;

namespace BuildSync.Server
{
    [Serializable]
    public class Settings : SettingsBase
    {
        public int ServerPort { get; set; } = 12341;
        public string StoragePath { get; set; } = "";
        public int MaximumManifests { get; set; } = 500;
        public int MaximumManifestUnseenDays { get; set; } = 30;
        public bool RunOnStartup { get; set; } = true;
        public List<User> Users { get; set; } = new List<User>();
        public long MaxBandwidth { get; set; } = 0;
        public Dictionary<string, DateTime> ManifestLastSeenTimes { get; set; } = new Dictionary<string, DateTime>();
    }
}
