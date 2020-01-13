using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using BuildSync.Core.Manifests;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    public enum BuildSelectionRule
    {
        [Description("Newest")]
        Newest,

        [Description("Oldest")]
        Oldest,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BuildSelectionFilter
    {
        [Description("None")]
        None,

        [Description("Build time <= SCM last update time")]
        BuildTimeBeforeScmSyncTime,

        [Description("Build time >= SCM last update time")]
        BuildTimeAfterScmSyncTime,

        [Description("Build name (as number) <= SCM file contents (as number)")]
        BuildNameBelowFileContents,

        [Description("Build name (as number) >= SCM file contents (as number)")]
        BuildNameAboveFileContents,

        [Description("Build name == SCM file contents")]
        BuildNameEqualsFileContents,
    }

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
        public string ScmWorkspaceLocation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BuildSelectionRule SelectionRule { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BuildSelectionFilter SelectionFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SelectionFilterFilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateAutomatically { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool InstallAutomatically { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InstallDeviceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Installed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActiveManifestId { get; set; }
    }
}
