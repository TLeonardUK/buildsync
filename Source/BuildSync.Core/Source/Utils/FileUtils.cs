using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetTempDirectory()
        {
            string TempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(TempDirectory);
            return TempDirectory;
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToWrite"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static byte[] WriteToArray<T>(T objectToWrite)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);

                return Compress(stream.ToArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T ReadFromArray<T>(byte[] Data)
        {
            using (MemoryStream stream = new MemoryStream(Decompress(Data)))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="objectToWrite"></param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite)
        {
            byte[] Data = WriteToArray(objectToWrite);
            File.WriteAllBytes(filePath, Data);

            /*
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            byte[] Data = File.ReadAllBytes(filePath);
            return ReadFromArray<T>(Data);

            /*
             *          using (Stream stream = File.Open(filePath, FileMode.Open))
                        {
                            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                            return (T)binaryFormatter.Deserialize(stream);
                        }
                        */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string GetChecksum(string FilePath, RateTracker Tracker, ChecksumProgressEventHandler Callback = null)
        {
            /*
            using (var md5 = MD5.Create())
            {
                using (var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }*/

            using (Crc32 crc = new Crc32())
            {
                using (var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var hash = crc.ComputeLargeStreamHash(stream, Tracker, Callback);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DirPath"></param>
        public static void DeleteDirectory(string DirPath)
        {
            File.SetAttributes(DirPath, FileAttributes.Normal);

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
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DirPath"></param>
        public static bool AnyRunningProcessesInDirectory(string DirPath)
        {
            DirPath = NormalizePath(DirPath);
            foreach (Process proc in Process.GetProcesses())
            {
                string File = NormalizePath(proc.MainModule.FileName);
                if (File.ToLower().StartsWith(DirPath))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
