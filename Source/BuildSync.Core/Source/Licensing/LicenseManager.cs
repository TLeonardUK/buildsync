using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

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
