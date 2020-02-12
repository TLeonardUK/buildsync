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

namespace BuildSync.Core.Utils
{

    /// <summary>
    /// 
    /// </summary>
    public class FileCache
    {
        /// <summary>
        /// 
        /// </summary>
        private class Entry
        {
            public string Path;
            public string Contents;
            public DateTime LastModified;
            public bool NeedsUpdate;
            public FileSystemWatcher Watcher;
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, Entry> Entries = new Dictionary<string, Entry>();

        /// <summary>
        /// 
        /// </summary>
        private const int RecheckedInterval = 10 * 1000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        private void UpdateEntry(Entry entry)
        {
            if (entry.NeedsUpdate)
            {
                FileInfo info = new FileInfo(entry.Path);
                if (info.Exists)
                {
                    if (info.LastWriteTimeUtc > entry.LastModified)
                    {
                        entry.Contents = File.ReadAllText(entry.Path);
                        entry.LastModified = info.LastWriteTimeUtc;

                        Logger.Log(LogLevel.Info, LogCategory.IO, "Updated cache entry for file: {0}", entry.Path);
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, LogCategory.IO, "Failed to read and cache file: {0}", entry.Path);
                }

                entry.NeedsUpdate = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private Entry GetCache(string InPath)
        {
            InPath = FileUtils.NormalizePath(InPath);

            Entry Result = null; 
            if (Entries.TryGetValue(InPath, out Result))
            {
                UpdateEntry(Result);
                return Result;
            }

            Result = new Entry();
            Result.Path = InPath;
            Result.LastModified = DateTime.MinValue;
            Result.NeedsUpdate = true;
            Result.Contents = "";
            Result.Watcher = new FileSystemWatcher();
            Result.Watcher.Path = Path.GetDirectoryName(InPath);
            Result.Watcher.Filter = Path.GetFileName(InPath);
            Result.Watcher.EnableRaisingEvents = true;
            Result.Watcher.Changed += (object sender, FileSystemEventArgs e) =>
            {
                Result.NeedsUpdate = true;
            };
            Entries.Add(Result.Path, Result);

            UpdateEntry(Result);
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public string Get(string Path)
        {
            return GetCache(Path).Contents;
        }
    }
}
