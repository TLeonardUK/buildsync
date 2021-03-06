﻿/*
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
    public class LineBuilder
    {
        private string UnparsedInput = "";
        private bool Ended = false;

        public string Seperator = "\n";

        public string GetUnparsed()
        {
            return UnparsedInput;
        }

        public void Add(string Input)
        {
            UnparsedInput += Input;
        }

        public string Read()
        {
            int LineEnd = UnparsedInput.IndexOf(Seperator);
            if (LineEnd < 0)
            {
                if (Ended && UnparsedInput.Length > 0)
                {
                    string Value = UnparsedInput;
                    UnparsedInput = "";
                    return Value;
                }
                else
                {
                    return null;
                }
            }

            string Line = UnparsedInput.Substring(0, LineEnd);
            if (Seperator == "\n")
            {
                Line = Line.Trim('\r');
            }

            UnparsedInput = UnparsedInput.Substring(LineEnd + 1);
            if (Seperator == "\n")
            {
                UnparsedInput = UnparsedInput.Trim('\r');
            }

            return Line;
        }

        public void End()
        {
            Ended = true;
        }
    }

    /// <summary>
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// 
        /// </summary>
        public static string[] RatePrefixes = {"bits/s", "Kb/s", "Mb/s", "Gb/s", "Tb/s"};

        /// <summary>
        /// 
        /// </summary>
        public static string[] RatePrefixesBytes = {"bytes/s", "KB/s", "MB/s", "GB/s", "TB/s"};

        /// <summary>
        /// 
        /// </summary>
        public static string[] SizePrefixes = {"bytes", "KB", "MB", "GB", "TB"};
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetStableHashCode(this string input) 
        { 
            // Same as string.GetHashCode, but only uses the 32bit version.
            unsafe 
            {
                fixed (char *src = input) 
                { 
                    int hash1 = (5381<<16) + 5381;
                    int hash2 = hash1;
 
                    int* pint = (int *)src;
                    int len = input.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len  -= 4;
                    }
 
                    if (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    }

                    return hash1 + (hash2 * 1566083941);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static int ConvertSemanticVerisonNumber(string Value)
        {
            string[] splits = Value.Split('.');
            if (splits.Length != 4)
            {
                return 0;
            }

            int major = 0;
            int minor = 0;
            int patch = 0;
            int build = 0;

            if (!int.TryParse(splits[0], out major))
            {
                return 0;
            }
            if (!int.TryParse(splits[1], out minor))
            {
                return 0;
            }
            if (!int.TryParse(splits[2], out patch))
            {
                return 0;
            }
            if (!int.TryParse(splits[3], out build))
            {
                return 0;
            }

            return major * 100000000 + minor * 1000000 + patch * 10000 + build;
        }

        /// <summary>
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
        /// </summary>
        /// <param name="Seconds"></param>
        /// <returns></returns>
        public static string FormatAsTextDuration(long Time)
        {
            long Seconds = Time % 60;
            Time = Time / 60;

            long Minutes = Time % 60;
            Time = Time / 60;

            long Hours = Time;

            if (Hours > 0)
            {
                return string.Format("{0} hour{1} {2} minute{3}", Hours, Hours > 1 ? "s" : "", Minutes, Minutes > 1 ? "s" : "");
            }
            if (Minutes > 0)
            {
                return string.Format("{0} minute{1}", Minutes, Minutes > 1 ? "s" : "");
            }
            if (Seconds > 0)
            {
                return string.Format("{0} second{1}", Seconds, Seconds > 1 ? "s" : "");
            }
            return "-";
        }

        /// <summary>
        /// </summary>
        /// <param name="Rate"></param>
        /// <returns></returns>
        public static string FormatAsSize(long RateBytes)
        {
            bool Negative = false;
            if (RateBytes < 0)
            {
                Negative = true;
                RateBytes = -RateBytes;
            }

            double Result = RateBytes;
            int PrefixIndex = 0;
            while (Result > 1024 && PrefixIndex < SizePrefixes.Length - 1)
            {
                Result /= 1024;
                PrefixIndex++;
            }

            return string.Format("{0:0.0} {1}", Negative ? -Result : Result, SizePrefixes[PrefixIndex]);
        }

        /// <summary>
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
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool IsValidNetAddress(string Input)
        {
            int Port = 0;

            string[] Split = Input.Split(':');

            if (Split.Length == 1 && Uri.CheckHostName(Split[0]) != UriHostNameType.Unknown ||
                Split.Length == 2 && Uri.CheckHostName(Split[0]) != UriHostNameType.Unknown && int.TryParse(Split[1], out Port))
            {
                return true;
            }

            return false;
        }

        /// <summary>
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

                int len = input.Length - Prefix.Length - 1;
                if (len < 0)
                {
                    return 0;
                }

                string PrefixRemoved = input.Substring(0, len);
                if (double.TryParse(PrefixRemoved, out Value))
                {
                    return (long) (Value * Multiplier);
                }
            }

            return 0;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string StripNonNumericTrailingPostfix(string Input)
        {
            string Result = "";
            for (int i = 0; i < Input.Length; i++)
            {
                if (char.IsNumber(Input[i]))
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
                
                int len = input.Length - Prefix.Length - 1;
                if (len < 0)
                {
                    return 0;
                }

                string PrefixRemoved = input.Substring(0, len);
                if (double.TryParse(PrefixRemoved, out Value))
                {
                    return (long) (Value * Multiplier);
                }
            }

            return 0;
        }
    }
}