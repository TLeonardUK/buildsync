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
using System.Diagnostics;
using System.IO;
using BuildSync.Core.Scm;
using BuildSync.Core.Utils;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Tags;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// </summary>
    /// <param name="State"></param>
    public delegate void DownloadStartedHandler(DownloadState State);

    /// <summary>
    /// </summary>
    /// <param name="State"></param>
    public delegate void DownloadFinishedHandler(DownloadState State);

    /// <summary>
    /// </summary>
    /// <param name="State"></param>
    public delegate void RequestReplicatedBuildsHandler(List<Guid> SelectTags, List<Guid> IgnoreTags, DateTime NewerThan);

    /// <summary>
    /// </summary>
    public class DownloadManager
    {
        /// <summary>
        /// </summary>
        public VirtualFileSystem BuildFileSystem = new VirtualFileSystem();

        /// <summary>
        /// </summary>
        public ScmManager ScmManager;

        /// <summary>
        /// </summary>
        private bool bHadConnection;

        /// <summary>
        /// </summary>
        private readonly FileCache FileContentsCache = new FileCache();

        /// <summary>
        /// </summary>
        private ManifestDownloadManager ManifestDownloader;

        /// <summary>
        /// </summary>
        private DownloadStateCollection StateCollection = new DownloadStateCollection();

        /// <summary>
        /// </summary>
        public bool AreStatesDirty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoReplicate { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public List<Guid> ReplicateSelectTags { get; set; } = new List<Guid>();

        /// <summary>
        /// 
        /// </summary>
        public List<Guid> ReplicateIgnoreTags { get; set; } = new List<Guid>();

        /// <summary>
        /// 
        /// </summary>
        private ulong LastReplicationRequestTime = 0;

        /// <summary>
        /// 
        /// </summary>
        public DateTime ReplicationNewerThanTime = DateTime.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        private const long ReplicationCheckInterval = 60 * 1000;

        /// <summary>
        /// 
        /// </summary>
        public const long ReplicationMaxIdleDays = 3;

        /// <summary>
        /// </summary>
        public DownloadStateCollection States => StateCollection;

        /// <summary>
        /// 
        /// </summary>
        public event DownloadStartedHandler OnDownloadStarted;

        /// <summary>
        /// 
        /// </summary>
        public event DownloadFinishedHandler OnDownloadFinished;

        /// <summary>
        /// 
        /// </summary>
        public event RequestReplicatedBuildsHandler OnRequestReplicatedBuilds;

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="VirtualPath"></param>
        /// <param name="Priority"></param>
        /// <param name="KeepUpToDate"></param>
        public DownloadState AddDownload(string Name, string VirtualPath, int Priority, BuildSelectionRule Rule, BuildSelectionFilter Filter, string SelectionFilterFilePath, string ScmWorkspaceLocation, bool AutomaticallyUpdate, bool AutomaticallyInstall, string InstallDeviceName, string InstallLocation, List<Guid> IncludeTags, List<Guid> ExcludeTags, bool DeleteOnComplete = false)
        {
            DownloadState State = new DownloadState();
            State.Id = Guid.NewGuid();
            State.Name = Name;
            State.VirtualPath = VirtualPath;
            State.Priority = Priority;
            State.UpdateAutomatically = AutomaticallyUpdate;
            State.InstallAutomatically = AutomaticallyInstall;
            State.InstallDeviceName = InstallDeviceName;
            State.InstallLocation = InstallLocation;
            State.SelectionRule = Rule;
            State.SelectionFilter = Filter;
            State.SelectionFilterFilePath = SelectionFilterFilePath;
            State.ScmWorkspaceLocation = ScmWorkspaceLocation;
            State.IncludeTags = IncludeTags;
            State.ExcludeTags = ExcludeTags;
            State.IsAutomaticReplication = DeleteOnComplete;

            StateCollection.States.Add(State);
            AreStatesDirty = true;

            return State;
        }

        /// <summary>
        /// </summary>
        public void ForceRefresh()
        {
            BuildFileSystem.ForceRefresh();
            LastReplicationRequestTime = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        private bool BuildHasAllTags(NetMessage_GetBuildsResponse.BuildInfo Info, List<Guid> Ids)
        {
            foreach (Guid Id in Ids)
            {
                bool Found = false;

                foreach (Tag tag in Info.Tags)
                {
                    if (tag.Id == Id)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        private bool BuildHasAnyTags(NetMessage_GetBuildsResponse.BuildInfo Info, List<Guid> Ids)
        {
            foreach (Tag Tag in Info.Tags)
            {
                if (Ids.Contains(Tag.Id))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public Guid GetTargetManifestForState(DownloadState State)
        {
            string VirtualPath = State.VirtualPath;

            VirtualFileSystemNode Node = BuildFileSystem.GetNodeByPath(VirtualPath);
            if (Node == null)
            {
                return Guid.Empty;
            }


            // Node is a build in and of itself, use its id.
            if (Node.Metadata != null)
            {
                NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Node.Metadata;
                Guid ManifestId = (Guid)BuildInfo.Guid;
                if (ManifestId != Guid.Empty)
                {
                    return ManifestId;
                }
            }

            List<VirtualFileSystemNode> Children = BuildFileSystem.GetChildren(VirtualPath);
            VirtualFileSystemNode SelectedChild = null;

            List<VirtualFileSystemNode> BuildChildren = new List<VirtualFileSystemNode>();
            foreach (VirtualFileSystemNode Child in Children)
            {
                if (Child.Metadata != null)
                {
                    NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Child.Metadata;
                    Guid ManifestId = (Guid)BuildInfo.Guid;
                    if (ManifestId != Guid.Empty)
                    {
                        BuildChildren.Add(Child);
                    }
                }
            }

            // Remove all children without included tags.
            for (int i = 0; i < BuildChildren.Count; i++)
            {
                VirtualFileSystemNode Child = BuildChildren[i];
                if (Child.Metadata != null)
                {
                    NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Child.Metadata;
                    if (!BuildHasAllTags(BuildInfo, State.IncludeTags))
                    {
                        BuildChildren.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Remove all children with any tags..
            for (int i = 0; i < BuildChildren.Count; i++)
            {
                VirtualFileSystemNode Child = BuildChildren[i];
                if (Child.Metadata != null)
                {
                    NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Child.Metadata;
                    if (BuildHasAnyTags(BuildInfo, State.ExcludeTags))
                    {
                        BuildChildren.RemoveAt(i);
                        i--;
                    }
                }
            }

            List<VirtualFileSystemNode> FilteredChildren = new List<VirtualFileSystemNode>();
            switch (State.SelectionFilter)
            {
                case BuildSelectionFilter.None:
                {
                    FilteredChildren = BuildChildren;
                    break;
                }
                case BuildSelectionFilter.BuildTimeBeforeScmSyncTime:
                {
                    IScmProvider Workspace = ScmManager.GetProvider(State.ScmWorkspaceLocation);
                    if (Workspace != null)
                    {
                        DateTime ScmSyncTime = Workspace.GetSyncTime();
                        if (ScmSyncTime != DateTime.MinValue)
                        {
                            foreach (VirtualFileSystemNode Child in BuildChildren)
                            {
                                if (Child.CreateTime <= ScmSyncTime)
                                {
                                    FilteredChildren.Add(Child);
                                }
                            }
                        }
                    }

                    break;
                }
                case BuildSelectionFilter.BuildTimeAfterScmSyncTime:
                {
                    IScmProvider Workspace = ScmManager.GetProvider(State.ScmWorkspaceLocation);
                    if (Workspace != null)
                    {
                        DateTime ScmSyncTime = Workspace.GetSyncTime();
                        if (ScmSyncTime != DateTime.MinValue)
                        {
                            foreach (VirtualFileSystemNode Child in BuildChildren)
                            {
                                if (Child.CreateTime >= ScmSyncTime)
                                {
                                    FilteredChildren.Add(Child);
                                }
                            }
                        }
                    }

                    break;
                }
                case BuildSelectionFilter.BuildNameBelowFileContents:
                {
                    string FilePath = Path.Combine(State.ScmWorkspaceLocation, State.SelectionFilterFilePath);
                    string FileContents = FileContentsCache.Get(FilePath);

                    int Value = 0;
                    if (int.TryParse(FileContents, out Value))
                    {
                        foreach (VirtualFileSystemNode Child in BuildChildren)
                        {
                            int ChildValue = 0;
                            if (int.TryParse(Child.Name, out ChildValue))
                            {
                                if (Value <= ChildValue)
                                {
                                    FilteredChildren.Add(Child);
                                }
                            }
                        }
                    }

                    break;
                }
                case BuildSelectionFilter.BuildNameAboveFileContents:
                {
                    string FilePath = Path.Combine(State.ScmWorkspaceLocation, State.SelectionFilterFilePath);
                    string FileContents = FileContentsCache.Get(FilePath);

                    int Value = 0;
                    if (int.TryParse(FileContents, out Value))
                    {
                        foreach (VirtualFileSystemNode Child in BuildChildren)
                        {
                            int ChildValue = 0;
                            if (int.TryParse(Child.Name, out ChildValue))
                            {
                                if (Value >= ChildValue)
                                {
                                    FilteredChildren.Add(Child);
                                }
                            }
                        }
                    }

                    break;
                }
                case BuildSelectionFilter.BuildNameEqualsFileContents:
                {
                    string FilePath = Path.Combine(State.ScmWorkspaceLocation, State.SelectionFilterFilePath);
                    string FileContents = FileContentsCache.Get(FilePath);

                    foreach (VirtualFileSystemNode Child in BuildChildren)
                    {
                        if (FileContents == Child.Name)
                        {
                            FilteredChildren.Add(Child);
                        }
                    }

                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }

            switch (State.SelectionRule)
            {
                case BuildSelectionRule.Newest:
                {
                    foreach (VirtualFileSystemNode Child in FilteredChildren)
                    {
                        if (SelectedChild == null || SelectedChild.CreateTime < Child.CreateTime)
                        {
                            SelectedChild = Child;
                        }
                    }

                    break;
                }
                case BuildSelectionRule.Oldest:
                {
                    foreach (VirtualFileSystemNode Child in FilteredChildren)
                    {
                        if (SelectedChild == null || SelectedChild.CreateTime > Child.CreateTime)
                        {
                            SelectedChild = Child;
                        }
                    }

                    break;
                }
            }

            if (SelectedChild != null)
            {
                NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)SelectedChild.Metadata;
                return BuildInfo.Guid;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// </summary>
        /// <param name="State"></param>
        public Guid GetTargetManifestForVirtualPath(string VirtualPath)
        {
            VirtualFileSystemNode Node = BuildFileSystem.GetNodeByPath(VirtualPath);
            if (Node == null)
            {
                return Guid.Empty;
            }

            // Node is a build in and of itself, use its id.
            if (Node.Metadata != null)
            {
                NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Node.Metadata;
                Guid ManifestId = BuildInfo.Guid;
                if (ManifestId != Guid.Empty)
                {
                    return ManifestId;
                }
            }

            // Look through children and return latest.
            List<VirtualFileSystemNode> Children = BuildFileSystem.GetChildren(VirtualPath);
            VirtualFileSystemNode NewestChild = null;
            foreach (VirtualFileSystemNode Child in Children)
            {
                if (Child.Metadata != null)
                {
                    NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Child.Metadata;
                    Guid ManifestId = BuildInfo.Guid;
                    if (ManifestId != Guid.Empty)
                    {
                        if (NewestChild == null || NewestChild.CreateTime < Child.CreateTime)
                        {
                            NewestChild = Child;
                        }
                    }
                }
            }

            if (NewestChild != null)
            {
                NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)NewestChild.Metadata;
                return BuildInfo.Guid;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// </summary>
        public void Poll(bool bHasConnection)
        {
            if (bHasConnection && !bHadConnection)
            {
                ForceRefresh();
            }

            bHadConnection = bHasConnection;

            if (bHasConnection)
            {
                UpdateReplication();
            }
            UpdateDownloads(bHasConnection);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RecieveReplicatedBuilds(NetMessage_GetFilteredBuildsResponse.BuildInfo[] Builds)
        {
            if (!AutoReplicate)
            {
                return;
            }

            Logger.Log(LogLevel.Info, LogCategory.Download, "Recieved {0} replicated builds from server.", Builds.Length);

            foreach (NetMessage_GetFilteredBuildsResponse.BuildInfo Build in Builds)
            {
                Logger.Log(LogLevel.Info, LogCategory.Download, "\tBuild:{0} Path:{1}", Build.Guid.ToString(), Build.VirtualPath);

                // Already downloaded/downloading.
                ManifestDownloadState State = ManifestDownloader.GetDownload(Build.Guid);
                if (State != null)
                {
                    continue;
                }

                // Create a new download.
                AddDownload(
                    "Automatic Replication",
                    Build.VirtualPath,
                    0,
                    BuildSelectionRule.Newest,
                    BuildSelectionFilter.None,
                    "",
                    "",
                    false,
                    false,
                    "",
                    "",
                    new List<Guid>(),
                    new List<Guid>(),
                    true
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateReplication()
        {
            if (!AutoReplicate)
            {
                return;
            }

            ulong Elapsed = TimeUtils.Ticks - LastReplicationRequestTime;
            if (Elapsed > ReplicationCheckInterval && bHadConnection)
            {
                Logger.Log(LogLevel.Info, LogCategory.Download, "Requesting update on published builds.");

                OnRequestReplicatedBuilds?.Invoke(ReplicateSelectTags, ReplicateIgnoreTags, ReplicationNewerThanTime);

                ReplicationNewerThanTime = DateTime.UtcNow;
                LastReplicationRequestTime = TimeUtils.Ticks;
            }        
        }

        /// <summary>
        /// </summary>
        private void UpdateDownloads(bool bHasConnection)
        {
            List<Guid> ActiveManifestIds = new List<Guid>();

            foreach (DownloadState State in StateCollection.States)
            {
                bool UpdateManifestId = true;

                // If not set to auto update and we already have a completed download, do nothing else.
                if (!State.UpdateAutomatically && State.ActiveManifestId != Guid.Empty)
                {
                    ManifestDownloadState OldVersionDownloader = ManifestDownloader.GetDownload(State.ActiveManifestId);
                    if (OldVersionDownloader != null && OldVersionDownloader.State == ManifestDownloadProgressState.Complete)
                    {
                        UpdateManifestId = false;
                    }
                }

                if (bHasConnection && UpdateManifestId)
                {
                    Guid NewManifestId = GetTargetManifestForState(State);
                    if (NewManifestId != Guid.Empty)
                    {
                        if (State.ActiveManifestId != NewManifestId && !State.Paused)
                        {
                            OnDownloadStarted?.Invoke(State);
                        }

                        State.ActiveManifestId = NewManifestId;
                    }
                }

                if (State.ActiveManifestId == Guid.Empty)
                {
                    continue;
                }

                ActiveManifestIds.Add(State.ActiveManifestId);

                ManifestDownloadState Downloader = ManifestDownloader.GetDownload(State.ActiveManifestId);
                if (Downloader != null)
                {
                    // If state is downloading but we've paused it, pause it and ignore.
                    if (State.Paused)
                    {
                        if (!Downloader.Paused)
                        {
                            ManifestDownloader.PauseDownload(State.ActiveManifestId);
                            continue;
                        }
                    }
                    else
                    {
                        if (Downloader.Paused)
                        {
                            ManifestDownloader.ResumeDownload(State.ActiveManifestId);
                        }
                    }

                    // Hackily change the priority - should change this to a SetDownloadPriority method.
                    Downloader.Priority = State.Priority;
                    Downloader.InstallOnComplete = State.InstallAutomatically;
                    Downloader.InstallDeviceName = State.InstallDeviceName;
                    Downloader.InstallLocation = State.InstallLocation;

                    // Have we finished this download?
                    if (Downloader.State == ManifestDownloadProgressState.Complete &&
                        State.PreviousDownloaderState != Downloader.State &&
                        State.PreviousDownloaderState != ManifestDownloadProgressState.Unknown)
                    {
                        OnDownloadFinished?.Invoke(State);
                    }

                    State.PreviousDownloaderState = Downloader.State;

                    // Store the amount of time the download is in each state to make
                    // time estimates a bit better.
                    if (Downloader.State != ManifestDownloadProgressState.Complete && !State.Paused && Downloader.Manifest != null && bHasConnection)
                    {
                        if (State.PendingDurationHistory == null)
                        {
                            State.PendingDurationHistory = new DownloadStateDuration();
                            State.PendingDurationHistory.TotalSize = Downloader.Manifest.GetTotalSize();
                        }

                        ulong CurrentTime = TimeUtils.Ticks;
                        if (State.PendingDurationTimer > 0)
                        {
                            ulong Elapsed = TimeUtils.Ticks - State.PendingDurationTimer;
                            ulong StartTime = State.PendingDurationHistory.GetDuration(Downloader.State);
                            ulong NewTime = StartTime + Elapsed;
                            State.PendingDurationHistory.SetDuration(Downloader.State, NewTime);
                            //Console.WriteLine("State:{0} Time:{1}", Downloader.State.ToString(), StringUtils.FormatAsDuration((long)State.PendingDurationHistory.StateDurations[Downloader.State] / 1000));
                        }
                        State.PendingDurationTimer = CurrentTime;
                    }
                    else if (Downloader.State == ManifestDownloadProgressState.Complete)
                    {
                        if (State.PendingDurationHistory != null)
                        {
                            State.DurationHistory.Add(State.PendingDurationHistory);
                            State.PendingDurationHistory = null;

                            // Only keep a few history entries.
                            while (State.DurationHistory.Count > 15)
                            {
                                State.DurationHistory.RemoveAt(0);
                            }
                        }
                    }
                }
                else
                {
                    // Start downloading this manifest.
                    ManifestDownloader.StartDownload(State.ActiveManifestId, State.Priority);
                }
            }

            // Go through each manifest download and pause any that are no longer relevant.
            foreach (ManifestDownloadState Downloader in ManifestDownloader.States.States)
            {
                if (!ActiveManifestIds.Contains(Downloader.ManifestId))
                {
                    if (!Downloader.Paused)
                    {
                        ManifestDownloader.PauseDownload(Downloader.ManifestId);
                    }

                    Downloader.Active = false;
                }
                else
                {
                    Downloader.Active = true;
                    Downloader.LastActive = DateTime.Now;
                }
            }

            // Remove auto replication downloads as required.
            for (int i = 0; i < StateCollection.States.Count; i++)
            {
                DownloadState State = StateCollection.States[i];

                if (State.ActiveManifestId == Guid.Empty)
                {
                    continue;
                }

                if (!State.IsAutomaticReplication)
                {
                    continue;
                }

                ManifestDownloadState Downloader = ManifestDownloader.GetDownload(State.ActiveManifestId);
                if (Downloader != null)
                {
                    bool ShouldRemove = false;

                    // Always remove downloads that have completed.
                    if (Downloader.State == ManifestDownloadProgressState.Complete)
                    {
                        ShouldRemove = true;
                    }

                    // Download has not recieved any blocks in a long time, remove.
                    TimeSpan Elapsed = DateTime.UtcNow - Downloader.LastRecievedData;
                    if (Elapsed.TotalDays > ReplicationMaxIdleDays)
                    {
                        ShouldRemove = true;
                    }

                    // Automatically resume replication downloads if they are in an error state.
                    if (State.Paused)
                    {
                        State.Paused = false;
                        //ManifestDownloader.ResumeDownload(Downloader.ManifestId);
                    }

                    if (ShouldRemove)
                    {
                        StateCollection.States.RemoveAt(i);
                        AreStatesDirty = false;
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <param name="TotalDownloadSize"></param>
        /// <param name="CurrentProgress"></param>
        public long GetEstimatedTimeRemainingForState(DownloadState State, ManifestDownloadProgressState ProgressState)
        {
            double DurationSum = 0;
            int SumCount = 0;
            long TotalSize = 0;

            double RawEstimateSeconds = 0.0f;
            double RawProgress = 0.0f;

            double CurrentStateDuration = 0.0f;

            if (State.PendingDurationHistory != null)
            {
                CurrentStateDuration = State.PendingDurationHistory.GetDuration(ProgressState);
            }

            ManifestDownloadState Downloader = ManifestDownloader.GetDownload(State.ActiveManifestId);
            if (Downloader != null && Downloader.Manifest != null)
            {
                TotalSize = Downloader.Manifest.GetTotalSize();

                // Calculate raw estimate based on speeds and data remaining.
                switch (Downloader.State)
                {
                    case ManifestDownloadProgressState.Initializing:
                    {
                        RawEstimateSeconds = Downloader.InitializeBytesRemaining / (double)Downloader.InitializeRateStats.RateIn;
                        RawProgress = Downloader.InitializeProgress;
                        if (Downloader.InitializeRateStats.RateIn == 0)
                        {
                            RawEstimateSeconds = 0;
                        }
                        break;
                    }
                    case ManifestDownloadProgressState.DeltaCopying:
                    {
                        RawEstimateSeconds = Downloader.DeltaCopyBytesRemaining / (double)Downloader.DeltaCopyRateStats.RateIn;
                        RawProgress = Downloader.DeltaCopyProgress;
                        if (Downloader.DeltaCopyRateStats.RateIn == 0)
                        {
                            RawEstimateSeconds = 0;
                        }
                        break;
                    }
                    case ManifestDownloadProgressState.Validating:
                    {
                        RawEstimateSeconds = Downloader.ValidateBytesRemaining / (double)Downloader.ValidateRateStats.RateOut;
                        RawProgress = Downloader.ValidateProgress;
                        if (Downloader.ValidateRateStats.RateOut == 0)
                        {
                            RawEstimateSeconds = 0;
                        }
                        break;
                    }
                    case ManifestDownloadProgressState.Downloading:
                    {
                        RawEstimateSeconds = Downloader.BytesRemaining / (double)Downloader.BandwidthStats.RateIn;
                        RawProgress = Downloader.Progress;
                        if (Downloader.BandwidthStats.RateIn == 0)
                        {
                            RawEstimateSeconds = 0;
                        }
                        break;
                    }
                }
            }

            // Calculate average historic duration adjusted to the difference in download size.
            foreach (DownloadStateDuration Duration in State.DurationHistory)
            {
                if (Duration.HasDuration(ProgressState))
                {
                    ulong Unadjusted = Duration.GetDuration(ProgressState);
                    float Adjustment = TotalSize == 0 ? 1.0f : (float)Duration.TotalSize / (float)TotalSize;
                    ulong AdjustedDuration = (ulong)(Unadjusted * Adjustment);

                    DurationSum += AdjustedDuration;
                    SumCount++;
                }
            }

            if (SumCount != 0)
            {
                double Historic = Math.Max(0, (DurationSum / SumCount) - CurrentStateDuration) / 1000.0f;// ((DurationSum / SumCount) * (1.0f - RawProgress)) / 1000.0f;
                double Combined = (Historic * 0.75f) + (RawEstimateSeconds * 0.25f);

                return (long)Combined;

            }
            else
            {
                return (long)RawEstimateSeconds;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <param name="TotalDownloadSize"></param>
        /// <param name="CurrentProgress"></param>
        public long GetEstimatedTimeRemaining(DownloadState State)
        {
            ulong SumCount = 0;
            ulong SumValue = 0;
            long TotalSize = 0;

            ManifestDownloadState Downloader = ManifestDownloader.GetDownload(State.ActiveManifestId);
            if (Downloader != null && Downloader.Manifest != null)
            {
                TotalSize = Downloader.Manifest.GetTotalSize();
            }

            foreach (DownloadStateDuration Duration in State.DurationHistory)
            {
                if (Duration.Values.Count > 0)
                {
                    ulong Unadjusted = 0;

                    foreach (DownloadStateDurationValue Value in Duration.Values)
                    {
                        Unadjusted += Value.Elapsed;
                    }

                    float Adjustment = TotalSize == 0 ? 1.0f : (float)Duration.TotalSize / (float)TotalSize;
                    ulong AdjustedDuration = (ulong)(Unadjusted * Adjustment);

                    SumValue += AdjustedDuration;
                    SumCount++;
                }
            }

            ulong AverageDuration = SumCount == 0 ? 0 : (SumValue / SumCount);
            ulong CurrentElapsed = 0;

            if (State.PendingDurationHistory != null)
            {
                foreach (DownloadStateDurationValue Value in State.PendingDurationHistory.Values)
                {
                    CurrentElapsed += Value.Elapsed;
                }
            }

            return (Math.Max(0, (long)AverageDuration - (long)CurrentElapsed) / 1000);
        }

        /// <summary>
        /// </summary>
        public void RemoveDownload(DownloadState State)
        {
            StateCollection.States.Remove(State);
            AreStatesDirty = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="InManifestDownloader"></param>
        /// <param name="Config"></param>
        public void Start(ManifestDownloadManager InManifestDownloader, DownloadStateCollection ResumeStateCollection, VirtualFileSystem FileSystem, ScmManager InScmManager, DateTime InReplicationNewestTime)
        {
            ManifestDownloader = InManifestDownloader;
            ManifestDownloader.OnDownloadError += DownloadError;

            ReplicationNewerThanTime = InReplicationNewestTime;

            ScmManager = InScmManager;

            StateCollection = ResumeStateCollection;
            BuildFileSystem = FileSystem;
            if (StateCollection == null)
            {
                StateCollection = new DownloadStateCollection();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="ManifestId"></param>
        private void DownloadError(Guid ManifestId)
        {
            foreach (DownloadState State in StateCollection.States)
            {
                if (State.ActiveManifestId == ManifestId)
                {
                    State.Paused = true;
                }
            }
        }
    }
}