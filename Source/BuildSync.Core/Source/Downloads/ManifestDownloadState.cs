using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BuildSync.Core.Manifests;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ManifestDownloadStateCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ManifestDownloadState> States { get; set; } = new List<ManifestDownloadState>();
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ManifestFileCompletedState
    {
        public string Path { get; set; } = "";
        public DateTime ModifiedTimestampOnCompleted { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable, JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ManifestDownloadProgressState
    {
        RetrievingManifest,
        Initializing,
        Downloading,
        Complete,
        Validating,
        InitializeFailed,
        ValidationFailed,
        Installing,
        InstallFailed,
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ManifestDownloadState
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ManifestDownloadProgressState State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LocalFolder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SparseStateArray BlockStates { get; set; } = new SparseStateArray();

        /// <summary>
        /// 
        /// </summary>
        public List<ManifestFileCompletedState> FileCompletedStates { get; set; } = new List<ManifestFileCompletedState>();

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public ulong LastManifestRequestTime = 0;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public BuildManifest Manifest;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public RateTracker BandwidthStats = new RateTracker();

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        internal Task InitializeTask = null;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        internal Task ValidationTask = null;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        internal Task InstallTask = null;

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public float ValidateProgress { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public float InitializeProgress { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool InstallOnComplete = false;

        /// <summary>
        /// 
        /// </summary>
        public string InstallDeviceName = "";

        /// <summary>
        /// 
        /// </summary>
        public bool Installed = false;

        /// <summary>
        /// 
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
                else
                {
                    float BlocksRetrieved = BlockStates.Count(true);
                    return BlocksRetrieved / BlockStates.Size;
                }
            }
        }

        /// <summary>
        /// 
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
                else
                {
                    long TotalSize = Manifest.GetTotalSize();
                    long Downloaded = Manifest.GetTotalSizeOfBlocks(BlockStates);

                    return TotalSize - Downloaded;
                }
            }
        }
    }
}
