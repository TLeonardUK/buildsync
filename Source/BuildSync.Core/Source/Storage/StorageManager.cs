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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Storage
{
    /// <summary>
    /// </summary>
    public delegate void RequestChooseDeletionCandidateHandler(List<Guid> Candidates, ManifestStorageHeuristic Heuristic, List<Guid> PrioritizeKeepingTagIds, List<Guid> PrioritizeDeletingTagIds);

    /// <summary>
    /// 
    /// </summary>
    public class StorageManager
    {
        /// <summary>
        ///
        /// </summary>
        public event RequestChooseDeletionCandidateHandler OnRequestChooseDeletionCandidate;

        /// <summary>
        /// </summary>
        public ManifestStorageHeuristic StorageHeuristic { get; set; } = ManifestStorageHeuristic.LeastAvailable;

        /// <summary>
        ///     List of all tag id's to prioritize keeping when deleting builds for space.
        /// </summary>
        public List<Guid> StoragePrioritizeKeepingBuildTagIds { get; set; } = new List<Guid>();

        /// <summary>
        ///     List of all tag id's to prioritize deleting when deleting builds for space.
        /// </summary>
        public List<Guid> StoragePrioritizeDeletingBuildTagIds { get; set; } = new List<Guid>();

        /// <summary>
        /// </summary>
        private List<StorageLocation> Locations = new List<StorageLocation>();

        /// <summary>
        /// </summary>
        private List<StorageLocation> StoredLocations = new List<StorageLocation>();

        /// <summary>
        /// 
        /// </summary>
        private ManifestDownloadManager DownloadManager = null;

        /// <summary>
        /// 
        /// </summary>
        private List<string> MoveOldDirectories = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        private List<string> MoveNewDirectories = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        private ulong TimeSinceLastPruneRequest = 0;

        /// <summary>
        /// </summary>
        private readonly List<string> PendingOrphanCleanups = new List<string>();

        /// <summary>
        /// </summary>
        private BuildManifestRegistry ManifestRegistry;

        /// <summary>
        /// </summary>
        private AsyncIOQueue IOQueue;

        /// <summary>
        /// 
        /// </summary>
        public StorageManager(List<StorageLocation> InLocations, ManifestDownloadManager InDownloadManager, BuildManifestRegistry Registry, AsyncIOQueue InIOQueue)
        {
            IOQueue = InIOQueue;
            ManifestRegistry = Registry;
            DownloadManager = InDownloadManager;
            Locations = InLocations;
            StoredLocations = new List<StorageLocation>(Locations);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool MoveLocations(List<StorageLocation> NewLocations)
        {
            List<StorageLocation> DeletedLocations = new List<StorageLocation>();

            MoveOldDirectories.Clear();
            MoveNewDirectories.Clear();

            // Find all deleted locations.
            foreach (StorageLocation CurrentLocation in StoredLocations)
            {
                bool Exists = false;
                foreach (StorageLocation NewLocation in NewLocations)
                {
                    if (FileUtils.NormalizePath(CurrentLocation.Path) == FileUtils.NormalizePath(NewLocation.Path))
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    DeletedLocations.Add(CurrentLocation);
                }
            }

            Locations = NewLocations;
            StoredLocations = new List<StorageLocation>(Locations);

            // If no deleted locations, we don't need to perform a move taks.
            if (DeletedLocations.Count == 0)
            {
                return false;
            }

            // Find all the manifests that exist in deleted folders and queue them up for a move.
            foreach (ManifestDownloadState State in DownloadManager.States.States)
            {
                string NormalizedStatePath = FileUtils.NormalizePath(State.LocalFolder) + @"\";

                foreach (StorageLocation Location in DeletedLocations)
                {
                    string NormalizedLocationPath = FileUtils.NormalizePath(Location.Path) + @"\";
                    if (NormalizedStatePath.StartsWith(NormalizedLocationPath))
                    {
                        MoveOldDirectories.Add(NormalizedStatePath);

                        string NewPath = "";
                        if (!AllocateSpace(State.Manifest == null ? 0 : State.Manifest.GetTotalSize(), State.ManifestId, out NewPath))
                        {
                            // Delete space requirements :|
                            MoveNewDirectories.Add("");
                        }
                        else
                        {
                            MoveNewDirectories.Add(NewPath + @"\");
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OldDirectories"></param>
        /// <param name="NewDirectories"></param>
        public void GetMoveStorageTasks(ref List<string> OldDirectories, ref List<string> NewDirectories)
        {
            OldDirectories = new List<string>(MoveOldDirectories);
            NewDirectories = new List<string>(MoveNewDirectories);

            MoveOldDirectories.Clear();
            MoveNewDirectories.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetDiskUsage()
        {
            long TotalUsage = 0;

            foreach (StorageLocation CurrentLocation in Locations)
            {
                TotalUsage += GetLocationDiskUsage(CurrentLocation);
            }

            return TotalUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetDiskQuota()
        {
            long TotalUsage = 0;

            foreach (StorageLocation CurrentLocation in Locations)
            {
                TotalUsage += CurrentLocation.MaxSize;
            }

            return TotalUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetLocationDiskUsage(StorageLocation Location)
        {
            string NormalizedLocationPath = FileUtils.NormalizePath(Location.Path) + @"\";

            long TotalUsage = 0;

            foreach (ManifestDownloadState State in DownloadManager.States.States)
            {
                string NormalizedStatePath = FileUtils.NormalizePath(State.LocalFolder) + @"\";
                if (NormalizedStatePath.StartsWith(NormalizedLocationPath))
                {
                    TotalUsage += (State.Manifest == null ? 0 : State.Manifest.GetTotalSize());
                }
            }

            return TotalUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DriveName"></param>
        /// <returns></returns>
        public long GetDriveFreeSpace(string DriveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == DriveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetLocationFreeSpace(StorageLocation Location)
        {
            long UsedSpace = GetLocationDiskUsage(Location);
            long AvailableSpace = Math.Max(0, Location.MaxSize - UsedSpace);

            if (Location.MaxSize == 0)
            {
                AvailableSpace = long.MaxValue;
            }

            // If rooted to a drive, check there is enough space on folder.
            if (Location.Path[1] == ':')
            {
                long AvailableDriveSpace = GetDriveFreeSpace(Location.Path.Substring(0, 3));
                if (AvailableDriveSpace >= 0)
                {
                    AvailableSpace = Math.Min(AvailableSpace, AvailableDriveSpace);
                }
            }

            return AvailableSpace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public bool AllocateSpace(long Size, Guid ManifestId, out string OutPath)
        {
            OutPath = "";

            long MostFreeSpace = 0;

            foreach (StorageLocation Location in Locations)
            {
                long FreeSpace = GetLocationFreeSpace(Location);
                if (FreeSpace >= Size && FreeSpace > MostFreeSpace)
                {
                    OutPath = Path.Combine(Path.Combine(Location.Path, @"Builds"), ManifestId.ToString());
                    MostFreeSpace = FreeSpace;
                }
            }

            return MostFreeSpace > 0;
        }

        /// <summary>
        /// </summary>
        public void Poll()
        {
            PruneDiskSpace();
        }

        /// <summary>
        /// </summary>
        public void PruneDiskSpace()
        {
            if (TimeUtils.Ticks - TimeSinceLastPruneRequest < 3000)
            {
                return;
            }

            foreach (StorageLocation Location in Locations)
            {
                long UsedSpace = GetLocationDiskUsage(Location);
                if (UsedSpace > Location.MaxSize) // Throttle send rate to give server time to respond.
                {
                    if (!CleanUpOrphanBuilds())
                    {
                        // Select manifests for deletion.
                        List<ManifestDownloadState> DeletionCandidates = new List<ManifestDownloadState>();
                        foreach (ManifestDownloadState State in DownloadManager.States.States)
                        {
                            if (!State.Active)
                            {
                                if (State.Manifest != null && !FileUtils.AnyRunningProcessesInDirectory(State.LocalFolder))
                                {
                                    DeletionCandidates.Add(State);
                                }
                            }
                        }
                        if (DeletionCandidates.Count > 0)
                        {
                            DeletionCandidates.Sort((Item1, Item2) => Item1.LastActive.CompareTo(Item2.LastActive));

                            // Request the server to select the next candidate for deletion.
                            List<Guid> DeletionCandidatesIds = new List<Guid>();
                            foreach (ManifestDownloadState State in DeletionCandidates)
                            {
                                DeletionCandidatesIds.Add(State.ManifestId);
                            }

                            OnRequestChooseDeletionCandidate?.Invoke(DeletionCandidatesIds, StorageHeuristic, StoragePrioritizeKeepingBuildTagIds, StoragePrioritizeDeletingBuildTagIds);
                        }

                        TimeSinceLastPruneRequest = TimeUtils.Ticks;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public bool CleanUpOrphanBuilds()
        {
            bool Result = false;

            foreach (StorageLocation Location in Locations)
            {
                string BuildPath = Path.Combine(Location.Path, @"Builds");

                if (Directory.Exists(BuildPath))
                {
                    // Check if there are any folders in the storage directory that do not have a manifest associated with them.
                    foreach (string Dir in Directory.GetDirectories(BuildPath))
                    {
                        Guid ManifestId = Guid.Empty;

                        if (Guid.TryParse(Path.GetFileName(Dir), out ManifestId))
                        {
                            // This download is currently being worked on, don't try and clean it up.
                            if (DownloadManager.IsDownloadBlocked(ManifestId))
                            {
                                continue;
                            }

                            BuildManifest Manifest = ManifestRegistry.GetManifestById(ManifestId);
                            if (Manifest == null)
                            {
                                lock (PendingOrphanCleanups)
                                {
                                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Deleting directory in storage folder that appears to have no matching manifest (probably a previous failed delete): {0}", Dir);
                                    if (!PendingOrphanCleanups.Contains(Dir))
                                    {
                                        PendingOrphanCleanups.Add(Dir);
                                        IOQueue.DeleteDir(Dir, bSuccess => { PendingOrphanCleanups.Remove(Dir); });
                                        Result = true;
                                    }
                                }
                            }
                            else
                            {
                                // If we have a manifest but no download state add as local download
                                // with no available blocks so we can clean it up if needed for space.
                                if (DownloadManager.GetDownload(ManifestId) == null)
                                {
                                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Found build directory for manifest, but no download state, added as local download, might have been orphaned due to settings save failure?: {0}", Dir);
                                    DownloadManager.AddLocalDownload(Manifest, Dir, false);
                                    Result = true;
                                }
                            }
                        }
                    }
                }
            }

            return Result;
        }

    }
}
