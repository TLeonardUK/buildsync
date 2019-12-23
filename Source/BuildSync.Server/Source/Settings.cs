using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Downloads;

namespace BuildSync.Server
{
    [Serializable]
    public class Settings : SettingsBase
    {
        public int ServerPort = 12341;
        public string StoragePath = "";
    }
}
