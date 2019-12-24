using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Manifests
{
    /// <summary>
    /// 
    /// </summary>
    public class BuildManifestRegistry
    {
        private string RootPath;

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

            Console.WriteLine("Registering manifest: {0}", FilePath);

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
            Console.WriteLine("Unregistering manifest: {0}", FilePath);

            try
            {
                File.Delete(FilePath);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Failed to delete unregistered manifest file {0} with error: {1}", FilePath, Ex.Message);
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
            ManifestFileSystem.InsertNode(Manifest.VirtualPath, Manifest);
            Manifests.Add(Manifest);
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
        public void Open(string Path)
        {
            Console.WriteLine("Loading build manfiests from: {0}", Path);

            UpdateStoragePath(Path);

            string[] ManifestFilePaths = Directory.GetFiles(Path, "*.manifest", SearchOption.AllDirectories);
            foreach (string FilePath in ManifestFilePaths)
            {
                Console.WriteLine("Loading manifest: {0}", FilePath);

                try
                { 
                    BuildManifest Manifest = BuildManifest.ReadFromFile(FilePath);
                    AddManifest(Manifest);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to read manifest '{0}', due to error {1}, deleting manifest.", FilePath, ex.Message);
                    File.Delete(FilePath);
                }
            }
        }
    }
}
