using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Networking
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void MessageRecievedHandler(NetConnection Connection, NetMessage Message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void DisconnectHandler(NetConnection Connection);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void ConnectHandler(NetConnection Connection);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void ConnectFailedHandler(NetConnection Connection);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void HandshakeFailedHandler(NetConnection Connection, HandshakeResultType ErrorType);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void ClientConnectHandler(NetConnection Connection, NetConnection ClientConnection);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void ClientDisconnectHandler(NetConnection Connection, NetConnection ClientConnection);

    /// <summary>
    /// 
    /// </summary>
    public class NetConnection
    {
        private Socket Socket;
        private bool Connecting = false;
        private bool Listening = false;
        private byte[] MessageBuffer = new byte[NetMessage.HeaderSize];
        private List<NetConnection> Clients = new List<NetConnection>();

        private bool IsClient = false;

        private bool ShouldDisconnectDueToError = false;

        private ulong LastKeepAliveSendTime = 0;
        private const int KeepAliveInterval = 5 * 1000;

        private NetMessage_Handshake Handshake = null;
        private bool HandshakeFailed = false;

        private Queue<Action> EventQueue = new Queue<Action>();

        private ConcurrentQueue<byte[]> SendQueue = new ConcurrentQueue<byte[]>();
        private object SendQueueWakeObject = new object();
        private long SendQueueBytes = 0;
        private Thread SendThread;
        private bool IsSendingThreadRunning = false;

        public enum AsyncCallType
        { 
            Accept,
            Recieve,
            Send,
            Dns,
            Connect,
            Count
        }

        private int[] OutstandingAsyncCalls = new int[(int)AsyncCallType.Count];

        /// <summary>
        /// 
        /// </summary>
        public static BandwidthThrottler GlobalBandwidthThrottleIn = new BandwidthThrottler();

        /// <summary>
        /// 
        /// </summary>
        public static BandwidthThrottler GlobalBandwidthThrottleOut = new BandwidthThrottler();

        /// <summary>
        /// 
        /// </summary>
        public static BandwidthTracker GlobalBandwidthStats = new BandwidthTracker();

        /// <summary>
        /// 
        /// </summary>
        public BandwidthTracker BandwidthStats = new BandwidthTracker();

        /// <summary>
        /// 
        /// </summary>
        public object Metadata = null;

        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint Address;

        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint ListenAddress;

        /// <summary>
        /// 
        /// </summary>
        public event MessageRecievedHandler OnMessageRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event DisconnectHandler OnDisconnect;

        /// <summary>
        /// 
        /// </summary>
        public event ConnectHandler OnConnect;

        /// <summary>
        /// 
        /// </summary>
        public event ConnectFailedHandler OnConnectFailed;

        /// <summary>
        /// 
        /// </summary>
        public event HandshakeFailedHandler OnHandshakeResult;        

        /// <summary>
        /// 
        /// </summary>
        public event ClientConnectHandler OnClientConnect;

        /// <summary>
        /// 
        /// </summary>
        public event ClientDisconnectHandler OnClientDisconnect;

        /// <summary>
        /// 
        /// </summary>
        public event MessageRecievedHandler OnClientMessageRecieved;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get { return Socket?.Connected ?? false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadyForData
        {
            get { return IsConnected && Handshake != null && !HandshakeFailed; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnecting
        {
            get { return Connecting; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsListening
        {
            get { return Listening; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<NetConnection> AllClients
        {
            get
            {
                lock (Clients)
                {
                    return Clients;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SendQueueSize
        {
            get
            {
                return (int)SendQueueBytes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public NetConnection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public NetConnection(Socket InSocket)
        {
            Socket = InSocket;
            Address = (IPEndPoint)Socket.RemoteEndPoint;
            IsClient = true;
        }

        /// <summary>
        /// 
        /// </summary>
        ~NetConnection()
        {
            Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginSendThread()
        {
            IsSendingThreadRunning = true;

            if (SendThread != null)
            {
                EndSendThread();
            }

            SendThread = new Thread(SendThreadEntry);
            SendThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndSendThread()
        {
            if (SendThread != null)
            {
                lock (SendQueueWakeObject)
                {
                    IsSendingThreadRunning = false;
                    Monitor.Pulse(SendQueueWakeObject);
                }

                SendThread.Join();
            }

            while (SendQueue.Count > 0)
            {
                byte[] Data = null;
                SendQueue.TryDequeue(out Data);
            }

            SendThread = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            lock (Clients)
            {
                foreach (NetConnection Client in Clients.ToArray())
                {
                    Client.Disconnect();
                }
            }

            if (Socket != null)
            {
                //Socket.Disconnect(true);
                Socket.Close();
            }

            EndSendThread();

            // Block while any outstanding async calls are waiting to finish.
            for (int i = 0; i < OutstandingAsyncCalls.Length; i++)
            {
                while (OutstandingAsyncCalls[i] > 0)
                {
                    Thread.Sleep(1);
                }
            }

            Listening = false;
            Connecting = false;
            ShouldDisconnectDueToError = false;
            Handshake = null;

            lock (EventQueue)
            {
                EventQueue.Enqueue(() => { OnDisconnect?.Invoke(this); });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CallType"></param>
        private void RecordBeginAsyncCall(AsyncCallType CallType)
        {
            int NewValue = Interlocked.Increment(ref OutstandingAsyncCalls[(int)CallType]);
            Debug.Assert(NewValue < 255);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CallType"></param>
        private void RecordEndAsyncCall(AsyncCallType CallType)
        {
            int NewValue = Interlocked.Decrement(ref OutstandingAsyncCalls[(int)CallType]);
            Debug.Assert(NewValue >= 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private void QueueDisconnect()
        {
            ShouldDisconnectDueToError = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            if (ShouldDisconnectDueToError)
            {
                Disconnect();
            }

            lock (Clients)
            {
                foreach (NetConnection Client in Clients.ToArray())
                {
                    Client.Poll();
                }
            }

            if (IsConnected && !IsListening && Handshake != null)
            {
                ulong Elapsed = TimeUtils.Ticks - LastKeepAliveSendTime;
                if (Elapsed > KeepAliveInterval)
                {
                    Send(new NetMessage_KeepAlive());
                    LastKeepAliveSendTime = TimeUtils.Ticks;
                }
            }

            // Fire off all queued events.
            lock (EventQueue)
            {
                while (EventQueue.Count > 0)
                {
                    EventQueue.Dequeue().Invoke();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Port"></param>
        public void BeginListen(int Port, bool ReuseAddresses = true)
        {
            if (Socket != null)
            {
                Disconnect();
            }

            Address = new IPEndPoint(IPAddress.Any, Port);

            string Hostname = Dns.GetHostName();
            IPAddress[] HostAddresses = Dns.GetHostAddresses(Hostname);
            IPAddress Ipv4Address = IPAddress.Any;
            foreach (IPAddress Address in HostAddresses)
            {
                if (Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    Ipv4Address = Address;
                    break;
                }
            }

            ListenAddress = new IPEndPoint(Ipv4Address, Port);

            Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 512 * 1024);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 512 * 1024);
            Socket.NoDelay = true;

            if (ReuseAddresses)
            {
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            }

            Logger.Log(LogLevel.Info, LogCategory.Transport, "Listening on {0} (Connectable on {1})", Address.ToString(), ListenAddress.ToString());

            Listening = true;
            try
            {
                RecordBeginAsyncCall(AsyncCallType.Accept);

                Socket.Bind(Address);
                Socket.Listen(128);

                AsyncCallback AcceptLambda = null;
                AcceptLambda = (IAsyncResult Result) =>
                {
                    try
                    {
                        Socket ClientSocket = Socket.EndAccept(Result);

                        ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 512 * 1024);
                        ClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 512 * 1024);
                        ClientSocket.NoDelay = true;

                        Logger.Log(LogLevel.Info, LogCategory.Transport, "Client connected from {0}", ClientSocket.RemoteEndPoint.ToString());

                        NetConnection ClientConnection = new NetConnection(ClientSocket);

                        // Handle messages from this client.
                        MessageRecievedHandler MessageRecievedLambda = null;
                        MessageRecievedLambda = (NetConnection, Message) =>
                        {
                            lock (Clients)
                            {
                                lock (EventQueue)
                                {
                                    EventQueue.Enqueue(() => { OnClientMessageRecieved?.Invoke(ClientConnection, Message); });
                                }
                            }
                        };

                        // Handle disconnects for this client.
                        DisconnectHandler DisconnectLambda = null;
                        DisconnectLambda = (NetConnection) =>
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Transport, "Client disconnected.");

                            lock (Clients)
                            {
                                lock (EventQueue)
                                {
                                    EventQueue.Enqueue(() => { OnClientDisconnect?.Invoke(this, ClientConnection); });
                                }

                                Clients.Remove(ClientConnection);

                                ClientConnection.OnDisconnect -= DisconnectLambda;
                                ClientConnection.OnMessageRecieved -= MessageRecievedLambda;
                            }
                        };

                        ClientConnection.OnMessageRecieved += MessageRecievedLambda;
                        ClientConnection.OnDisconnect += DisconnectLambda;

                        // Add to client list.
                        lock (Clients)
                        {
                            Clients.Add(ClientConnection);

                            lock (EventQueue)
                            {
                                EventQueue.Enqueue(() => { OnClientConnect?.Invoke(this, ClientConnection); });
                            }
                        }

                        ClientConnection.BeginClient();

                        RecordBeginAsyncCall(AsyncCallType.Accept);
                        Socket.BeginAccept(AcceptLambda, this);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to accept connection {0} with error: {1}", Address.ToString(), ex.Message);
                        Listening = false;
                    }
                    finally
                    {
                        RecordEndAsyncCall(AsyncCallType.Accept);
                    }
                };

                Socket.BeginAccept(AcceptLambda, this);
            }
            catch (Exception ex)
            {
                RecordEndAsyncCall(AsyncCallType.Accept);
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to listen on {0} with error: {1}", Address.ToString(), ex.Message);
                Listening = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Port"></param>
        public void BeginConnect(string Hostname, int Port)
        {
            if (Socket != null)
            {
                Disconnect();
            }

            Connecting = true;

            try
            {
                RecordBeginAsyncCall(AsyncCallType.Dns);
                Dns.BeginGetHostEntry(Hostname, DnsResult =>
                {
                    bool BegunSocketConnect = false;

                    try
                    {
                        IPHostEntry HostEntry = Dns.EndGetHostEntry(DnsResult);

                        foreach (IPAddress address in HostEntry.AddressList)
                        {
                            if (address.AddressFamily == AddressFamily.InterNetwork ||
                                address.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                Address = new IPEndPoint(address, Port);
                            }
                        }

                        if (Address == null)
                        {
                            throw new Exception("Address is not valid '" + Hostname + "'.");
                        }

                        Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        Logger.Log(LogLevel.Info, LogCategory.Transport, "Connecting to {0} ({1})", Address.ToString(), HostEntry.HostName);

                        RecordBeginAsyncCall(AsyncCallType.Connect);
                        BegunSocketConnect = true;
                        Socket.BeginConnect(Address, Result =>
                        {
                            try
                            {
                                Socket.EndConnect(Result);

                                Logger.Log(LogLevel.Info, LogCategory.Transport, "Connected to {0}", Socket.RemoteEndPoint.ToString());

                                lock (EventQueue)
                                {
                                    EventQueue.Enqueue(() => { OnConnect?.Invoke(this); });
                                }

                                // Send handshake.
                                NetMessage_Handshake HandshakeMsg = new NetMessage_Handshake();
                                HandshakeMsg.Version = AppVersion.VersionNumber;
                                Send(HandshakeMsg);

                                BeginSendThread();
                                BeginRecievingHeader();
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to connect to {0} with error: {1}", Address.ToString(), ex.Message);

                                lock (EventQueue)
                                {
                                    EventQueue.Enqueue(() => { OnConnectFailed?.Invoke(this); });
                                }
                            }
                            finally
                            {
                                RecordEndAsyncCall(AsyncCallType.Connect);
                                Connecting = false;
                            }
                        },
                        this);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to connect to {0} with error: {1}", Hostname, ex.Message);
                        Connecting = false;

                        lock (EventQueue)
                        {
                            EventQueue.Enqueue(() => { OnConnectFailed?.Invoke(this); });
                        }

                        if (BegunSocketConnect)
                        {
                            RecordEndAsyncCall(AsyncCallType.Connect);
                        }
                    }
                    finally
                    {
                        RecordEndAsyncCall(AsyncCallType.Dns);
                    }

                }, null);
            }
            catch (Exception ex)
            {
                lock (EventQueue)
                {
                    EventQueue.Enqueue(() => { OnConnectFailed?.Invoke(this); });
                }

                RecordEndAsyncCall(AsyncCallType.Dns);
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to connect to {0} with error: {1}", Hostname, ex.Message);
                Connecting = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendThreadEntry()
        {
            while (IsSendingThreadRunning)
            {
                byte[] SendData;

                lock (SendQueueWakeObject)
                {
                    if (SendQueue.Count > 0)
                    {
                        if (!SendQueue.TryDequeue(out SendData))
                        {
                            continue;
                        }
                        else
                        {
                            Interlocked.Add(ref SendQueueBytes, -SendData.Length);
                        }
                    }
                    else 
                    {
                        Monitor.Wait(SendQueueWakeObject, 1000);
                        continue;
                    }
                }

                SendBlock(SendData, 0, SendData.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public void Send(NetMessage Message)
        {
            //Stopwatch totalstop = new Stopwatch();
            //totalstop.Start();

            byte[] Serialized = Message.ToByteArray();

            SendQueue.Enqueue(Serialized);
            Interlocked.Add(ref SendQueueBytes, Serialized.Length);

            lock (SendQueueWakeObject)
            {
                Monitor.Pulse(SendQueueWakeObject);
            }

            //totalstop.Stop();
            //Logger.Log(LogLevel.Info, LogCategory.Transport, "Elapsed ms to send: {0}", ((float)totalstop.ElapsedTicks / (Stopwatch.Frequency / 1000.0)));
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendBlock(byte[] Block, int Offset, int Length)
        {
            RecordBeginAsyncCall(AsyncCallType.Send);
            try
            {
                int TotalBytesSent = 0;
                while (TotalBytesSent < Length)
                {
                    int BytesLeft = Length - TotalBytesSent;
                    int BytesToSend = GlobalBandwidthThrottleOut.Throttle(BytesLeft);

                    int BytesSent = Socket.Send(Block, Offset + TotalBytesSent, BytesToSend, SocketFlags.None);
                    BandwidthStats.BytesOut(BytesSent);
                    GlobalBandwidthStats.BytesOut(BytesSent);

                    TotalBytesSent += BytesSent;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to begin sending to client {0} with error: {1}", Address.ToString(), ex.Message);
                QueueDisconnect();
            }
            finally
            {
                RecordEndAsyncCall(AsyncCallType.Send);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginClient()
        {
            // Send handshake to client.
            NetMessage_Handshake HandshakeMsg = new NetMessage_Handshake();
            HandshakeMsg.Version = AppVersion.VersionNumber;
            Send(HandshakeMsg);

            // Start recieving from client
            try
            {
                BeginSendThread();
                BeginRecievingHeader();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to begin reciving from client {0} with error: {1}", Address.ToString(), ex.Message);
                QueueDisconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginRecievingHeader(int Offset = 0, int Size = NetMessage.HeaderSize)
        {
            try
            {
                RecordBeginAsyncCall(AsyncCallType.Recieve);

                int BytesToRecv = GlobalBandwidthThrottleIn.Throttle(Size);

                Socket.BeginReceive(MessageBuffer, Offset, BytesToRecv, SocketFlags.None, Result =>
                {
                    bool ShouldDisconnect = false;
                    try
                    {
                        int BytesRecieved = Socket.EndReceive(Result);

                        BandwidthStats.BytesIn(BytesRecieved);
                        GlobalBandwidthStats.BytesIn(BytesRecieved);

                        if (BytesRecieved < Size)
                        {
                            BeginRecievingHeader(Offset + BytesRecieved, Size - BytesRecieved);
                            return;
                        }

                        BeginRecievingPayload();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve header from {0}, with error: {1}", Address.ToString(), ex.Message);
                        ShouldDisconnect = true;
                    }
                    finally
                    {
                        RecordEndAsyncCall(AsyncCallType.Recieve);

                        if (ShouldDisconnect)
                        {
                            QueueDisconnect();
                        }
                    }
                }, this);
            }
            catch (Exception ex)
            {
                RecordEndAsyncCall(AsyncCallType.Recieve);

                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve header from {0}, with error: {1}", Address.ToString(), ex.Message);
                QueueDisconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginRecievingPayload()
        {
            NetMessage MessageHeader = new NetMessage();
            MessageHeader.ReadHeader(MessageBuffer);

            if (MessageHeader.PayloadSize > NetMessage.MaxPayloadSize)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Recieved message with payload above max size {0}, disconnecting", MessageHeader.PayloadSize);
                QueueDisconnect();
                return;
            }

            // Make sure buffer has enough space.
            if (MessageBuffer.Length < NetMessage.HeaderSize + MessageHeader.PayloadSize)
            {
                Array.Resize(ref MessageBuffer, NetMessage.HeaderSize + MessageHeader.PayloadSize);
            }

            // If no payload, we have the full message.
            if (MessageHeader.PayloadSize == 0)
            {
                ProcessMessage(MessageBuffer, NetMessage.HeaderSize + MessageHeader.PayloadSize);

                BeginRecievingHeader();
            }
            else
            {
                BeginRecievingPayloadWithOffset(0, MessageHeader.PayloadSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        private void BeginRecievingPayloadWithOffset(int Offset, int Size)
        {
            try
            {
                RecordBeginAsyncCall(AsyncCallType.Recieve);

                int BytesToRecv = GlobalBandwidthThrottleIn.Throttle(Size);

                Socket.BeginReceive(MessageBuffer, NetMessage.HeaderSize + Offset, BytesToRecv, SocketFlags.None, Result =>
                {
                    try
                    {
                        int BytesRecieved = Socket.EndReceive(Result);

                        BandwidthStats.BytesIn(BytesRecieved);
                        GlobalBandwidthStats.BytesIn(BytesRecieved);

                        if (BytesRecieved < Size)
                        {
                            BeginRecievingPayloadWithOffset(Offset + BytesRecieved, Size - BytesRecieved);
                            return;
                        }

                        ProcessMessage(MessageBuffer, NetMessage.HeaderSize + Offset + Size);

                        BeginRecievingHeader();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve payload from {0}, with error: {1}", Address.ToString(), ex.Message);
                        QueueDisconnect();
                    }
                    finally
                    {
                        RecordEndAsyncCall(AsyncCallType.Recieve);
                    }
                }, this);
            }
            catch (Exception ex)
            {
                RecordEndAsyncCall(AsyncCallType.Recieve);
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve payload from {0}, with error: {1}", Address.ToString(), ex.Message);
                QueueDisconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessMessage(byte[] Buffer, int Size = 0)
        {
            //Stopwatch stop = new Stopwatch();
            //stop.Start();

            NetMessage Message = NetMessage.FromByteArray(Buffer);
            if (Message != null)
            {
                //Logger.Log(LogLevel.Info, LogCategory.Transport, "Recieved message of type {0} from {1}", Message.GetType().Name, Address.ToString());

                if (Message is NetMessage_HandshakeResult)
                {
                    NetMessage_HandshakeResult HandshakeResult = Message as NetMessage_HandshakeResult;
                    Logger.Log(LogLevel.Error, LogCategory.Transport, "Client returned handshake result of type '{0}'.", HandshakeResult.ResultType.ToString());

                    lock (EventQueue)
                    {
                        EventQueue.Enqueue(() => { OnHandshakeResult?.Invoke(this, HandshakeResult.ResultType); });
                    }

                    // Client is responsible for the disconnect to ensure it gets message before the disconnect occurs.
                    if (IsClient && HandshakeResult.ResultType != HandshakeResultType.Success)
                    {
                        QueueDisconnect();
                    }
                }
                else if (Message is NetMessage_Handshake)
                {
                    Handshake = Message as NetMessage_Handshake;

                    NetMessage_HandshakeResult Response = new NetMessage_HandshakeResult();
                    Response.ResultType = HandshakeResultType.Success;

#if SHIPPING
                    if (Handshake.Version != AppVersion.VersionNumber)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Client has incompatible protocol version, rejecting.", Address.ToString());
                        Response.ResultType = HandshakeResultType.InvalidVersion;
                        HandshakeFailed = true;
                    }
#endif

                    Send(Response);
                }
                else if (Handshake != null && !HandshakeFailed)
                {
                    lock (EventQueue)
                    {
                        EventQueue.Enqueue(() => { OnMessageRecieved?.Invoke(this, Message); });
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, LogCategory.Transport, "Recieved message before recieving handshake, disconnecting.", Address.ToString());
                    QueueDisconnect();
                }
            }
            else
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to decode message, disconnecting.", Address.ToString());
                QueueDisconnect();
            }

            //stop.Stop();
            //Logger.Log(LogLevel.Info, LogCategory.Transport, "Elapsed ms to process message: {0} size was {1}kb type {2}", ((float)stop.ElapsedTicks / (Stopwatch.Frequency/1000.0)), Size/1024, Message.GetType().Name);
        }
    }
}
