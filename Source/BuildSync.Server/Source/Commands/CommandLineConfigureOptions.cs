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

using System.Reflection;
using System.Threading;
using BuildSync.Core.Utils;
using BuildSync.Server.Tasks;
using CommandLine;

namespace BuildSync.Server.Commands
{
    /// <summary>
    ///     CLI command for modifying the value of a setting that configures the behaviour of this tool.
    /// </summary>
    [Verb("configure", HelpText = "Sets the value of a setting configuring the behaviour of this tool.")]
    public class CommandLineConfigureOptions
    {
        /// <summary>
        ///     Name of setting to modify.
        /// </summary>
        [Value(0, MetaName = "Name", Required = true, HelpText = "Name of setting to modify.")]
        public string Name { get; set; }

        /// <summary>
        ///     New value of setting to modify.
        /// </summary>
        [Value(1, MetaName = "Value", Required = true, HelpText = "New value of setting to modify.")]
        public string Value { get; set; }

        /// <summary>
        ///     Called when the CLI invokes this command.
        /// </summary>
        /// <param name="IpcClient">Interprocess communication pipe to the application that invoked this command.</param>
        internal void Run(CommandIPC IpcClient)
        {
            PropertyInfo Property = null;
            foreach (PropertyInfo Potential in Program.Settings.GetType().GetProperties())
            {
                if (Potential.Name.ToLower() == Name.ToLower())
                {
                    Property = Potential;
                    break;
                }
            }

            if (Property == null)
            {
                IpcClient.Respond(string.Format("FAILED: Setting '{0}' does not exist.", Name));
                return;
            }

            // Calcualte correct value to set property to.
            object ValueToSet = null;
            if (Property.PropertyType == typeof(string))
            {
                ValueToSet = Value;
            }
            else if (Property.PropertyType == typeof(int))
            {
                int Result = 0;
                if (!int.TryParse(Value, out Result))
                {
                    IpcClient.Respond(string.Format("FAILED: Value '{0}' is not a valid int.", Value));
                    return;
                }

                ValueToSet = Result;
            }
            else if (Property.PropertyType == typeof(float))
            {
                float Result = 0;
                if (!float.TryParse(Value, out Result))
                {
                    IpcClient.Respond(string.Format("FAILED: Value '{0}' is not a valid float.", Value));
                    return;
                }

                ValueToSet = Result;
            }
            else if (Property.PropertyType == typeof(bool))
            {
                bool Result = false;
                if (Value == "1")
                {
                    Result = true;
                }
                else if (Value == "0")
                {
                    Result = false;
                }
                else
                {
                    if (!bool.TryParse(Value, out Result))
                    {
                        IpcClient.Respond(string.Format("FAILED: Value '{0}' is not a valid bool.", Value));
                        return;
                    }
                }

                ValueToSet = Result;
            }
            else if (Property.PropertyType == typeof(bool))
            {
                bool Result = false;
                if (Value == "1")
                {
                    Result = true;
                }
                else if (Value == "0")
                {
                    Result = false;
                }
                else
                {
                    if (!bool.TryParse(Value, out Result))
                    {
                        IpcClient.Respond(string.Format("FAILED: Value '{0}' is not a valid bool.", Value));
                        return;
                    }
                }

                ValueToSet = Result;
            }
            else
            {
                IpcClient.Respond(string.Format("FAILED: Setting '{0}' cannot be set externally.", Name));
                return;
            }

            // Check if we don't need to modify the setting.
            if (Property.GetValue(Program.Settings).ToString() == ValueToSet.ToString())
            {
                IpcClient.Respond(string.Format("FAILED: Setting '{0}' already set to given value.", Name));
                return;
            }

            // Special handling for storage path changing, we need to run some code to clean things up.
            if (Name.ToLower() == "storagepath")
            {
                MoveStorageTask MoveTask = new MoveStorageTask();
                MoveTask.Start(Program.Settings.StoragePath, Value);

                MoveStorageState OldState = MoveStorageState.Unknown;
                string OldFile = "";
                while (MoveTask.State != MoveStorageState.Success)
                {
                    if (OldState != MoveTask.State || OldFile != MoveTask.CurrentFile)
                    {
                        switch (MoveTask.State)
                        {
                            case MoveStorageState.CopyingFiles:
                            {
                                if (MoveTask.CurrentFile != null && MoveTask.CurrentFile.Trim().Length > 0)
                                {
                                    IpcClient.Respond(string.Format("Copying: {0}", MoveTask.CurrentFile));
                                }

                                break;
                            }
                            case MoveStorageState.CleaningUpOldDirectory:
                            {
                                IpcClient.Respond("Cleaning up old directory.");
                                break;
                            }
                            case MoveStorageState.Success:
                            {
                                break;
                            }
                            case MoveStorageState.FailedDiskError:
                            case MoveStorageState.Failed:
                            default:
                            {
                                IpcClient.Respond("FAILED: Failed to change storage directory due to disk error.");
                                return;
                            }
                        }

                        OldFile = MoveTask.CurrentFile;
                        OldState = MoveTask.State;
                    }

                    Thread.Sleep(10);
                }
            }

            // Update the value.
            IpcClient.Respond(string.Format("Applying value '{1}' to '{0}'.", Name, Value));
            Property.SetValue(Program.Settings, ValueToSet);
            Program.SaveSettings();
            Program.ApplySettings();
            IpcClient.Respond(string.Format("SUCCESS: Setting '{0}' was set to '{1}'.", Name, Value));
        }
    }
}