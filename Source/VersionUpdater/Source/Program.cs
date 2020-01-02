﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionUpdater
{
    public class Program
    {
        public static string PatchAssemblyTag(string Contents, string TagName, string NewValue)
        {
            string Tag = "[assembly: " + TagName + "(";

            int TagIndex = Contents.IndexOf(Tag);
            if (TagIndex < 0)
            {
                Console.WriteLine("BuildVersion not found.");
                return "";
            }
            TagIndex = TagIndex + Tag.Length;

            int EndTagIndex = Contents.IndexOf(")", TagIndex);
            if (EndTagIndex < 0)
            {
                Console.WriteLine("BuildVersion end not found.");
                return "";
            }

            return Contents.Substring(0, TagIndex) + NewValue + Contents.Substring(EndTagIndex);
        }

        public static string GetVariableValue(string Contents, string Name)
        {
            string Tag = Name + " = ";

            int TagIndex = Contents.IndexOf(Tag);
            if (TagIndex < 0)
            {
                Console.WriteLine("BuildVersion not found.");
                return "";
            }
            TagIndex = TagIndex + Tag.Length;

            int EndTagIndex = Contents.IndexOf(";", TagIndex);
            if (EndTagIndex < 0)
            {
                Console.WriteLine("BuildVersion end not found.");
                return "";
            }

            string Result = Contents.Substring(TagIndex, EndTagIndex - TagIndex);
            return Result;
        }

        public static string SetVariableValue(string Contents, string Name, string Value)
        {
            string Tag = Name + " = ";

            int TagIndex = Contents.IndexOf(Tag);
            if (TagIndex < 0)
            {
                Console.WriteLine("BuildVersion not found.");
                return "";
            }
            TagIndex = TagIndex + Tag.Length;

            int EndTagIndex = Contents.IndexOf(";", TagIndex);
            if (EndTagIndex < 0)
            {
                Console.WriteLine("BuildVersion end not found.");
                return "";
            }

            return Contents.Substring(0, TagIndex) + Value + Contents.Substring(EndTagIndex);
        }

        public static void Main(string[] Args)
        {
            if (Args.Length != 2)
            {
                Console.WriteLine("Usage: VersionUpdater.exe <Version.cs> <SolutionDirectory>");
            }
            else
            {
                string VersionCsFile = Args[0];
                string SolutionDirectory = Args[1];

                Console.WriteLine("Updating version in '{0}' and assemblies in directory '{1}'", VersionCsFile, SolutionDirectory);

                // Patch version number.
                string VersionContents = File.ReadAllText(VersionCsFile);
                int BuildVersion = int.Parse(GetVariableValue(VersionContents, "BuildVersion"));
                BuildVersion++;
                VersionContents = SetVariableValue(VersionContents, "BuildVersion", BuildVersion.ToString());

                int MajorVersion = int.Parse(GetVariableValue(VersionContents, "MajorVersion"));
                int MinorVersion = int.Parse(GetVariableValue(VersionContents, "MinorVersion"));
                int PatchVersion = int.Parse(GetVariableValue(VersionContents, "PatchVersion"));

                string VersionString = string.Format("{0}.{1}.{2}.{3}", MajorVersion, MinorVersion, PatchVersion, BuildVersion);

                File.WriteAllText(VersionCsFile, VersionContents);

                // Patch assembly files.
                string[] AssemblyFiles = Directory.GetFiles(SolutionDirectory, "AssemblyInfo.cs", SearchOption.AllDirectories);
                foreach (string FilePath in AssemblyFiles)
                {
                    string Contents = File.ReadAllText(FilePath);
                    Contents = PatchAssemblyTag(Contents, "AssemblyVersion", "\"" + VersionString + "\"");
                    Contents = PatchAssemblyTag(Contents, "AssemblyFileVersion", "\"" + VersionString + "\"");
                    File.WriteAllText(FilePath, Contents);
                }
            }
        }
    }
}
