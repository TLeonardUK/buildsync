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

namespace VersionUpdater
{
    /// <summary>
    ///     Application entry class.
    /// 
    ///     The VersionUpdater application is just used to bump the version numbers if
    ///     various files each time a shipping build is compiled.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Entry point.
        /// 
        ///     Usage syntax: VersionUpdater.exe VersionCsFile SolutionDirectory
        ///
        ///     VersionCSFile is the C# file that contains the persistent version number that is bumped 
        ///     each run. At a minimum it should define the variables: BuildVersion, MajorVersion,
        ///     MinorVersion, PatchVersion.
        ///
        ///     SolutionDirectory is the directory of the solution, which will be recursively searched for files
        ///     with version numbers that need updating (such as AssemblyInfo.cs files).
        /// </summary>
        /// <param name="Args">Command line arguments provided.</param>
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

                // Patch installer data.
                string[] InstallFiles = Directory.GetFiles(SolutionDirectory, "InstallerMM.wxs", SearchOption.AllDirectories);
                foreach (string FilePath in InstallFiles)
                {
                    string Contents = File.ReadAllText(FilePath);
                    Contents = PatchFullTag(Contents, "Language=\"1033\" Version=\"", "\"", VersionString);
                    File.WriteAllText(FilePath, Contents);
                }

                InstallFiles = Directory.GetFiles(SolutionDirectory, "Product.wxs", SearchOption.AllDirectories);
                foreach (string FilePath in InstallFiles)
                {
                    string Contents = File.ReadAllText(FilePath);
                    Contents = PatchFullTag(Contents, "<Product Id=\"*\" Name=\"Build Sync\" Language=\"1033\" Version=\"", "\"", VersionString);
                    File.WriteAllText(FilePath, Contents);
                }
            }
        }

        /// <summary>
        ///     Finds the value of a variable defined in a snippet of C# code.
        ///     The format it finds is: VariableName = Value;
        /// </summary>
        /// <param name="Contents">C# source code to find variable value within.</param>
        /// <param name="Name">Name of variable to get value of.</param>
        /// <returns>Value of variable if found, else an empty string.</returns>
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

        /// <summary>
        ///     Finds and replaces the value of a variable defined in a snippet of C# code.
        ///     The format it finds is: VariableName = Value;
        /// </summary>
        /// <param name="Contents">C# source code to find and replace the variable value within.</param>
        /// <param name="Name">Name of variable to get value of.</param>
        /// <param name="Value">New value to replace variable value with.</param>
        /// <returns>Patched version of Contents, or if not found then Contents unmodified.</returns>
        public static string SetVariableValue(string Contents, string Name, string Value)
        {
            string Tag = Name + " = ";

            int TagIndex = Contents.IndexOf(Tag);
            if (TagIndex < 0)
            {
                Console.WriteLine("BuildVersion not found.");
                return Contents;
            }

            TagIndex = TagIndex + Tag.Length;

            int EndTagIndex = Contents.IndexOf(";", TagIndex);
            if (EndTagIndex < 0)
            {
                Console.WriteLine("BuildVersion end not found.");
                return Contents;
            }

            return Contents.Substring(0, TagIndex) + Value + Contents.Substring(EndTagIndex);
        }

        /// <summary>
        ///     Patches the value of a CSharp assembly tag contained with a snippet of C# code.
        ///     The format it finds is [assembly: TagName("TagValue")]
        /// </summary>
        /// <param name="Contents">C# source code to patch value within.</param>
        /// <param name="TagName">Name of assembly tag to patch.</param>
        /// <param name="NewValue">New value for assembly tag.</param>
        /// <returns>Patched version of Contents, or if not found then Contents unmodified.</returns>
        public static string PatchAssemblyTag(string Contents, string TagName, string NewValue)
        {
            string Tag = "[assembly: " + TagName + "(";

            int TagIndex = Contents.IndexOf(Tag);
            if (TagIndex < 0)
            {
                Console.WriteLine("BuildVersion not found.");
                return Contents;
            }

            TagIndex = TagIndex + Tag.Length;

            int EndTagIndex = Contents.IndexOf(")", TagIndex);
            if (EndTagIndex < 0)
            {
                Console.WriteLine("BuildVersion end not found.");
                return Contents;
            }

            return Contents.Substring(0, TagIndex) + NewValue + Contents.Substring(EndTagIndex);
        }

        /// <summary>
        ///     Patches a value surrounded by the given post/pre-fixes.
        ///     The format it finds is TagStart Value TagEnd.
        /// </summary>
        /// <param name="Contents">Source code to patch value within.</param>
        /// <param name="TagStart">Prefix that is directly before the value to patch.</param>
        /// <param name="TagEnd">Postfix that is directly after the value to patch.</param>
        /// <param name="NewValue">New value to patch between the post/pre-fixes.</param>
        /// <returns>Patched version of Contents, or if not found then Contents unmodified.</returns>
        public static string PatchFullTag(string Contents, string TagStart, string TagEnd, string NewValue)
        {
            int TagIndex = Contents.IndexOf(TagStart);
            if (TagIndex < 0)
            {
                Console.WriteLine("Tag not found.");
                return Contents;
            }

            TagIndex = TagIndex + TagStart.Length;

            int EndTagIndex = Contents.IndexOf(TagEnd, TagIndex);
            if (EndTagIndex < 0)
            {
                Console.WriteLine("Tag End end not found.");
                return Contents;
            }

            return Contents.Substring(0, TagIndex) + NewValue + Contents.Substring(EndTagIndex);
        }
    }
}