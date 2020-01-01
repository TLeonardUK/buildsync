using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringUtils
    {
        public static string[] RatePrefixes = new string[] { "bits", "Kb", "Mb", "Gb", "Tb" };
        public static string[] SizePrefixes = new string[] { "bytes", "KB", "MB", "GB", "TB" };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rate"></param>
        /// <returns></returns>
        public static string FormatAsTransferRate(long RateBytes)
        {
            double Result = RateBytes * 8; // This converst 
            int PrefixIndex = 0;
            while (Result > 1024 && PrefixIndex < RatePrefixes.Length - 1)
            {
                Result /= 1024;
                PrefixIndex++;
            }
            return string.Format("{0:0.##} {1}/s", Result, RatePrefixes[PrefixIndex]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rate"></param>
        /// <returns></returns>
        public static string FormatAsSize(long RateBytes)
        {
            double Result = RateBytes;
            int PrefixIndex = 0;
            while (Result > 1024 && PrefixIndex < SizePrefixes.Length - 1)
            {
                Result /= 1024;
                PrefixIndex++;
            }
            return string.Format("{0:0.##} {1}", Result, SizePrefixes[PrefixIndex]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Seconds"></param>
        /// <returns></returns>
        public static string FormatAsDuration(long Time)
        {
            long Seconds = Time % 60;
            Time = Time / 60;

            long Minutes = Time % 60;
            Time = Time / 60;

            long Hours = Time;

            return string.Format("{0:D2}:{1:D2}:{2:D2}", Hours, Minutes, Seconds);
        }
    }
}
