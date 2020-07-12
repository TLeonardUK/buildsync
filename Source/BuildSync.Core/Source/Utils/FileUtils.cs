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

//#define LOG_COMPRESS_RATIO

using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using Snappy;

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
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Fastest))
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
        /// 
        /// </summary>
        public static ThreadLocal<byte[]> CompressBuffer = new ThreadLocal<byte[]>(() => new byte[2 * 1024 * 1024]);
        public static object CompressLock = new object();

#if LOG_COMPRESS_RATIO
        /// <summary>
        /// 
        /// </summary>
        public static long CompressedBytes = 0;
        public static long UncompressedBytes = 0;
#endif

        /// <summary>
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static bool CompressInPlace(byte[] Data, long DataOffset, long DataLength, out long ResultLength)
        {
            byte[] Buffer = CompressBuffer.Value;

#if LOG_COMPRESS_RATIO
            Stopwatch watch = new Stopwatch();
            watch.Start();
#endif

            int MaxBufferSize = SnappyCodec.GetMaxCompressedLength((int)DataLength);
            if (Buffer.Length < MaxBufferSize)
            {
                Buffer = new byte[MaxBufferSize];
                CompressBuffer.Value = Buffer;
            }

            ResultLength = SnappyCodec.Compress(Data, (int)DataOffset, (int)DataLength, Buffer, 0);
            if (ResultLength < DataLength)
            {
                Array.Copy(Buffer, 0, Data, (int)DataOffset, ResultLength);

#if LOG_COMPRESS_RATIO
                watch.Stop();
                float elapsed = ((float)watch.ElapsedTicks / (float)Stopwatch.Frequency) * 1000.0f;
                CompressedBytes += ResultLength;
                UncompressedBytes += DataLength;

                float Reduction = ((float)CompressedBytes / (float)UncompressedBytes) * 100.0f;

                Console.WriteLine("CompressInPlace: ratio={0} elapsed={1}ms compressed={2} uncompressed={3} saved={4}", 
                    Reduction, 
                    elapsed,
                    StringUtils.FormatAsSize(CompressedBytes),
                    StringUtils.FormatAsSize(UncompressedBytes),
                    StringUtils.FormatAsSize(UncompressedBytes - CompressedBytes));
#endif

                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static int GetDecompressedLength(byte[] Data, long DataOffset, long DataLength)
        {
            return SnappyCodec.GetUncompressedLength(Data, (int)DataOffset, (int)DataLength);
        }

        /// <summary>
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static bool DecompressInPlace(byte[] Data, long DataOffset, long DataLength, out long ResultLength)
        {
            byte[] Buffer = CompressBuffer.Value;

            int MaxBufferSize = SnappyCodec.GetUncompressedLength(Data, (int)DataOffset, (int)DataLength);
            if (Buffer.Length < MaxBufferSize)
            {
                Buffer = new byte[MaxBufferSize];
                CompressBuffer.Value = Buffer;
            }

#if LOG_COMPRESS_RATIO
            Stopwatch watch = new Stopwatch();
            watch.Start();
#endif

            ResultLength = SnappyCodec.Uncompress(Data, (int)DataOffset, (int)DataLength, Buffer, 0);
            if (ResultLength <= Buffer.Length)
            {
                Array.Copy(Buffer, 0, Data, (int)DataOffset, ResultLength);

#if LOG_COMPRESS_RATIO
                watch.Stop();
                float elapsed = ((float)watch.ElapsedTicks / (float)Stopwatch.Frequency) * 1000.0f;

                CompressedBytes += ResultLength;
                UncompressedBytes += DataLength;

                float Reduction = ((float)CompressedBytes / (float)UncompressedBytes) * 100.0f;

                Console.WriteLine("DecompressInPlace: elapsed={0}ms", elapsed);
#endif

                return true;
            }
            else
            {
                return false;
            }
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
                FileInfo fileInfo = new FileInfo(file);
                if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }

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
            using (SHA1Extended crc = new SHA1Extended())
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static bool PathFileExists(StringBuilder path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public static bool FileExistsFast(string Path)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Path);
            return PathFileExists(builder);
        }
    }
}