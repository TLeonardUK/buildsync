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

using BuildSync.Core.Utils;

namespace BuildSync.Core
{
    /// <summary>
    ///     Version and flavour of the application.
    ///     Parts of this class are auto-generated so care should be taken when modifying it.
    /// </summary>
    public class AppVersion
    {
        // AUTO GENERATION
        public static int BuildVersion = 613;
        public static int MajorVersion = 1;
        public static int MinorVersion = 0;
        public static int PatchVersion = 0;

        public static int ProtocolVersion = 7;
        // END AUTO GENERATION

        public static string VersionString = MajorVersion + "." + MinorVersion + "." + PatchVersion + "." + BuildVersion;
        public static int VersionNumber = StringUtils.ConvertSemanticVerisonNumber(VersionString);

        /// <summary>
        ///     Determines if all licensing ui/limits are removed.
        /// </summary>
        public static bool NonLicensed = true;

        /// <summary>
        ///     Determines if error tracing / console display is shown.
        /// </summary>
        public static bool Trace = false;
    }
}