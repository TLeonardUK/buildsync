//#define FAKE_LATENCY
//#define TRACK_MESSAGE_BUFFER_ALLOCATION_SITES

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
    public class Statistic_BandwidthIn : Statistic
    {
        public Statistic_BandwidthIn()
        {
            Name = @"IO\Download Speed (MB/s)";
            MaxLabel = "128 MB/s";
            MaxValue = 128.0f;
            DefaultShown = true;
        }

        public override void Gather()
        {
            AddSample(NetConnection.GlobalBandwidthStats.RateIn / 1024.0f / 1024.0f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_BandwidthOut : Statistic
    {
        public Statistic_BandwidthOut()
        {
            Name = @"IO\Upload Speed (MB/s)";
            MaxLabel = "128 MB/s";
            MaxValue = 128.0f;
            DefaultShown = true;
        }

        public override void Gather()
        {
            AddSample(NetConnection.GlobalBandwidthStats.RateOut / 1024.0f / 1024.0f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_TotalIn : Statistic
    {
        public Statistic_TotalIn()
        {
            Name = @"IO\Total Downloaded (MB)";
            MaxLabel = "128 MB";
            MaxValue = 128 * 1024 * 1024;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsSize = true;
        }

        public override void Gather()
        {
            AddSample(NetConnection.GlobalBandwidthStats.TotalIn);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_TotalOut : Statistic
    {
        public Statistic_TotalOut()
        {
            Name = @"IO\Total Uploaded (MB)";
            MaxLabel = "128 MB";
            MaxValue = 128 * 1024 * 1024;
            DefaultShown = false;

            Series.YAxis.AutoAdjustMax = true;
            Series.YAxis.FormatMaxLabelAsSize = true;
        }

        public override void Gather()
        {
            AddSample(NetConnection.GlobalBandwidthStats.TotalOut);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_AvailableRecieveMessageBuffers : Statistic
    {
        public Statistic_AvailableRecieveMessageBuffers()
        {
            Name = @"Packets\Available Recieve Message Buffers";
            MaxLabel = NetConnection.MaxRecieveMessageBuffers.ToString();
            MaxValue = NetConnection.MaxRecieveMessageBuffers;
            DefaultShown = false;
        }

        public override void Gather()
        {
            AddSample(NetConnection.FreeRecieveMessageBuffers.Count);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_AvailableSendMessageBuffers : Statistic
    {
        public Statistic_AvailableSendMessageBuffers()
        {
            Name = @"Packets\Available Send Message Buffers";
            MaxLabel = NetConnection.MaxSendMessageBuffers.ToString();
            MaxValue = NetConnection.MaxSendMessageBuffers;
            DefaultShown = false;
        }

        public override void Gather()
        {
            AddSample(NetConnection.FreeSendMessageBuffers.Count);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_AvailableGenericMessageBuffers : Statistic
    {
        public Statistic_AvailableGenericMessageBuffers()
        {
            Name = @"Packets\Available Generic Message Buffers";
            MaxLabel = NetConnection.MaxGenericMessageBuffers.ToString();
            MaxValue = NetConnection.MaxGenericMessageBuffers;
            DefaultShown = false;
        }

        public override void Gather()
        {
            AddSample(NetConnection.FreeGenericMessageBuffers.Count);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_PacketsIn : Statistic
    {
        public Statistic_PacketsIn()
        {
            Name = @"Packets\Packets In";
            MaxLabel = "256";
            MaxValue = 256;
            DefaultShown = false;
        }

        public override void Gather()
        {
            AddSample(NetConnection.GlobalPacketStats.RateIn);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_PacketsOut : Statistic
    {
        public Statistic_PacketsOut()
        {
            Name = @"Packets\Packets Out";
            MaxLabel = "256";
            MaxValue = 256;
            DefaultShown = false;
        }

        public override void Gather()
        {
            AddSample(NetConnection.GlobalPacketStats.RateOut);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Statistic_RequestFailures : Statistic
    {
        public Statistic_RequestFailures()
        {
            Name = @"Packets\Process Failures (OOM)";
            MaxLabel = "100";
            MaxValue = 100;
            DefaultShown = false;

            Series.Outline = Drawing.PrimaryOutlineColors[4];
            Series.Fill = Drawing.PrimaryFillColors[4];
        }

        public override void Gather()
        {
            AddSample(NetConnection.ProcessFailuresDueToMemory);
            NetConnection.ProcessFailuresDueToMemory = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NetConnection
    {
        private Socket Socket;
        private bool Connecting = false;
        private bool Listening = false;
        private List<NetConnection> Clients = new List<NetConnection>();

        private bool IsClient = false;

        private bool ShouldDisconnectDueToError = false;
        private ulong DisconnectTimeout = 0;

        private ulong LastPingSendTime = 0;
        private const int PingInterval = 1 * 1000;

        public NetMessage_Handshake Handshake { get; internal set; }
        private bool HandshakeFailed = false;
        private bool HandshakeFinished = false;

        private Queue<Action> EventQueue = new Queue<Action>();

        public static int ProcessFailuresDueToMemory = 0;

        public struct MessageQueueEntry
        {
            public byte[] Data;
            public int BufferSize;

#if FAKE_LATENCY
            public ulong SendTimeFakeLatency;
#endif
        }

        private ConcurrentQueue<MessageQueueEntry> SendQueue = new ConcurrentQueue<MessageQueueEntry>();
        private object SendQueueWakeObject = new object();
        private long SendQueueBytes = 0;
        private Thread SendThread;
        private bool IsSendingThreadRunning = false;
        private bool IsSendingThreadTerminating = false;
        private object SendThreadQueueLock = new object();

        private BlockingCollection<MessageQueueEntry> ProcessBufferQueue = null;
        private Thread ProcessThread;

        internal static ConcurrentBag<byte[]> FreeRecieveMessageBuffers = new ConcurrentBag<byte[]>();
        internal static ConcurrentBag<byte[]> FreeSendMessageBuffers = new ConcurrentBag<byte[]>();
        internal static ConcurrentBag<byte[]> FreeGenericMessageBuffers = new ConcurrentBag<byte[]>();

        internal static int RecieveMessageBufferCount = 0;
        internal static int SendMessageBufferCount = 0;
        internal static int GenericMessageBufferCount = 0;

#if FAKE_LATENCY
        internal const int MaxRecieveMessageBuffers = 2;
        internal const int MaxSendMessageBuffers = 2;
        internal const int MaxGenericMessageBuffers = 256;
#else
        // Message buffer only exists for the duration of recieving/processing. If we assume we have about 10 peers being actively 
        // downloaded from, a couple each should surfice, one for reading and one for processing. This should only become an issue
        // if we have a processing backlog. And in that case I think something else is wrong as all the data should be quick to parse into
        // a seperate NetMessage structure.
        internal const int MaxRecieveMessageBuffers = 32;
        internal const int MaxSendMessageBuffers = 32;
        internal const int MaxGenericMessageBuffers = 64;
#endif

        private ulong MaxClientsExceededPurgeTime = 0;

#if FAKE_LATENCY
        private Random FakeLatencyRandom = new Random();
        private const int FakeLatencyBase = 1000;
        private const int FakeLatencyJitter = 50;
#endif

#if TRACK_MESSAGE_BUFFER_ALLOCATION_SITES
        private static Dictionary<string, int> MessageBufferAllocationSites = new Dictionary<string, int>();
        private static ulong MessageBufferAllocationSitePrintTimer = TimeUtils.Ticks;
#endif

        private NetMessage MessageHeaderTempStorage = new NetMessage();

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
        public static RateTracker GlobalBandwidthStats = new RateTracker();

        /// <summary>
        /// 
        /// </summary>
        public static RateTracker GlobalPacketStats = new RateTracker();

        /// <summary>
        /// 
        /// </summary>
        public RateTracker BandwidthStats = new RateTracker();

        /// <summary>
        /// 
        /// </summary>
        public RollingAverage Ping = new RollingAverage(10);

        /// <summary>
        /// 
        /// </summary>
        public ulong BestPing = 0;

        /// <summary>
        /// 
        /// </summary>
        public object Metadata = null;

        /// <summary>
        /// 
        /// </summary>
        public int MaxConnectedClients = int.MaxValue;

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
        public NetConnection ParentConnection = null;

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
            get { return IsConnected && Handshake != null && !HandshakeFailed && HandshakeFinished; }
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
        public NetConnection(Socket InSocket, NetConnection InParent)
        {
            Socket = InSocket;
            Address = (IPEndPoint)Socket.RemoteEndPoint;
            IsClient = true;
            ParentConnection = InParent;
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
            IsSendingThreadTerminating = false;

            if (SendThread != null || ProcessThread != null)
            {
                EndSendThread();
            }

            SendThread = new Thread(SendThreadEntry);
            SendThread.Name = "Net Send (" + Address.ToString() + ")";
            SendThread.Start();

            ProcessBufferQueue = new BlockingCollection<MessageQueueEntry>();

            ProcessThread = new Thread(ProcessThreadEntry);
            ProcessThread.Name = "Net Process (" + Address.ToString() + ")";
            ProcessThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessThreadEntry()
        {
            while (!ProcessBufferQueue.IsCompleted)
            {
                MessageQueueEntry Data;
                if (ProcessBufferQueue.TryTake(out Data, 1000))
                {
                    if (ProcessMessage(Data.Data, Data.BufferSize))
                    {
                        try
                        {
                            ProcessBufferQueue.Add(Data);
                        }
                        catch (InvalidOperationException)
                        {
                            // This can happen if another thread switches adding off.
                        }
                        continue;
                    }
                    ReleaseMessageBuffer(Data.Data, "ProcessThreadEntry");
                }
            }
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
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    //Socket.Disconnect(true);
                }
                catch (Exception Ex)
                {
                }
                finally
                {
                    Socket.Close();
                }
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
            HandshakeFinished = false;
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
        private void QueueDisconnect(ulong Timeout = 0)
        {
            if (!ShouldDisconnectDueToError)
            {
                ShouldDisconnectDueToError = true;
                DisconnectTimeout = TimeUtils.Ticks + Timeout;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            if (ShouldDisconnectDueToError && TimeUtils.Ticks > DisconnectTimeout)
            {
                Disconnect();
                ShouldDisconnectDueToError = false;
            }

            lock (Clients)
            {
                NetConnection[] ClientArray = Clients.ToArray();

                foreach (NetConnection Client in ClientArray)
                {
                    Client.Poll();
                }

                // Disconnect clients if we go over max allowed.
                if (ClientArray.Length <= MaxConnectedClients)
                {
                    MaxClientsExceededPurgeTime = TimeUtils.Ticks;
                }

                // Give a grace period incase one of the connections is in the middle of disconnecting or handshaking.
                /*if (MaxClientsExceededPurgeTime - TimeUtils.Ticks > 1000)
                {
                    if (ClientArray.Length > 0 && ClientArray.Length > MaxConnectedClients)
                    {
                        Logger.Log(LogLevel.Warning, LogCategory.Licensing, "Disconnecting user as at maximum connected clients.");
                        //ClientArray[0].Disconnect();
                    }
                }*/
            }

            if (!IsListening && IsReadyForData)
            {
                ulong Elapsed = TimeUtils.Ticks - LastPingSendTime;
                if (Elapsed > PingInterval)
                {
                    Send(new NetMessage_Ping());
                    LastPingSendTime = TimeUtils.Ticks;
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

            /*
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
            */

            ListenAddress = new IPEndPoint(WindowUtils.GetLocalIPAddress(), Port);

            Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.SendBufferSize = 128 * 1024;
            Socket.SendTimeout = 2000;
            Socket.ReceiveBufferSize = 128 * 1024;
            Socket.ReceiveTimeout = 2000;
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
                        ClientSocket.SendBufferSize = 128 * 1024;
                        ClientSocket.SendTimeout = 2000;
                        ClientSocket.ReceiveBufferSize = 128 * 1024;
                        ClientSocket.ReceiveTimeout = 2000;
                        ClientSocket.NoDelay = true;

                        Logger.Log(LogLevel.Info, LogCategory.Transport, "Client connected from {0}", ClientSocket.RemoteEndPoint.ToString());

                        NetConnection ClientConnection = new NetConnection(ClientSocket, this);

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
        /// <param name="EndPoint"></param>
        public void ConnectToIP(IPEndPoint EndPoint)
        {
            HandshakeFailed = false;
            HandshakeFinished = false;
            Handshake = null;

            try
            {
                Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                Logger.Log(LogLevel.Info, LogCategory.Transport, "Connecting to {0}", Address.ToString());

                RecordBeginAsyncCall(AsyncCallType.Connect);
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

                        BeginSendThread();
                        BeginRecievingHeader(AllocMessageBuffer("ConnectToIP.RecieveHeader"));

                        // Send handshake.
                        NetMessage_Handshake HandshakeMsg = new NetMessage_Handshake();
                        HandshakeMsg.Version = AppVersion.ProtocolVersion;
                        Send(HandshakeMsg);
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
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to connect to {0} with error: {1}", EndPoint.ToString(), ex.Message);
                Connecting = false;

                lock (EventQueue)
                {
                    EventQueue.Enqueue(() => { OnConnectFailed?.Invoke(this); });
                }

                RecordEndAsyncCall(AsyncCallType.Connect);
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
                // If its already an IP, no need to do a hostname lookup.
                IPAddress HostnameAddress;
                if (IPAddress.TryParse(Hostname, out HostnameAddress))
                {
                    Address = new IPEndPoint(HostnameAddress, Port);
                    ConnectToIP(Address);
                }
                else
                {
                    RecordBeginAsyncCall(AsyncCallType.Dns);
                    Dns.BeginGetHostEntry(Hostname, DnsResult =>
                    {
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

                            ConnectToIP(Address);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to connect to {0} with error: {1}", Hostname, ex.Message);
                            Connecting = false;

                            lock (EventQueue)
                            {
                                EventQueue.Enqueue(() => { OnConnectFailed?.Invoke(this); });
                            }
                        }
                        finally
                        {
                            RecordEndAsyncCall(AsyncCallType.Dns);
                        }

                    }, null);
                }
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
                SendThread = null;
            }

            if (ProcessThread != null)
            {
                ProcessBufferQueue.CompleteAdding();
                ProcessThread.Join();
            }

            IsSendingThreadTerminating = true;
            lock (SendThreadQueueLock)
            {
                while (SendQueue.Count > 0)
                {
                    MessageQueueEntry Data;
                    SendQueue.TryDequeue(out Data);
                    ReleaseMessageBuffer(Data.Data, "EndSendThread", true);
                }
            }

            ProcessBufferQueue = null;
            SendThread = null;
            ProcessThread = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendThreadEntry()
        {
            while (IsSendingThreadRunning)
            {
                MessageQueueEntry SendData;

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

                            Interlocked.Add(ref SendQueueBytes, -SendData.BufferSize);
                        }
                    }
                    else 
                    {
                        Monitor.Wait(SendQueueWakeObject, 1000);
                        continue;
                    }
                }

#if FAKE_LATENCY
                ulong CurrentTime = TimeUtils.Ticks;
                if (CurrentTime < SendData.SendTimeFakeLatency)
                {
                    ulong TimeLeft = SendData.SendTimeFakeLatency - CurrentTime;
                    Thread.Sleep((int)TimeLeft);
                }
#endif

                GlobalPacketStats.Out(1);

                SendBlock(SendData.Data, 0, SendData.BufferSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public void Send(NetMessage Message)
        {
            lock (SendThreadQueueLock)
            {
                if (IsSendingThreadTerminating)
                {
                    return;
                }

                byte[] Serialized = AllocMessageBuffer("Send", true);
                int BufferLength = Message.ToByteArray(ref Serialized);

                //Logger.Log(LogLevel.Info, LogCategory.Transport, "Sending message of type {0}", Message.GetType().Name);

                Message.Cleanup();

#if FAKE_LATENCY
                int Latency = FakeLatencyBase + FakeLatencyRandom.Next(0, FakeLatencyJitter);
                SendQueue.Enqueue(new MessageQueueEntry { Data = Serialized, BufferSize = BufferLength, SendTimeFakeLatency = TimeUtils.Ticks + (ulong)Latency });
#else
                SendQueue.Enqueue(new MessageQueueEntry { Data = Serialized, BufferSize = BufferLength });
#endif

                Interlocked.Add(ref SendQueueBytes, BufferLength);

                lock (SendQueueWakeObject)
                {
                    Monitor.Pulse(SendQueueWakeObject);
                }
            }
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
                    BandwidthStats.Out(BytesSent);
                    GlobalBandwidthStats.Out(BytesSent);

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
                ReleaseMessageBuffer(Block, "SendBlockFinally", true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginClient()
        {
            // Send handshake to client.
            NetMessage_Handshake HandshakeMsg = new NetMessage_Handshake();
            HandshakeMsg.Version = AppVersion.ProtocolVersion;
            Send(HandshakeMsg);

            // Start recieving from client
            try
            {
                BeginSendThread();
                BeginRecievingHeader(AllocMessageBuffer("BeginClient.RecieveHeader"));
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
        private void BeginRecievingHeader(byte[] MessageBuffer, int Offset = 0, int Size = NetMessage.HeaderSize)
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

                        GlobalPacketStats.In(1);

                        BandwidthStats.In(BytesRecieved);
                        GlobalBandwidthStats.In(BytesRecieved);

                        if (BytesRecieved < Size)
                        {
                            BeginRecievingHeader(MessageBuffer, Offset + BytesRecieved, Size - BytesRecieved); // TODO: This is fucked!?
                            return;
                        }

                        BeginRecievingPayload(MessageBuffer);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve header from {0}, with error: {1}", Address.ToString(), ex.Message);
                        ReleaseMessageBuffer(MessageBuffer, "BeginRecievingHeader.Exception");
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
                ReleaseMessageBuffer(MessageBuffer, "BeginRecievingHeader.OuterException");

                RecordEndAsyncCall(AsyncCallType.Recieve);

                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve header from {0}, with error: {1}", Address.ToString(), ex.Message);
                QueueDisconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginRecievingPayload(byte[] MessageBuffer)
        {
            MessageHeaderTempStorage.ReadHeader(MessageBuffer);

            int TotalSize = NetMessage.HeaderSize + MessageHeaderTempStorage.PayloadSize;

            if (TotalSize > NetMessage.HeaderSize + NetMessage.MaxPayloadSize)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Recieved message with payload above max size {0}, disconnecting", MessageHeaderTempStorage.PayloadSize);
                ReleaseMessageBuffer(MessageBuffer, "BeginRecievingpayload.TotalSize");
                QueueDisconnect();
                return;
            }

            if (MessageBuffer.Length < TotalSize)
            {
                Array.Resize(ref MessageBuffer, TotalSize);
            }

            // If no payload, we have the full message.
            if (MessageHeaderTempStorage.PayloadSize == 0)
            {
                ProcessBufferQueue.Add(new MessageQueueEntry { Data = MessageBuffer,  BufferSize = NetMessage.HeaderSize + MessageHeaderTempStorage.PayloadSize });
                BeginRecievingHeader(AllocMessageBuffer("BeginRecievingPayload.RecieveNextHeader"));
            }
            else
            {
                BeginRecievingPayloadWithOffset(MessageBuffer, 0, MessageHeaderTempStorage.PayloadSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        private void BeginRecievingPayloadWithOffset(byte[] MessageBuffer, int Offset, int Size)
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

                        BandwidthStats.In(BytesRecieved);
                        GlobalBandwidthStats.In(BytesRecieved);

                        if (BytesRecieved < Size)
                        {
                            BeginRecievingPayloadWithOffset(MessageBuffer, Offset + BytesRecieved, Size - BytesRecieved);
                            return;
                        }

                        ProcessBufferQueue.Add(new MessageQueueEntry { Data = MessageBuffer, BufferSize = NetMessage.HeaderSize + Offset + Size });

                        BeginRecievingHeader(AllocMessageBuffer("BeginRecievingPayloadWithOffset.RecieveNextHeader"));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve payload from {0}, with error: {1}", Address.ToString(), ex.Message);
                        ReleaseMessageBuffer(MessageBuffer, "BeginRecievingPayloadWithOffset.Exception");
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
                ReleaseMessageBuffer(MessageBuffer, "BeginRecievingPayloadWithOffset.OuterException");

                RecordEndAsyncCall(AsyncCallType.Recieve);
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to recieve payload from {0}, with error: {1}", Address.ToString(), ex.Message);
                QueueDisconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PreallocateBuffers(int RecieveCount, int SendCount, int GenericCount)
        {
            RecieveCount = Math.Min(RecieveCount, MaxRecieveMessageBuffers);
            for (int i = 0; i < RecieveCount; i++)
            {
                byte[] Serialized = new byte[(1 * 1024 * 1024) + 64]; // based on 1mb block being most frequent large message + 64 bytes of overhead
                Interlocked.Increment(ref RecieveMessageBufferCount);
                FreeRecieveMessageBuffers.Add(Serialized);
            }

            SendCount = Math.Min(SendCount, MaxSendMessageBuffers);
            for (int i = 0; i < SendCount; i++)
            {
                byte[] Serialized = new byte[(1 * 1024 * 1024) + 64]; // based on 1mb block being most frequent large message + 64 bytes of overhead
                Interlocked.Increment(ref SendMessageBufferCount);
                FreeSendMessageBuffers.Add(Serialized);
            }

            GenericCount = Math.Min(GenericCount, MaxGenericMessageBuffers);
            for (int i = 0; i < GenericCount; i++)
            {
                byte[] Serialized = new byte[(1 * 1024 * 1024) + 64]; // based on 1mb block being most frequent large message + 64 bytes of overhead
                Interlocked.Increment(ref GenericMessageBufferCount);
                FreeGenericMessageBuffers.Add(Serialized);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private byte[] AllocMessageBuffer(string Site, bool ForSend = false)
        {
            ConcurrentBag<byte[]> FallbackFreeQueue = ForSend ? FreeSendMessageBuffers : FreeRecieveMessageBuffers;

            byte[] Serialized = null;
            while (true)
            {
                if (FreeGenericMessageBuffers.TryTake(out Serialized))
                {
                    break;
                }

                // Try and steal from fallback free queue.
                lock (FallbackFreeQueue)
                {
                    if (FallbackFreeQueue.Count > 0)
                    {
                        if (FallbackFreeQueue.TryTake(out Serialized))
                        {
                            break;
                        }
                    }
                }

                // If we can allocate another buffer do that, else, just keep trying until a buffer is available.
                if (GenericMessageBufferCount < MaxGenericMessageBuffers)
                {
                    break;
                }

#if TRACK_MESSAGE_BUFFER_ALLOCATION_SITES
                TryPrintMessageBufferAllocationSites();
#endif

                Thread.Sleep(1);
            }

            if (Serialized == null)
            {
                Serialized = new byte[(1 * 1024 * 1024) + 64]; // based on 1mb block being most frequent large message + 64 bytes of overhead
                Interlocked.Increment(ref GenericMessageBufferCount);
                Logger.Log(LogLevel.Info, LogCategory.Transport, "Dynamically allocating new generic message buffer, now a total of {0} buffers ({1} in queue).", RecieveMessageBufferCount, FreeRecieveMessageBuffers.Count);
            }

#if TRACK_MESSAGE_BUFFER_ALLOCATION_SITES
            lock (MessageBufferAllocationSites)
            {
                if (!MessageBufferAllocationSites.ContainsKey(Site))
                {
                    MessageBufferAllocationSites.Add(Site, 0);
                }
                MessageBufferAllocationSites[Site] = MessageBufferAllocationSites[Site] + 1;

                TryPrintMessageBufferAllocationSites();
            }
#endif

            return Serialized;
        }

#if TRACK_MESSAGE_BUFFER_ALLOCATION_SITES
        /// <summary>
        /// 
        /// </summary>
        private void TryPrintMessageBufferAllocationSites()
        {
            lock (MessageBufferAllocationSites)
            {
                ulong Elapsed = TimeUtils.Ticks - MessageBufferAllocationSitePrintTimer;
                if (Elapsed >= 1000)
                {
                    Console.WriteLine("==== Buffer Allocations ====");
                    int Total = 0;
                    foreach (string key in MessageBufferAllocationSites.Keys)
                    {
                        Console.WriteLine("[{0}] {1}", MessageBufferAllocationSites[key], key);
                        Total += MessageBufferAllocationSites[key];
                    }
                    Console.WriteLine("Sum: {0}", Total);

                    MessageBufferAllocationSitePrintTimer = TimeUtils.Ticks;
                }
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
        private void ReleaseMessageBuffer(byte[] Buffer, string Site, bool ForSend = false)
        {
#if TRACK_MESSAGE_BUFFER_ALLOCATION_SITES
            lock (MessageBufferAllocationSites)
            {
                if (!MessageBufferAllocationSites.ContainsKey(Site))
                {
                    MessageBufferAllocationSites.Add(Site, 0);
                }
                MessageBufferAllocationSites[Site] = MessageBufferAllocationSites[Site] - 1;
            }
#endif

            if (ForSend)
            {
                lock (FreeSendMessageBuffers)
                {
                    // Always fill back up the specific buffer first, followed by generics.
                    if (FreeSendMessageBuffers.Count < MaxSendMessageBuffers)
                    {
                        FreeSendMessageBuffers.Add(Buffer);
                    }
                    else
                    {
                        FreeGenericMessageBuffers.Add(Buffer);
                    }
                }
            }
            else
            {
                lock (FreeRecieveMessageBuffers)
                {
                    // Always fill back up the specific buffer first, followed by generics.
                    if (FreeRecieveMessageBuffers.Count < MaxRecieveMessageBuffers)
                    {
                        FreeRecieveMessageBuffers.Add(Buffer);
                    }
                    else
                    {
                        FreeGenericMessageBuffers.Add(Buffer);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool ProcessMessage(byte[] Buffer, int Size = 0)
        {
            bool WasMemoryAvailable = false;
            NetMessage Message = NetMessage.FromByteArray(Buffer, out WasMemoryAvailable);

            // If no memory allocation was available for deserialize message then requeue.
            if (!WasMemoryAvailable)
            {
                Interlocked.Increment(ref ProcessFailuresDueToMemory);
                return true;
            }

            if (Message != null)
            {
                //Logger.Log(LogLevel.Info, LogCategory.Transport, "Recieved message of type {0} from {1}", Message.GetType().Name, Address.ToString());

                bool CleanupHandled = false;

                if (Message is NetMessage_HandshakeResult)
                {
                    NetMessage_HandshakeResult HandshakeResult = Message as NetMessage_HandshakeResult;
                    Logger.Log(LogLevel.Info, LogCategory.Transport, "Client returned handshake result of type '{0}'.", HandshakeResult.ResultType.ToString());

                    lock (EventQueue)
                    {
                        EventQueue.Enqueue(() => { OnHandshakeResult?.Invoke(this, HandshakeResult.ResultType); });
                    }

                    // Client is responsible for the disconnect to ensure it gets message before the disconnect occurs.
                    if (HandshakeResult.ResultType != HandshakeResultType.Success)
                    {
                        QueueDisconnect(200ul);
                    }

                    HandshakeFailed = (HandshakeResult.ResultType != HandshakeResultType.Success);
                    HandshakeFinished = true;
                }
                else if (Message is NetMessage_Ping)
                {
                    NetMessage_Ping Msg = Message as NetMessage_Ping;

                    NetMessage_Pong Response = new NetMessage_Pong();
                    Response.Timestamp = Msg.Timestamp;

                    Send(Response);
                }
                else if (Message is NetMessage_Pong)
                {
                    NetMessage_Pong Msg = Message as NetMessage_Pong;

                    ulong Now = TimeUtils.Ticks;
                    ulong Timestamp = Msg.Timestamp;
                    ulong Elapsed = Math.Max(5, Now - Timestamp);

                    if (Elapsed < BestPing || BestPing == 0)
                    {
                        BestPing = Elapsed;
                    }
                    Ping.Add(Elapsed);
                }
                else if (Message is NetMessage_Handshake)
                {
                    NetMessage_Handshake Msg = Message as NetMessage_Handshake;

                    NetMessage_HandshakeResult Response = new NetMessage_HandshakeResult();
                    Response.ResultType = HandshakeResultType.Success;

#if SHIPPING
                    if (Msg.Version != AppVersion.ProtocolVersion)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Client has incompatible protocol version {0}, rejecting.", Msg.Version, Address.ToString());
                        Response.ResultType = HandshakeResultType.InvalidVersion;
                        HandshakeFailed = true;
                    }
#endif

                    if (ParentConnection != null && ParentConnection.Clients.Count > ParentConnection.MaxConnectedClients)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Exceeded maximum seats, rejecting client connection..", Address.ToString());
                        Response.ResultType = HandshakeResultType.MaxSeatsExceeded;
                        HandshakeFailed = true;
                    }

                    if (ParentConnection != null)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Transport, "Current seats used {0} of {1}.", ParentConnection.Clients.Count, ParentConnection.MaxConnectedClients);
                    }

                    if (Response.ResultType != HandshakeResultType.Success)
                    {
                        QueueDisconnect(200ul);
                    }

                    Handshake = Message as NetMessage_Handshake;

                    Send(Response);
                }
                else if (Handshake != null && !HandshakeFailed)
                {
                    CleanupHandled = true;
                    lock (EventQueue)
                    {
                        EventQueue.Enqueue(() =>
                        {
                            OnMessageRecieved?.Invoke(this, Message);
                            if (!Message.DoesRecieverHandleCleanup)
                            {
                                Message.Cleanup();
                            }
                        });
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, LogCategory.Transport, "Recieved message (type: {0}) before recieving handshake, disconnecting.", Message.GetType().Name, Address.ToString());
                    QueueDisconnect();
                }

                if (!CleanupHandled)
                {
                    Message.Cleanup();
                }
            }
            else
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to decode message, disconnecting.", Address.ToString());
                QueueDisconnect();
            }

            return false;
        }
    }
}
