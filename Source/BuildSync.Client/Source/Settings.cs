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
using System.Drawing;
using BuildSync.Core.Downloads;
using BuildSync.Core.Scm;
using BuildSync.Core.Utils;

namespace BuildSync.Client
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class StoredLaunchSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Values = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId { get; set; } = Guid.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ScmWorkspaceSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string Location { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public ScmProviderType ProviderType { get; set; } = ScmProviderType.Perforce;
        
        /// <summary>
        /// 
        /// </summary>
        public string Server { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; } = "";
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PeerSettingsRecord
    {
        /// <summary>
        /// 
        /// </summary>
        internal long TotalInTracker = -1;

        /// <summary>
        /// 
        /// </summary>
        internal long TotalOutTracker = -1;

        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public float AverageRateIn { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public float AverageRateOut { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long BestPing { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public float CurrentInFlightData { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastSeen { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        public float PeakRateIn { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public float PeakRateOut { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long Ping { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public float TargetInFlightData { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long TotalIn { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long TotalOut { get; set; } = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Settings : SettingsBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string ServerHostname { get; set; } = "localhost";

        /// <summary>
        /// 
        /// </summary>
        public int ServerPort { get; set; } = 12341;

        /// <summary>
        /// 
        /// </summary>
        public string StoragePath { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public long StorageMaxSize { get; set; } = 1024L * 1024L * 1024L * 1024L;

        /// <summary>
        /// 
        /// </summary>
        public long BandwidthMaxUp { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long BandwidthMaxDown { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int BandwidthStartTimeHour { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int BandwidthStartTimeMin { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int BandwidthEndTimeHour { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int BandwidthEndTimeMin { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int ClientPortRangeMin { get; set; } = 12342;

        /// <summary>
        /// 
        /// </summary>
        public int ClientPortRangeMax { get; set; } = 12352;

        /// <summary>
        /// 
        /// </summary>
        public bool FirstRun { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
#if SHIPPING
        public bool MinimizeToTrayOnClose { get; set; } = true;
#else
        public bool MinimizeToTrayOnClose { get; set; } = false;
#endif

        /// <summary>
        /// 
        /// </summary>
        public bool AlwaysRunInstallBeforeLaunching { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool RunOnStartup { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public byte[] LayoutState { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public Size LayoutSize { get; set; } = Size.Empty;

        /// <summary>
        /// 
        /// </summary>
        public List<string> ActiveStatistics { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public bool SkipValidation { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool SkipDiskAllocation { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool AutoFixValidationErrors { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool ShowInternalDownloads { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public LogLevel LoggingLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// 
        /// </summary>
        public List<PeerSettingsRecord> PeerRecords { get; set; } = new List<PeerSettingsRecord>();

        /// <summary>
        /// 
        /// </summary>
        public List<ScmWorkspaceSettings> ScmWorkspaces { get; set; } = new List<ScmWorkspaceSettings>();

        /// <summary>
        /// 
        /// </summary>
        public const int MaxLaunchSettings = 10;

        /// <summary>
        /// 
        /// </summary>
        public List<StoredLaunchSettings> LaunchSettings { get; set; } = new List<StoredLaunchSettings>();

        /// <summary>
        /// 
        /// </summary>
        public DownloadStateCollection DownloadStates { get; set; } = new DownloadStateCollection();

        /// <summary>
        /// 
        /// </summary>
        public ManifestDownloadStateCollection ManifestDownloadStates { get; set; } = new ManifestDownloadStateCollection();

        /// <summary>
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