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

using BuildSync.Core.Downloads;
using BuildSync.Core.Scm;
using BuildSync.Core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BuildSync.Client
{
    [Serializable]
    public class StoredLaunchSettings
    {
        public Guid ManifestId { get; set; } = Guid.Empty;
        public Dictionary<string, string> Values = new Dictionary<string, string>();
    }

    [Serializable]
    public class ScmWorkspaceSettings
    {
        public ScmProviderType ProviderType { get; set; } = ScmProviderType.Perforce;
        public string Server { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Location { get; set; } = "";
    }

    [Serializable]
    public class PeerSettingsRecord
    {
        public string Address { get; set; } = "";
        public long TotalIn { get; set; } = 0;
        public long TotalOut { get; set; } = 0;
        public float AverageRateIn { get; set; } = 0;
        public float AverageRateOut { get; set; } = 0;
        public float PeakRateIn { get; set; } = 0;
        public float PeakRateOut { get; set; } = 0;
        public DateTime LastSeen { get; set; } = DateTime.Now;
        public float TargetInFlightData { get; set; } = 0;
        public float CurrentInFlightData { get; set; } = 0;
        public long Ping { get; set; } = 0;
        public long BestPing { get; set; } = 0;

        internal long TotalInTracker = -1;
        internal long TotalOutTracker = -1;
    }

    [Serializable]
    public class Settings : SettingsBase
    {
        public string ServerHostname { get; set; } = "localhost";
        public int ServerPort { get; set; } = 12341;
        public string StoragePath { get; set; } = "";
        public long StorageMaxSize { get; set; } = 1024L * 1024L * 1024L * 1024L;
        public long BandwidthMaxUp { get; set; } = 0;
        public long BandwidthMaxDown { get; set; } = 0;
        public int BandwidthStartTimeHour { get; set; } = 0;
        public int BandwidthStartTimeMin { get; set; } = 0;
        public int BandwidthEndTimeHour { get; set; } = 0;
        public int BandwidthEndTimeMin { get; set; } = 0;
        public int ClientPortRangeMin { get; set; } = 12342;
        public int ClientPortRangeMax { get; set; } = 12352;
        public bool FirstRun { get; set; } = true;
#if SHIPPING
        public bool MinimizeToTrayOnClose { get; set; } = true;
#else
        public bool MinimizeToTrayOnClose { get; set; } = false;
#endif
        public bool AlwaysRunInstallBeforeLaunching { get; set; } = true;
        public bool RunOnStartup { get; set; } = true;

        public byte[] LayoutState { get; set; } = null;
        public Size LayoutSize { get; set; } = Size.Empty;

        public List<string> ActiveStatistics { get; set; } = new List<string>();

        public bool SkipValidation { get; set; } = false;
        public bool SkipDiskAllocation { get; set; } = false;
        public bool ShowInternalDownloads { get; set; } = false;

        public List<PeerSettingsRecord> PeerRecords { get; set; } = new List<PeerSettingsRecord>();

        public List<ScmWorkspaceSettings> ScmWorkspaces { get; set; } = new List<ScmWorkspaceSettings>();

        public const int MaxLaunchSettings = 10;
        public List<StoredLaunchSettings> LaunchSettings { get; set; } = new List<StoredLaunchSettings>();

        public DownloadStateCollection DownloadStates { get; set; } = new DownloadStateCollection();
        public ManifestDownloadStateCollection ManifestDownloadStates { get; set; } = new ManifestDownloadStateCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public PeerSettingsRecord GetOrCreatePeerRecord(string Address)
        {
            foreach (PeerSettingsRecord Record in PeerRecords)
            {
                if (Record.Address == Address)
                {
                    return Record;
                }
            }

            PeerSettingsRecord NewRecord = new PeerSettingsRecord();
            NewRecord.Address = Address;
            PeerRecords.Add(NewRecord);

            return NewRecord;
        }
    }
}
