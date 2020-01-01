using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Server;
using BuildSync.Core.Utils;

namespace BuildSync.Cmd
{
    public class Program
    {
        public static void Main(string[] Args)
        {
            try
            {
                CommandIPC Ipc = new CommandIPC("buildsync-server", true);

                RecievePartialIPCResponseEventHandler ResponseHandler = (string Response) =>
                {
                    Console.Write(Response);
                };

                string Result = "";
                if (!Ipc.Send("RunCommand", Args, out Result, ResponseHandler))
                {
                    Console.WriteLine("FAILURE: Failed to execute ipc command on server.");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("FAILURE: Could not connect to ipc server, make sure buildsync server is running.");
            }
        }
    }
}
