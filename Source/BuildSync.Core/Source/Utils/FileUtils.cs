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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// </summary>
        /// <param name="DirPath"></param>
        public static bool AnyRunningProcessesInDirectory(string DirPath)
        {
            DirPath = NormalizePath(DirPath);
            Process[] procs = Process.GetProcesses();
            foreach (Process proc in procs)
            {
                string ModuleName = WindowUtils.GetProcessName(proc.Id);
                if (ModuleName == null)
                {
                    continue;
                }

                string File = NormalizePath(ModuleName);
                if (File.ToLower().StartsWith(DirPath))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }

            return output.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }

            return output.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="DirPath"></param>
        public static void DeleteDirectory(string DirPath)
        {
            string[] Files = Directory.GetFiles(DirPath);
            string[] Dirs = Directory.GetDirectories(DirPath);

            foreach (string file in Files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in Dirs)
            {
                DeleteDirectory(dir);
            }

            try
            {
                Directory.Delete(DirPath, false);
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.IO, "Unable to delete directory {0} with error: {1}", DirPath, Ex.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string GetChecksum(string FilePath, RateTracker Tracker, ChecksumProgressEventHandler Callback = null)
        {
            using (Crc32 crc = new Crc32())
            {
                using (FileStream stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] hash = crc.ComputeLargeStreamHash(stream, Tracker, Callback);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GetTempDirectory()
        {
            string TempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(TempDirectory);
            return TempDirectory;
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            try
            {
                return Path.GetFullPath(new Uri(path).LocalPath)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .ToUpperInvariant();
            }
            catch (UriFormatException)
            {
                return path;
            }
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T ReadFromArray<T>(byte[] Data)
        {
            using (MemoryStream stream = new MemoryStream(Decompress(Data)))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T) binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            byte[] Data = File.ReadAllBytes(filePath);
            return ReadFromArray<T>(Data);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToWrite"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static byte[] WriteToArray<T>(T objectToWrite)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);

                return Compress(stream.ToArray());
            }
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="objectToWrite"></param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite)
        {
            byte[] Data = WriteToArray(objectToWrite);
            File.WriteAllBytes(filePath, Data);
        }
    }
}