using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Utils;
using System.Diagnostics;

namespace BuildSync.Core.Manifests
{
    /// <summary>
    /// 
    /// </summary>
    public class BuildManifestRegistry
    {
        private string RootPath;
        private int MaximumManifests;

        /// <summary>
        /// 
        /// </summary>
        public List<BuildManifest> Manifests = new List<BuildManifest>();

        /// <summary>
        /// 
        /// </summary>
        public VirtualFileSystem ManifestFileSystem = new VirtualFileSystem();

        /// <summary>
        /// 
        /// </summary>
        public void RegisterManifest(BuildManifest Manifest)
        {
            string FilePath = Path.Combine(RootPath, Manifest.Guid.ToString() + ".manifest");

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Registering manifest: {0}", FilePath);

            Manifest.WriteToFile(FilePath);
            AddManifest(Manifest);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnregisterManifest(Guid ManifestId)
        {
            BuildManifest Manifest = GetManifestById(ManifestId);
            if (Manifest == null)
            {
                return;
            }

            string FilePath = Path.Combine(RootPath, ManifestId.ToString() + ".manifest");
            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Unregistering manifest: {0}", FilePath);

            try
            {
                File.Delete(FilePath);
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to delete unregistered manifest file {0} with error: {1}", FilePath, Ex.Message);
            }

            ManifestFileSystem.RemoveNode(Manifest.VirtualPath);
            Manifests.Remove(Manifest);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> GetVirtualPathChildren(string Path)
        {
            return ManifestFileSystem.GetChildrenNames(Path);
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="Manifest"></param>
        public void AddManifest(BuildManifest Manifest)
        {
            ManifestFileSystem.InsertNode(Manifest.VirtualPath, DateTime.UtcNow, Manifest);
            Manifests.Add(Manifest);

            PruneManifests();
        }

        /// <summary>
        /// 
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
            foreach (var Entry in Folders)
            {
                Entry.Value.Sort((Item1, Item2) => -Item1.CreateTime.CompareTo(Item2.CreateTime));
            }

            // Prune until we are back in space constraints.
            while (Manifests.Count > MaximumManifests)
            {
                // Find folder with the most entries.
                List<BuildManifest> Folder = null;
                foreach (var Entry in Folders)
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
        /// 
        /// </summary>
        /// <param name="NewDirectory"></param>
        public void UpdateStoragePath(string NewDirectory)
        {
            RootPath = NewDirectory;

            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public void Open(string Path, int InMaxManifests)
        {
            MaximumManifests = InMaxManifests;

            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Loading build manfiests from: {0}", Path);

            UpdateStoragePath(Path);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            string[] ManifestFilePaths = Directory.GetFiles(Path, "*.manifest", SearchOption.AllDirectories);
            foreach (string FilePath in ManifestFilePaths)
            {
                Logger.Log(LogLevel.Info, LogCategory.Manifest, "Loading manifest: {0}", FilePath);

                try
                { 
                    BuildManifest Manifest = BuildManifest.ReadFromFile(FilePath);
                    AddManifest(Manifest);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to read manifest '{0}', due to error {1}, deleting manifest.", FilePath, ex.Message);
                    File.Delete(FilePath);
                }
            }

            watch.Stop();
            Logger.Log(LogLevel.Info, LogCategory.Manifest, "Loaded all manifests in {0}ms", watch.ElapsedMilliseconds);

            PruneManifests();
        }
    }
}
