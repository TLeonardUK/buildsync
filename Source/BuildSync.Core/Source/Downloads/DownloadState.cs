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
        public List<DownloadState> States { get; set; } = new List<DownloadState>();
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
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateAutomatically { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActiveManifestId { get; set; }
    }
}
