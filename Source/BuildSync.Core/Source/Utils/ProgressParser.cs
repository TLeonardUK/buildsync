/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this sofPtware must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public enum ProgressMatchType
    {
        FileProgress,
        CurrentFile,
        FileCount,
        CurrentFileName,
        Progress
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ProgressMatchFormat
    {
        StringLength,
        Integer,
        Float,
        String
    }

    /// <summary>
    /// 
    /// </summary>
    public struct ProgressMatch
    {
        public ProgressMatchType Type;
        public ProgressMatchFormat Format;
        public float NormalizeValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InType"></param>
        /// <param name="InFormat"></param>
        /// <param name="InNormalizeValue"></param>
        public ProgressMatch(ProgressMatchType InType, ProgressMatchFormat InFormat, float InNormalizeValue = 0.0f)
        {
            Type = InType;
            Format = InFormat;
            NormalizeValue = InNormalizeValue;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProgressPattern
    {
        public string Pattern;
        public ProgressMatch[] Matches;
        public Regex PatternRegex = null;

        /// <summary>
        /// 
        /// </summary>
        public bool Compile()
        {
            try
            {
                PatternRegex = new Regex(Pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            catch (ArgumentException Ex)
            {
                Logger.Log(LogLevel.Info, LogCategory.Script, "Failed to compile progress regex '{0}' with error: {1}", Pattern, Ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InType"></param>
        /// <param name="InFormat"></param>
        /// <param name="InNormalizeValue"></param>
        public ProgressPattern(string InPattern, ProgressMatch[] InMatches)
        {
            Pattern = InPattern;
            Matches = InMatches;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProgressParser
    {
        private bool Compiled = false;
        private bool CompileSuccess = false;
        private List<ProgressPattern> Patterns = new List<ProgressPattern>();

        public float FileProgress = -1.0f;
        public int CurrentFile = -1;
        public int FileCount = -1;
        public string CurrentFileName = "";

        public float Progress = 0;
        public bool ProgressExplicitlySet = false;

        public bool ParsePartialLines = true;

        public string LineSeperator = "\n";

        private LineBuilder InputBuilder = new LineBuilder();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Pattern"></param>
        /// <param name="Matches"></param>
        public void AddPattern(string Pattern, params ProgressMatch[] Matches)
        {
            Patterns.Add(new ProgressPattern(Pattern, Matches));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Compile()
        {
            Compiled = true;
            CompileSuccess = false;

            foreach (ProgressPattern Pattern in Patterns)
            {
                if (!Pattern.Compile())
                {
                    return;
                }
            }

            CompileSuccess = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        private string ParseValue(string Value, ProgressMatchFormat Format, float NormalizeValue)
        {
            string Result = Value;
            switch (Format)
            {
                case ProgressMatchFormat.StringLength:
                    {
                        Result = Value.Length.ToString();
                        break;
                    }
            }
            if (NormalizeValue > 0.0f)
            {
                float FloatValue = 0.0f;
                if (float.TryParse(Result, out FloatValue))
                {
                    if (Format == ProgressMatchFormat.Integer)
                    {
                        Result = ((int)(FloatValue / NormalizeValue)).ToString();
                    }
                    else
                    {
                        Result = (FloatValue / NormalizeValue).ToString();
                    }
                }
            }
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Input"></param>
        private void ParseLine(string Input)
        {
            foreach (ProgressPattern Pattern in Patterns)
            {
                try
                {
                    Match match = Pattern.PatternRegex.Match(Input);
                    if (match == null ||
                        !match.Success ||
                        match.Groups.Count - 1 != Pattern.Matches.Length)
                    {
                        continue;
                    }

                    for (int i = 0; i < Pattern.Matches.Length; i++)
                    {
                        string Value = match.Groups[i + 1].Value;
                        ProgressMatch Match = Pattern.Matches[i];

                        Value = ParseValue(Value, Match.Format, Match.NormalizeValue);

                        switch (Match.Type)
                        {
                            case ProgressMatchType.CurrentFile: int.TryParse(Value, out CurrentFile); break;
                            case ProgressMatchType.CurrentFileName: CurrentFileName = Value; break;
                            case ProgressMatchType.FileCount: int.TryParse(Value, out FileCount); break;
                            case ProgressMatchType.FileProgress: float.TryParse(Value, out FileProgress); break;
                            case ProgressMatchType.Progress: float.TryParse(Value, out Progress); ProgressExplicitlySet = true;  break;
                        }
                    }

                    if (!ProgressExplicitlySet)
                    {
                        if (FileCount > 0 && CurrentFile > 0)
                        {
                            float FileChunk = 1.0f / (float)FileCount;

                            if (FileProgress > 0.0f)
                            {
                                Progress = (FileChunk * (CurrentFile - 1)) + (FileChunk * FileProgress);
                            }
                            else
                            {
                                Progress = FileChunk * (CurrentFile - 1);
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Script, "Failed to run progress regex on output with error: {0}", Ex.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Input"></param>
        public void Parse(string Input)
        {
            if (Input == null)
            {
                return;
            }
            if (!Compiled)
            {
                Compile();
            }
            if (!CompileSuccess)
            {
                return;
            }

            InputBuilder.Seperator = LineSeperator;
            InputBuilder.Add(Input);
            while (true)
            {
                string Line = InputBuilder.Read();
                if (Line == null)
                {
                    // We always try to match the currently building line,
                    // as we may want to match progress for a character/by/character progress bar.
                    if (ParsePartialLines)
                    {
                        Line = InputBuilder.GetUnparsed();
                        if (Line != "")
                        {
                            ParseLine(Line);
                        }
                    }
                    break;
                }
                ParseLine(Line);
            }
        }
    }
}

