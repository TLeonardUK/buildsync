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
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="VirtualPath"></param>
        /// <param name="Priority"></param>
        /// <param name="KeepUpToDate"></param>
        public DownloadState AddDownload(string Name, string VirtualPath, int Priority, BuildSelectionRule Rule, BuildSelectionFilter Filter, string SelectionFilterFilePath, string ScmWorkspaceLocation, bool AutomaticallyUpdate, bool AutomaticallyInstall, string InstallDeviceName)
        {
            DownloadState State = new DownloadState();
            State.Id = Guid.NewGuid();
            State.Name = Name;
            State.VirtualPath = VirtualPath;
            State.Priority = Priority;
            State.UpdateAutomatically = AutomaticallyUpdate;
            State.InstallAutomatically = AutomaticallyInstall;
            State.InstallDeviceName = InstallDeviceName;
            State.SelectionRule = Rule;
            State.SelectionFilter = Filter;
            State.SelectionFilterFilePath = SelectionFilterFilePath;
            State.ScmWorkspaceLocation = ScmWorkspaceLocation;

            StateCollection.States.Add(State);
            AreStatesDirty = true;

            return State;
        }

        /// <summary>
        /// </summary>
        public void ForceRefresh()
        {
            BuildFileSystem.ForceRefresh();
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
                Guid ManifestId = (Guid) Node.Metadata;
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
                    Guid ManifestId = (Guid) Child.Metadata;
                    if (ManifestId != Guid.Empty)
                    {
                        BuildChildren.Add(Child);
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
                return (Guid) SelectedChild.Metadata;
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
                Guid ManifestId = (Guid) Node.Metadata;
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
                    Guid ManifestId = (Guid) Child.Metadata;
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
                return (Guid) NewestChild.Metadata;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// </summary>
        public void Poll(bool bHasConnection)
        {
            List<Guid> ActiveManifestIds = new List<Guid>();

            if (bHasConnection && !bHadConnection)
            {
                ForceRefresh();
            }

            bHadConnection = bHasConnection;

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

                    // Have we finished this download?
                    if (Downloader.State == ManifestDownloadProgressState.Complete && 
                        State.PreviousDownloaderState != Downloader.State &&
                        State.PreviousDownloaderState != ManifestDownloadProgressState.Unknown)
                    {
                        OnDownloadFinished?.Invoke(State);
                    }

                    State.PreviousDownloaderState = Downloader.State;
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
        public void Start(ManifestDownloadManager InManifestDownloader, DownloadStateCollection ResumeStateCollection, VirtualFileSystem FileSystem, ScmManager InScmManager)
        {
            ManifestDownloader = InManifestDownloader;
            ManifestDownloader.OnDownloadError += DownloadError;

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