using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Manifests;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DownloadStateCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DownloadState> States = new List<DownloadState>();
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DownloadState
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id;

        /// <summary>
        /// 
        /// </summary>
        public string Name;

        /// <summary>
        /// 
        /// </summary>
        public string VirtualPath;

        /// <summary>
        /// 
        /// </summary>
        public int Priority;

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateAutomatically;

        /// <summary>
        /// 
        /// </summary>
        public bool Paused;

        /// <summary>
        /// 
        /// </summary>
        public Guid ActiveManifestId;
    }
}
