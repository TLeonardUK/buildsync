using System;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Manifests;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    public class DownloadManager
    {
        /// <summary>
        /// 
        /// </summary>
        private DownloadStateCollection StateCollection = new DownloadStateCollection();

        /// <summary>
        /// 
        /// </summary>
        public VirtualFileSystem BuildFileSystem = new VirtualFileSystem();

        /// <summary>
        /// 
        /// </summary>
        private ManifestDownloadManager ManifestDownloader;

        /// <summary>
        /// 
        /// </summary>
        public bool AreStatesDirty
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DownloadStateCollection States
        {
            get { return StateCollection; }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool bHadConnection = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InManifestDownloader"></param>
        /// <param name="Config"></param>
        public void Start(ManifestDownloadManager InManifestDownloader, DownloadStateCollection ResumeStateCollection, VirtualFileSystem FileSystem)
        {
            ManifestDownloader = InManifestDownloader;
            ManifestDownloader.OnDownloadError += DownloadError;

            StateCollection = ResumeStateCollection;
            BuildFileSystem = FileSystem;
            if (StateCollection == null)
            {
                StateCollection = new DownloadStateCollection();
            }
        }

        /// <summary>
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="VirtualPath"></param>
        /// <param name="Priority"></param>
        /// <param name="KeepUpToDate"></param>
        public DownloadState AddDownload(string Name, string VirtualPath, int Priority, bool AutomaticallyUpdate)
        {
            DownloadState State = new DownloadState();
            State.Id = Guid.NewGuid();
            State.Name = Name;
            State.VirtualPath = VirtualPath;
            State.Priority = Priority;
            State.UpdateAutomatically = AutomaticallyUpdate;

            StateCollection.States.Add(State);
            AreStatesDirty = true;

            return State;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveDownload(DownloadState State)
        {
            StateCollection.States.Remove(State);
            AreStatesDirty = false;
        }

        /// <summary>
        /// 
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
                Guid ManifestId = (Guid)Node.Metadata;
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
                    Guid ManifestId = (Guid)Child.Metadata;
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
                return (Guid)NewestChild.Metadata;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForceRefresh()
        {
            BuildFileSystem.ForceRefresh();
        }

        /// <summary>
        /// 
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
                // If not set to auto update and we already have a completed download, do nothing else.
                if (!State.UpdateAutomatically && State.ActiveManifestId != Guid.Empty)
                {
                    ManifestDownloadState OldVersionDownloader = ManifestDownloader.GetDownload(State.ActiveManifestId);
                    if (OldVersionDownloader != null && OldVersionDownloader.State == ManifestDownloadProgressState.Complete)
                    {
                        continue;
                    }
                }

                if (bHasConnection)
                {
                    State.ActiveManifestId = GetTargetManifestForVirtualPath(State.VirtualPath);
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
    }
}
