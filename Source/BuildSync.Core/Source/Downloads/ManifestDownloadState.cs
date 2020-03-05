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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BuildSync.Core.Manifests;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class ManifestDownloadStateCollection
    {
        /// <summary>
        /// </summary>
        public List<ManifestDownloadState> States { get; set; } = new List<ManifestDownloadState>();
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class ManifestFileCompletedState
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime ModifiedTimestampOnCompleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; } = "";
    }

    /// <summary>
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ManifestDownloadProgressState
    {
        Unknown,
        RetrievingManifest,
        Initializing,
        DeltaCopying,
        Downloading,
        Complete,
        Validating,
        DiskError,
        InitializeFailed,
        ValidationFailed,
        Installing,
        InstallFailed
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class ManifestDownloadState
    {
        /// <summary>
        /// </summary>
        [NonSerialized]
        public RateTracker BandwidthStats = new RateTracker(30);

        /// <summary>
        /// </summary>
        [NonSerialized]
        public RateTracker InitializeRateStats = new RateTracker(30);

        /// <summary>
        /// </summary>
        [NonSerialized]
        public RateTracker DeltaCopyRateStats = new RateTracker(30);

        /// <summary>
        /// </summary>
        public string InstallDeviceName = "";

        /// <summary>
        /// </summary>
        public string InstallLocation = "";

        /// <summary>
        /// </summary>
        public bool Installed = false;

        /// <summary>
        /// </summary>
        public bool InstallOnComplete = false;

        /// <summary>
        /// </summary>
        [NonSerialized]
        public ulong LastManifestRequestTime = 0;

        /// <summary>
        /// </summary>
        [NonSerialized]
        public BuildManifest Manifest;

        /// <summary>
        /// </summary>
        [NonSerialized]
        public RateTracker ValidateRateStats = new RateTracker(30);

        /// <summary>
        /// </summary>
        [NonSerialized]
        internal bool DiskError = false;

        /// <summary>
        /// </summary>
        [NonSerialized]
        internal Task InitializeTask = null;

        /// <summary>
        /// </summary>
        [NonSerialized]
        internal Task DeltaCopyTask = null;

        /// <summary>
        /// </summary>
        [NonSerialized]
        internal Task InstallTask = null;

        /// <summary>
        /// </summary>
        [NonSerialized]
        internal Task ValidationTask = null;

        /// <summary>
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// </summary>
        public SparseStateArray BlockStates { get; set; } = new SparseStateArray();

        /// <summary>
        /// </summary>
        [JsonIgnore]
        public long BytesRemaining
        {
            get
            {
                if (Manifest == null)
                {
                    return 0;
                }

                long TotalSize = Manifest.GetTotalSize();
                long Downloaded = Manifest.GetTotalSizeOfBlocks(BlockStates);

                return TotalSize - Downloaded;
            }
        }

        /// <summary>
        /// </summary>
        public List<ManifestFileCompletedState> FileCompletedStates { get; set; } = new List<ManifestFileCompletedState>();

        /// <summary>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonIgnore]
        public long InitializeBytesRemaining { get; set; }

        /// <summary>
        /// </summary>
        [JsonIgnore]
        public float InitializeProgress { get; internal set; }
        
        /// <summary>
        /// </summary>
        [JsonIgnore]
        public long DeltaCopyBytesRemaining { get; set; }

        /// <summary>
        /// </summary>
        [JsonIgnore]
        public float DeltaCopyProgress { get; internal set; }

        /// <summary>
        /// </summary>
        public DateTime LastActive { get; set; }

        /// <summary>
        /// </summary>
        public string LocalFolder { get; set; }

        /// <summary>
        /// </summary>
        public Guid ManifestId { get; set; }

        /// <summary>
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// </summary>
        [JsonIgnore]
        public float Progress
        {
            get
            {
                if (BlockStates == null || BlockStates.Size == 0)
                {
                    return 0;
                }

                float BlocksRetrieved = BlockStates.Count(true);
                return BlocksRetrieved / BlockStates.Size;
            }
        }

        /// <summary>
        /// </summary>
        public ManifestDownloadProgressState State { get; set; }

        /// <summary>
        /// </summary>
        [JsonIgnore]
        public long ValidateBytesRemaining { get; set; }

        /// <summary>
        /// </summary>
        [JsonIgnore]
        public float ValidateProgress { get; internal set; }
    }
}