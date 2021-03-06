﻿/*
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
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Tags;

namespace BuildSync.Core.Manifests
{
    /// </summary>
    public delegate void TagsUpdatedEventHandler();

    /// <summary>
    /// </summary>
    public class BuildManifestRegistry
    {
        /// <summary>
        /// </summary>
        private VirtualFileSystem ManifestFileSystem = new VirtualFileSystem();

        /// <summary>
        /// </summary>
        public bool ManifestLastSeenTimesDirty;

        /// <summary>
        /// </summary>
        public List<BuildManifest> Manifests = new List<BuildManifest>();

        /// <summary>
        /// </summary>
        private TagRegistry TagRegistry = null;

        /// <summary>
        /// 
        /// </summary>
        private int MaximumManifests;

        /// <summary>
        /// 
        /// </summary>
        private string RootPath;

        /// <summary>
        /// </summary>
        public Dictionary<string, DateTime> ManifestLastSeenTimes { get; set; } = new Dictionary<string, DateTime>();

        /// <summary>
        /// 
        /// </summary>
        public BuildManifestRegistry(TagRegistry InTagRegistry)
        {
            TagRegistry = InTagRegistry;
            TagRegistry.TagDeleted += (Guid TagId) =>
            {
                // Remove tags from all manifests.
                foreach (BuildManifest Manifest in Manifests)
                {
                    if (Manifest.Metadata != null && Manifest.Metadata.TagIds.Contains(TagId))
                    {
                        Manifest.Metadata.TagIds.Remove(TagId);
                        StoreMetadata(Manifest);
                    }
                }
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="Manifest"></param>
        public void AddManifest(BuildManifest Manifest)
        {
            ManifestFileSystem.InsertNode(Manifest.VirtualPath, DateTime.UtcNow, Manifest);
            Manifests.Add(Manifest);

            PruneManifests();
        }

        /// <summary>
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public BuildManifest GetManifestById(Guid Id)
        {
            foreach (BuildManifest Manifest in Manifests)
            {
                if (Manifest.Guid == Id)
                {
                    return Manifest;
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public byte[] GetManifestBytesByid(Guid Id)
        {
            string FilePath = Path.Combine(RootPath, Id + ".manifest");
            if (File.Exists(FilePath))
            {
                return File.ReadAllBytes(FilePath);
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public BuildManifest GetManifestByPath(string Path)
        {
            VirtualFileSystemNode Node = ManifestFileSystem.GetNodeByPath(Path);
            if (Node != null)
            {
                return Node.Metadata as BuildManifest;
            }

            return null;
        }

        /// <summary>
        /// </summary>
        public List<string> GetVirtualPathChildren(string Path)
        {
            return ManifestFileSystem.GetChildrenNames(Path);
        }

        /// <summary>
        /// </summary>
        /// <param name="ManifestId"></param>
        public void MarkAsSeen(Guid ManifestId)
        {
            string Id = ManifestId.ToString();
            lock (ManifestLastSeenTimes)
            {
                if (!ManifestLastSeenTimes.ContainsKey(Id))
                {
                    ManifestLastSeenTimes.Add(Id, DateTime.UtcNow);
                }
                else
                {
                    ManifestLastSeenTimes[Id] = DateTime.UtcNow;
                }

                ManifestLastSeenTimesDirty = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <returns></returns>
        public DateTime GetLastSeenTime(Guid ManifestId)
        {
            string Id = ManifestId.ToString();
            lock (ManifestLastSeenTimes)
            {
                if (ManifestLastSeenTimes.ContainsKey(Id))
                {
                    return ManifestLastSeenTimes[Id];
                }
            }
            return DateTime.UtcNow;
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public void Open(string Path, int InMaxManifests)
        {
            MaximumManifests = InMaxManifests;
            RootPath = Path;

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Loading build manfiests from: {0}", Path);

            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            string[] ManifestFilePaths = Directory.GetFiles(Path, "*.manifest", SearchOption.AllDirectories);

            List<Task> Tasks = new List<Task>();
            List<BuildManifest> Results = new List<BuildManifest>();

            foreach (string FilePath in ManifestFilePaths)
            {
                Tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        Logger.Log(LogLevel.Verbose, LogCategory.Manifest, "Loading manifest: {0}", FilePath);

                        BuildManifest Manifest = BuildManifest.ReadFromFile(FilePath);
                        if (Manifest != null)
                        {
                            string MetaFile = FilePath + ".metadata";
                            if (File.Exists(MetaFile))
                            {
                                Logger.Log(LogLevel.Verbose, LogCategory.Manifest, "Loading manifest metadata: {0}", MetaFile);
                                Manifest.Metadata = BuildManifestMetadata.ReadFromFile(MetaFile);
                            }

                            if (Manifest.Metadata == null)
                            {
                                Manifest.Metadata = new BuildManifestMetadata();
                            }

                            lock (Results)
                            {
                                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Loaded Manifest (Version {2}): {0} -> {1}", Manifest.Guid.ToString(), Manifest.VirtualPath, Manifest.Version);

                                Results.Add(Manifest);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to read manifest '{0}', due to error {1}", FilePath, ex.Message);
                    //    File.Delete(FilePath);
                    }
                }));
            }

            Task.WaitAll(Tasks.ToArray());

            GC.Collect();

            foreach (BuildManifest Manifest in Results)
            {
                AddManifest(Manifest);
            }

            watch.Stop();
            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Loaded all manifests in {0}ms", watch.ElapsedMilliseconds);

            PruneManifests();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            foreach (BuildManifest Manifest in Manifests)
            {
                Manifest.Poll();
            }
        }

        /// <summary>
        /// </summary>
        public void PruneManifests()
        {
            if (Manifests.Count <= MaximumManifests)
            {
                return;
            }

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Pruning manifests to max limits");

            // Create a flat list of folders and all builds they contain, we will prune starting from the
            // folders with the largest number of builds.
            Dictionary<string, List<BuildManifest>> Folders = new Dictionary<string, List<BuildManifest>>();
            foreach (BuildManifest Manifest in Manifests)
            {
                string Folder = VirtualFileSystem.GetParentPath(Manifest.VirtualPath);
                if (!Folders.ContainsKey(Folder))
                {
                    Folders.Add(Folder, new List<BuildManifest>());
                }

                List<BuildManifest> List = Folders[Folder];
                List.Add(Manifest);
            }

            // Sort all folders.
            foreach (KeyValuePair<string, List<BuildManifest>> Entry in Folders)
            {
                Entry.Value.Sort((Item1, Item2) => -Item1.CreateTime.CompareTo(Item2.CreateTime));
            }

            // Prune until we are back in space constraints.
            while (Manifests.Count > MaximumManifests)
            {
                // Find folder with the most entries.
                List<BuildManifest> Folder = null;
                foreach (KeyValuePair<string, List<BuildManifest>> Entry in Folders)
                {
                    if (Folder == null || Entry.Value.Count > Folder.Count)
                    {
                        Folder = Entry.Value;
                    }
                }

                // Last entry is oldests.
                BuildManifest Manifest = Folder[Folder.Count - 1];
                Folder.RemoveAt(Folder.Count - 1);

                UnregisterManifest(Manifest.Guid);
            }
        }

        /// <summary>
        /// </summary>
        public void PruneUnseenManifests(int MaxDaysOld)
        {
            lock (ManifestLastSeenTimes)
            {
                for (int i = 0; i < Manifests.Count; i++)
                {
                    BuildManifest Manifest = Manifests[i];
                    string Id = Manifest.Guid.ToString();

                    if (ManifestLastSeenTimes.ContainsKey(Id))
                    {
                        TimeSpan Elapsed = DateTime.UtcNow - ManifestLastSeenTimes[Id];
                        if (Elapsed.TotalDays > MaxDaysOld)
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Pruning manifest '{0}' '{1}' as it's {2} days since it was last seen on any peer.", Manifest.Guid.ToString(), Manifest.VirtualPath, (int)Elapsed.TotalDays);

                            UnregisterManifest(Manifest.Guid);
//                            ManifestFileSystem.RemoveNode(Manifest.VirtualPath);
                            //Manifests.RemoveAt(i);
                            i--;

                            ManifestLastSeenTimesDirty = true;
                        }
                    }
                    else
                    {
                        // Put an initial timestamp in for this manifest.
                        MarkAsSeen(Manifest.Guid);

                        ManifestLastSeenTimesDirty = true;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public void RegisterManifest(BuildManifest Manifest)
        {
            string FilePath = Path.Combine(RootPath, Manifest.Guid + ".manifest");

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Registering manifest: {0}", FilePath);

            if (Manifest.Metadata == null)
            {
                Manifest.Metadata = new BuildManifestMetadata();
            }

            Manifest.WriteToFile(FilePath);
            AddManifest(Manifest);

            StoreMetadata(Manifest);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StoreMetadata(BuildManifest Manifest)
        {
            string MetaFile = Path.Combine(RootPath, Manifest.Guid + ".manifest") + ".metadata";

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Storing manifest metadata: {0}", MetaFile);

            if (Manifest.Metadata != null)
            {
                Manifest.Metadata.WriteToFile(MetaFile);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Tag"></param>
        public void TagManifest(Guid ManifestId, Guid TagId)
        {
            BuildManifest Manifest = GetManifestById(ManifestId);
            if (Manifest == null)
            {
                return;
            }

            Tag Tag = TagRegistry.GetTagById(TagId);
            if (Tag == null)
            {
                return;
            }

            if (Manifest.Metadata != null && !Manifest.Metadata.TagIds.Contains(TagId))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Tagging manifest {0} with {1}", ManifestId.ToString(), Tag.Name);

                Manifest.Metadata.ModifiedTime = DateTime.UtcNow;
                Manifest.Metadata.TagIds.Add(TagId);
                StoreMetadata(Manifest);
            }

            if (Tag.Unique)
            {
                string ParentPath = VirtualFileSystem.GetParentPath(Manifest.VirtualPath);
                string NodeName = VirtualFileSystem.GetNodeName(Manifest.VirtualPath);

                List<string> Children = GetVirtualPathChildren(ParentPath);

                Tag DecayTag = TagRegistry.GetTagById(Tag.DecayTagId);
                
                foreach (string ChildName in Children)
                {
                    if (ChildName == Manifest.VirtualPath)
                    {
                        continue;
                    }

                    BuildManifest ChildManifest = GetManifestByPath(ChildName);
                    if (ChildManifest != null)
                    {
                        if (ChildManifest.Metadata != null && ChildManifest.Metadata.TagIds.Contains(TagId))
                        {
                            UntagManifest(ChildManifest.Guid, TagId);

                            if (DecayTag != null)
                            {
                                TagManifest(ChildManifest.Guid, DecayTag.Id);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        /// <param name="Tag"></param>
        public void UntagManifest(Guid ManifestId, Guid TagId)
        {
            BuildManifest Manifest = GetManifestById(ManifestId);
            if (Manifest == null)
            {
                return;
            }

            Tag Tag = TagRegistry.GetTagById(TagId);
            if (Tag == null)
            {
                return;
            }

            if (Manifest.Metadata != null && Manifest.Metadata.TagIds.Contains(TagId))
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Unagging manifest {0} with {1}", ManifestId.ToString(), Tag.Name);

                Manifest.Metadata.ModifiedTime = DateTime.UtcNow;
                Manifest.Metadata.TagIds.Remove(TagId);
                StoreMetadata(Manifest);
            }
        }

        /// <summary>
        /// </summary>
        public void UnregisterManifest(Guid ManifestId)
        {
            BuildManifest Manifest = GetManifestById(ManifestId);
            if (Manifest == null)
            {
                return;
            }

            string FilePath = Path.Combine(RootPath, ManifestId + ".manifest");
            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Unregistering manifest: {0}", FilePath);

            try
            {
                File.Delete(FilePath);

                string MetaFile = FilePath + ".metadata";
                if (File.Exists(MetaFile))
                {
                    Logger.Log(LogLevel.Info, LogCategory.Manifest, "Unregistering manifest metadata: {0}", MetaFile);
                    File.Delete(MetaFile);
                }
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to delete unregistered manifest file {0} with error: {1}", FilePath, Ex.Message);
            }

            ManifestFileSystem.RemoveNode(Manifest.VirtualPath);
            Manifests.Remove(Manifest);
        }
    }
}