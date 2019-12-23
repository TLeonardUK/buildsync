using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Downloads;

namespace BuildSync.Client
{
    [Serializable]
    public class Settings : SettingsBase
    {
        public string ServerHostname = "localhost";
        public int ServerPort = 12341;
        public string StoragePath = "";
        public long StorageMaxSize = 1024l * 1024l * 1024l * 1024l;
        public long BandwidthMaxUp = 0;
        public long BandwidthMaxDown = 0;
        public int BandwidthStartTimeHour = 0;
        public int BandwidthStartTimeMin = 0;
        public int BandwidthEndTimeHour = 0;
        public int BandwidthEndTimeMin = 0;
        public int ClientPortRangeMin = 12342;
        public int ClientPortRangeMax = 12352;
        public bool FirstRun = true;
        public DownloadStateCollection DownloadStates = new DownloadStateCollection();
        public ManifestDownloadStateCollection ManifestDownloadStates = new ManifestDownloadStateCollection();
    }
}
