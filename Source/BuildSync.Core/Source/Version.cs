using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core
{
    public class AppVersion
    {
        public static int MajorVersion = 1;
        public static int MinorVersion = 0;
        public static int PatchVersion = 0;

        // AUTO GENERATION
        public static int BuildVersion = 111;
        // END AUTO GENERATION

        public static int VersionNumber = (MajorVersion * 100000000) + (MinorVersion * 1000000) + (PatchVersion * 10000) + BuildVersion;
        public static string VersionString = MajorVersion + "." + MinorVersion + "." + PatchVersion + "." + BuildVersion;

        public static int ProtocolVersion = 2;
    }
}
