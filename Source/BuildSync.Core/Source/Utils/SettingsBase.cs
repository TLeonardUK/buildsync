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
using System.Text.Encodings.Web;
using System.Text.Json;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// </summary>
    public class SettingsBase
    {
        /// <summary>
        /// </summary>
        /// <param name="FullFilePath"></param>
        /// <param name="SaveItem"></param>
        /// <returns></returns>
        public static bool Load<T>(string FullFilePath, out T SaveItem)
            where T : new()
        {
            bool Result = false;
            SaveItem = new T();

            string BackupPath = FullFilePath + ".backup";

            try
            {
                JsonSerializerOptions Options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string Text = File.ReadAllText(FullFilePath);
                SaveItem = (T) JsonSerializer.Deserialize(Text, typeof(T), Options);
                if (SaveItem != null)
                {
                    Result = true;

                    if (File.Exists(BackupPath))
                    {
                        File.Delete(BackupPath);
                    }
                }
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Failed to load '{0}' with error: {1}", FullFilePath, Ex.Message);

                // Try and load backup.
                if (File.Exists(BackupPath))
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "Backup setings file found '{0}', attempting to load.", BackupPath);

                    Result = Load(BackupPath, out SaveItem);
                    if (!Result)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Failed to load settings, aborting so as not to lose data.");
                        Environment.FailFast("Failed to load configuration files.");
                    }

                    //File.Delete(BackupPath);
                }
            }

            return Result;
        }

        /// <summary>
        /// </summary>
        /// <param name="FullFilePath"></param>
        /// <returns></returns>
        public bool Save(string FullFilePath)
        {
            bool Result = false;
            try
            {
                string DirPath = Path.GetDirectoryName(FullFilePath);
                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                }

                if (File.Exists(FullFilePath))
                {
                    File.Copy(FullFilePath, FullFilePath + ".backup", true);
                }

                using (TextWriter TextWriter = new StreamWriter(FullFilePath))
                {
                    JsonSerializerOptions Options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    string Data = JsonSerializer.Serialize(this, GetType(), Options);
                    TextWriter.Write(Data);
                    Result = true;
                }
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Failed to save '{0}' with error: {1}", FullFilePath, Ex.Message);
            }

            return Result;
        }
    }
}