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
using System.ComponentModel;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// </summary>
    public enum BuildSelectionRule
    {
        [Description("Newest")]
        Newest,

        [Description("Oldest")]
        Oldest
    }

    /// <summary>
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
        BuildNameEqualsFileContents
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class DownloadStateCollection
    {
        /// <summary>
        /// </summary>
        public List<DownloadState> States { get; set; } = new List<DownloadState>();
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class DownloadState
    {
        /// <summary>
        /// </summary>
        public Guid ActiveManifestId { get; set; }

        /// <summary>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// </summary>
        public bool InstallAutomatically { get; set; }

        /// <summary>
        /// </summary>
        public string InstallDeviceName { get; set; }

        /// <summary>
        /// </summary>
        public bool Installed { get; set; }

        /// <summary>
        /// </summary>
        public bool IsInternal => Name.Contains("$");

        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// </summary>
        public string ScmWorkspaceLocation { get; set; }

        /// <summary>
        /// </summary>
        public BuildSelectionFilter SelectionFilter { get; set; }

        /// <summary>
        /// </summary>
        public string SelectionFilterFilePath { get; set; }

        /// <summary>
        /// </summary>
        public BuildSelectionRule SelectionRule { get; set; }

        /// <summary>
        /// </summary>
        public bool UpdateAutomatically { get; set; }

        /// <summary>
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// </summary>
        [JsonIgnore]
        public ManifestDownloadProgressState PreviousDownloaderState { get; set; } = ManifestDownloadProgressState.Unknown;
    }
}