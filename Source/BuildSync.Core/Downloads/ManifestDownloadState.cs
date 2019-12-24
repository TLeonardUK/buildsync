using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
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
        public List<ManifestDownloadState> States = new List<ManifestDownloadState>();
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ManifestDownloadProgressState
    {
        RetrievingManifest,
        Initializing,
        Downloading,
        Complete,
        Validating,
        InitializeFailed,
        ValidationFailed
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
        public Guid Id;

        /// <summary>
        /// 
        /// </summary>
        public ManifestDownloadProgressState State;

        /// <summary>
        /// 
        /// </summary>
        public bool Paused;

        /// <summary>
        /// 
        /// </summary>
        public bool Active;

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastActive;

        /// <summary>
        /// 
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        /// 
        /// </summary>
        public int Priority;

        /// <summary>
        /// 
        /// </summary>
        public string LocalFolder;

        /// <summary>
        /// 
        /// </summary>
        public SparseStateArray BlockStates;

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore, NonSerialized]
        public ulong LastManifestRequestTime = 0;

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore, NonSerialized]
        public BuildManifest Manifest;

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore, NonSerialized]
        public BandwidthTracker BandwidthStats = new BandwidthTracker();

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public float Progress
        {
            get
            {
                float BlocksRetrieved = BlockStates.Count(true);
                return BlocksRetrieved / BlockStates.Size;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public long BytesRemaining
        {
            get
            {
                long TotalSize = Manifest.GetTotalSize();
                long Downloaded = Manifest.GetTotalSizeOfBlocks(BlockStates);

                return TotalSize - Downloaded;
            }
        }
    }
}
