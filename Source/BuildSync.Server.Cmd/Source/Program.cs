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
using BuildSync.Core.Utils;

namespace BuildSync.Cmd
{
    /// <summary>
    ///     Application entry class.
    /// 
    ///     The BuildSync.Server.Cmd application is used purely as a stub application that
    ///     opens a interprocess communication pipe to the locally running build-sync server
    ///     and passes any command line arguments to it in the form of a command to execute.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Entry point.
        /// 
        ///     Usage syntax: BuildSync.Server.Cmd.exe [Arguments to pass to server]
        /// </summary>
        /// <param name="Args">Command line arguments provided.</param>
        public static void Main(string[] Args)
        {
            try
            {
                CommandIPC Ipc = new CommandIPC("buildsync-server", true);

                RecievePartialIPCResponseEventHandler ResponseHandler = Response => { Console.Write(Response); };

                string Result = "";
                if (!Ipc.Send("RunCommand", Args, out Result, ResponseHandler))
                {
                    Console.WriteLine("FAILURE: Failed to execute ipc command on server.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("FAILURE: Could not connect to ipc server, make sure buildsync server is running.");
            }
        }
    }
}