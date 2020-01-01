using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Response"></param>
    public delegate void RecievePartialIPCResponseEventHandler(string Response);

    /// <summary>
    /// 
    /// </summary>
    public class CommandIPCWriter : TextWriter
    {
        private CommandIPC Ipc;

        public CommandIPCWriter(CommandIPC InIpc)
        {
            Ipc = InIpc;
        }

        public override void Write(char value)
        {
            Ipc.Respond(new string(value, 1));
        }

        public override void Write(string value)
        {
            Ipc.Respond(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CommandIPC
    {
        private string PipeName = "";
        private bool IsClient = false;

        private NamedPipeServerStream PipeServer = null;
        private BinaryWriter ServerWriter = null;
        private BinaryReader ServerReader = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public CommandIPC(string InName, bool InIsClient)
        {
            PipeName = InName;
            IsClient = InIsClient;

            if (!IsClient)
            {
                try
                {
                    PipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    ServerWriter = new BinaryWriter(PipeServer);
                    ServerReader = new BinaryReader(PipeServer);

                    BeginAccept();
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to create pipe server with error: {0}", Ex.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginAccept()
        {
            PipeServer.BeginWaitForConnection((IAsyncResult Result) =>
            {
                try
                {
                    PipeServer.EndWaitForConnection(Result);
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to wait for pipe connection with error: {0}", Ex.Message);
                }

                if (!PipeServer.IsConnected)
                {
                    BeginAccept();
                }

            }, null);
        }

        /// <summary>
        /// 
        /// </summary>
        ~CommandIPC()
        {
            if (PipeServer != null)
            {
                ServerWriter.Close();
                ServerReader.Close();

                ServerWriter = null;
                ServerReader = null;

                if (PipeServer.IsConnected)
                {
                    PipeServer.WaitForPipeDrain();
                    PipeServer.Disconnect();
                }
                PipeServer.Close();
                PipeServer = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public bool Send(string Command, string[] Args, out string Result, RecievePartialIPCResponseEventHandler StreamHandler = null)
        {
            Result = "";

            try
            {
                using (NamedPipeClientStream PipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    PipeClient.Connect(3000);

                    using (BinaryWriter Writer = new BinaryWriter(PipeClient))
                    {
                        using (BinaryReader Reader = new BinaryReader(PipeClient))
                        {
                            // Send command.
                            //Console.WriteLine("Sending: " + Command);
                            Writer.Write(Command);
                            Writer.Write(Args.Length);
                            foreach (string arg in Args)
                            {
                                Writer.Write(arg);
                            }
                            Writer.Flush();

                            // Read response.
                            while (true)
                            {
                                bool Final = Reader.ReadBoolean();
                                if (!Final)
                                {
                                    string Block = Reader.ReadString();
                                    StreamHandler?.Invoke(Block);

                                    Result += Block;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            //Console.WriteLine("Response: " + Result);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to send ipc command with error: {0}", Ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public bool Recieve(out string Command, out string[] Args)
        {
            Command = "";
            Args = null;

            if (PipeServer == null)
            {
                return false;
            }

            try
            {
                if (PipeServer.IsConnected)
                {
                    Command = ServerReader.ReadString();
                    int ArgCount = ServerReader.ReadInt32();

                    Args = new string[ArgCount];
                    for (int i = 0; i < ArgCount; i++)
                    {
                        Args[i] = ServerReader.ReadString();
                    }

                    return true;
                }
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve ipc command with error: {0}", Ex.Message);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public void Respond(string Result)
        {
            try
            {
                ServerWriter.Write(false);
                ServerWriter.Write(Result + Environment.NewLine);
                ServerWriter.Flush();
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to respond to ipc command with error: {0}", Ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndResponse()
        {
            try
            {
                ServerWriter.Write(true);
                ServerWriter.Flush();

                PipeServer.WaitForPipeDrain();
                PipeServer.Disconnect();
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to respond to ipc command with error: {0}", Ex.Message);
            }
            finally
            {
                BeginAccept();
            }
        }
    }
}
