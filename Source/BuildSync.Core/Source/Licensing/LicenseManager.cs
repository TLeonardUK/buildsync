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
using System.IO;

namespace BuildSync.Core.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    public class LicenseManager
    {
        /// <summary>
        /// 
        /// </summary>
        private License FreeLicense = null;

        /// <summary>
        /// 
        /// </summary>
        public License ActiveLicense { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private string LicensePath = "";

        /// <summary>
        /// 
        /// </summary>
        public LicenseManager()
        {
            FreeLicense = new License();
            FreeLicense.LicensedTo = "Trial License";
            FreeLicense.MaxSeats = 3;
            FreeLicense.ExpirationTime = License.InfiniteExpirationTime;

            ActiveLicense = FreeLicense;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public void Start(string Path)
        {
            LicensePath = Path;

            if (File.Exists(Path))
            {
                ActiveLicense = License.Load(Path);
                if (ActiveLicense == null)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Licensing, "Failed to load license file: {0}", Path);
                    ActiveLicense = FreeLicense;
                }
                if (ActiveLicense.IsExpired)
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Licensing, "License file has expired: {0}", Path);
                    return;
                }
            }

            PrintLicenseInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrintLicenseInfo()
        {
            Logger.Log(LogLevel.Info, LogCategory.Licensing, "Active License:");
            Logger.Log(LogLevel.Info, LogCategory.Licensing, "\tLicensed to: {0}", ActiveLicense.LicensedTo);
            Logger.Log(LogLevel.Info, LogCategory.Licensing, "\tMax seats: {0}", ActiveLicense.MaxSeats);
            Logger.Log(LogLevel.Info, LogCategory.Licensing, "\tExpiration date: {0}", ActiveLicense.ExpirationTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public bool Apply(License license)
        {
            if (!license.Verify())
            {
                Logger.Log(LogLevel.Warning, LogCategory.Licensing, "Unable to apply license, not verified.");
                return false;
            }

            if (ActiveLicense.IsExpired)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Licensing, "Unable to apply license, expired.");
                return false;
            }

            ActiveLicense = license;

            PrintLicenseInfo();

            license.Save(LicensePath);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            if (ActiveLicense != FreeLicense && ActiveLicense.IsExpired)
            {
                Logger.Log(LogLevel.Warning, LogCategory.Licensing, "License has now expired.");
                ActiveLicense = FreeLicense;
            }
        }
    }
}
