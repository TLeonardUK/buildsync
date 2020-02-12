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

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringUtils
    {
        public static string[] RatePrefixes = new string[] { "bits/s", "Kb/s", "Mb/s", "Gb/s", "Tb/s" };
        public static string[] RatePrefixesBytes = new string[] { "bytes/s", "KB/s", "MB/s", "GB/s", "TB/s" };
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
            return string.Format("{0:0.0} {1}", Result, Prefixes[PrefixIndex]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rate"></param>
        /// <returns></returns>
        public static long TransferRateFormatToBytes(string input)
        {
            int Multiplier = 1;
            int PrefixIndex = 0;
            string Prefix = "";
            while (PrefixIndex < RatePrefixesBytes.Length - 1)
            {
                Prefix = RatePrefixesBytes[PrefixIndex];
                if (input.EndsWith(Prefix))
                {
                    break;
                }

                Multiplier *= 1024;
                PrefixIndex++;
            }

            if (PrefixIndex < RatePrefixesBytes.Length)
            {
                double Value = 0;
                string PrefixRemoved = input.Substring(0, input.Length - Prefix.Length - 1);
                if (double.TryParse(PrefixRemoved, out Value))
                {
                    return (long)(Value * Multiplier);
                }
            }

            return 0;
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
        /// <param name="Rate"></param>
        /// <returns></returns>
        public static long SizeFormatToBytes(string input)
        {
            int Multiplier = 1;
            int PrefixIndex = 0;
            string Prefix = "";
            while (PrefixIndex < SizePrefixes.Length - 1)
            {
                Prefix = SizePrefixes[PrefixIndex];
                if (input.EndsWith(Prefix))
                {
                    break;
                }

                Multiplier *= 1024;
                PrefixIndex++;
            }

            if (PrefixIndex < SizePrefixes.Length)
            {
                double Value = 0;
                string PrefixRemoved = input.Substring(0, input.Length - Prefix.Length - 1);
                if (double.TryParse(PrefixRemoved, out Value))
                {
                    return (long)(Value * Multiplier);
                }
            }

            return 0;
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
