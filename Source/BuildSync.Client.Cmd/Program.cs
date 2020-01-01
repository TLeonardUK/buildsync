using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Client;
using BuildSync.Core.Utils;

namespace BuildSync.Cmd
{
    public class Program
    {
        public static void Main(string[] Args)
        {
            try
            {
                CommandIPC Ipc = new CommandIPC("buildsync-client", true);

                RecievePartialIPCResponseEventHandler ResponseHandler = (string Response) =>
                {
                    Console.Write(Response);
                };

                string Result = "";
                if (!Ipc.Send("RunCommand", Args, out Result, ResponseHandler))
                {
                    Console.WriteLine("FAILURE: Failed to execute ipc command on client.");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("FAILURE: Could not connect to ipc client, make sure buildsync client is running.");
            }
        }
    }
}
