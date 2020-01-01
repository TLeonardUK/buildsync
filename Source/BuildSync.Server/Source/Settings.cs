﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Downloads;
using BuildSync.Core.Users;

namespace BuildSync.Server
{
    [Serializable]
    public class Settings : SettingsBase
    {
        public int ServerPort { get; set; } = 12341;
        public string StoragePath { get; set; } = "";
        public int MaximumManifests { get; set; } = 500;
        public bool RunOnStartup { get; set; } = true;
        public List<User> Users { get; set; } = new List<User>();
    }
}
