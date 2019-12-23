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
            foreach (VirtualFileSystemNode Child in Children)
            {
                if (Child.Metadata != null)
                {
                    Guid ManifestId = (Guid)Child.Metadata;
                    if (ManifestId != Guid.Empty)
                    {
                        return ManifestId;
                    }
                }
            }

            // This is not a valid virtual path, or no builds are contained in it at the moment ...
            return Guid.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll(bool bHasConnection)
        {
            List<Guid> ActiveManifestIds = new List<Guid>();

            foreach (DownloadState State in StateCollection.States)
            {
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
            foreach (ManifestDownloadState State in ManifestDownloader.States.States.ToArray())
            {
                if (!ActiveManifestIds.Contains(State.ManifestId))
                {
                    ManifestDownloadState Downloader = ManifestDownloader.GetDownload(State.ManifestId);
                    if (Downloader != null)
                    {
                        if (!Downloader.Paused)
                        {
                            ManifestDownloader.PauseDownload(State.ManifestId);
                        }
                    }
                }
            }
        }
    }
}
