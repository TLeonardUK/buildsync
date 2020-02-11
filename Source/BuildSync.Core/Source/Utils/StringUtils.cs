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
        public static string[] RatePrefixesBytes = new string[] { "bytes", "KB", "MB", "GB", "TB" };
        public static string[] SizePrefixes = new string[] { "bytes", "KB", "MB", "GB", "TB" };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string StripNonNumericTrailingPostfix(string Input)
        {
            string Result = "";
            for (int i = 0; i < Input.Length; i++)
            {
                if (Char.IsNumber(Input[i]))
                {
                    Result += Input[i];
                }
                else
                {
                    break;
                }
            }            
            return Result.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rate"></param>
        /// <returns></returns>
        public static string FormatAsTransferRate(long RateBytes, bool AsBits = false)
        {
            string[] Prefixes = AsBits ? RatePrefixes : RatePrefixesBytes;
            double Result = AsBits ? RateBytes * 8 : RateBytes;
            int PrefixIndex = 0;
            while (Result > 1024 && PrefixIndex < Prefixes.Length - 1)
            {
                Result /= 1024;
                PrefixIndex++;
            }
            return string.Format("{0:0.0} {1}/s", Result, Prefixes[PrefixIndex]);
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
            return string.Format("{0:0.0} {1}", Result, SizePrefixes[PrefixIndex]);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool IsValidNetAddress(string Input)
        {
            int Port = 0;

            string[] Split = Input.Split(':');

            if ((Split.Length == 1 && Uri.CheckHostName(Split[0]) != UriHostNameType.Unknown) ||
                (Split.Length == 2 && Uri.CheckHostName(Split[0]) != UriHostNameType.Unknown && int.TryParse(Split[1], out Port)))
            {
                return true;
            }

            return false;
        }
    }
}
