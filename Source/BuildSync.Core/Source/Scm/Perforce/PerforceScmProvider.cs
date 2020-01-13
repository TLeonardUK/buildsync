using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Perforce.P4;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Scm.Perforce
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Repo"></param>
    public delegate void PerforceCommandHandler(Repository Repo);

    /// <summary>
    /// 
    /// </summary>
    public class PerforceScmProvider : IScmProvider
    {
        /// <summary>
        /// 
        /// </summary>
        private BlockingCollection<PerforceCommandHandler> CommandQueue = new BlockingCollection<PerforceCommandHandler>();

        /// <summary>
        /// 
        /// </summary>
        private Server ServerInstance = null;

        /// <summary>
        /// 
        /// </summary>
        private Repository Repository = null;

        /// <summary>
        /// 
        /// </summary>
        public string Server { get; private set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Username { get; private set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; private set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Root { get; private set; } = "";

        /// <summary>
        /// 
        /// </summary>
        private Thread CommandThread = null;

        /// <summary>
        /// 
        /// </summary>
        private ulong LastRequestedSyncTime = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int SYNC_TIME_RECHECK_INTERVAL = 2 * 60 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private DateTime SyncTime = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        private bool SyncTimeRequesting = false;

        /// <summary>
        /// 
        /// </summary>
        private string ClientName = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InServerName"></param>
        /// <param name="InUsername"></param>
        /// <param name="InPassword"></param>
        public PerforceScmProvider(string InServerName, string InUsername, string InPassword, string InRoot)
        {
            Server = InServerName;
            Username = InUsername;
            Password = InPassword;
            Root = FileUtils.NormalizePath(InRoot);

            CommandThread = new Thread(CommandThreadEntry);
            CommandThread.Name = "Perforce Commands";
            CommandThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Callback"></param>
        public void QueueAndAwaitCommand(PerforceCommandHandler Callback)
        {
            ManualResetEvent Event = new ManualResetEvent(false);

            QueueCommand((Repository Repo) =>
            {
                try
                {
                    Callback(Repo);
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Scm, "Encountered exception while running perforce command: {0}", Ex.Message);
                }
                finally
                {
                    Event.Set();
                }
            });

            Event.WaitOne();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Callback"></param>
        public void QueueCommand(PerforceCommandHandler Callback)
        {
            CommandQueue.Add(Callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateClientName()
        {
            Logger.Log(LogLevel.Info, LogCategory.Scm, "Requesting perforce clients.");

            Options options = new Options();
            options["-u"] = Username;

            IList<Client> Clients = Repository.GetClients(options);
            foreach (Client client in Clients)
            {
                if (FileUtils.NormalizePath(client.Root) == Root)
                {
                    Repository.Connection.Client = client;
                    ClientName = client.Name;

                    Logger.Log(LogLevel.Info, LogCategory.Scm, "Found perforce client={0} for location={1}", client.Name, Root);
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CommandThreadEntry()
        {
            while (!CommandQueue.IsCompleted)
            {
                // Server name valid?
                if (Server == string.Empty)
                {
                    Thread.Sleep(1 * 1000);
                    continue;
                }

                // Connect to repro.
                if (Repository == null || !Repository.Connection.connectionEstablished())
                {
                    try
                    {
                        ServerInstance = new Server(new ServerAddress(Server));
                        Repository = new Repository(ServerInstance);
                        Repository.Connection.UserName = Username;
                        
                        Options options = new Options();
                        options["Password"] = Password;

                        Logger.Log(LogLevel.Info, LogCategory.Scm, "Connecting to perforce: {0}", Server);
                        Repository.Connection.Connect(options);

                        if (Repository.Connection.connectionEstablished())
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Scm, "Connected to perforce server.");
                            UpdateClientName();
                        }
                    }
                    catch (P4Exception Ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Scm, "Failed to connect to perforce server with error: {0}", Ex.Message);
                        Thread.Sleep(5 * 1000);
                        continue;
                    }
                }

                // Execute next command.
                PerforceCommandHandler Handler = CommandQueue.Take();
                try
                {
                    Handler(Repository);
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Scm, "Encountered exception while running perforce command: {0}", Ex.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime GetSyncTime()
        {
            if (ClientName == string.Empty)
            {
                return DateTime.MinValue;
            }

            if (SyncTimeRequesting == false)
            {
                SyncTimeRequesting = true;

                ulong Current = TimeUtils.Ticks;
                ulong Elapsed = Current - LastRequestedSyncTime;
                if (Elapsed > SYNC_TIME_RECHECK_INTERVAL)
                {
                    QueueAndAwaitCommand((Repository Repo) =>
                    {
                        try
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Scm, "Requesting sync time of perforce workspace: {0}", ClientName);

                            ChangesCmdOptions Options = new ChangesCmdOptions(ChangesCmdFlags.IncludeTime, null, 1, ChangeListStatus.Submitted, null, 0);

                            FileSpec FilePath = new FileSpec(new DepotPath("//..."), VersionSpec.Have);
                            IList<Changelist> Changes = Repo.GetChangelists(Options, FilePath);
                            if (Changes.Count > 0)
                            {
                                SyncTime = Changes[0].ModifiedDate;
                                Logger.Log(LogLevel.Info, LogCategory.Scm, "Workspace '{0}' last have changeset has timestamp {1}", ClientName, SyncTime.ToString());
                            }
                        }
                        finally
                        {
                            SyncTimeRequesting = false;
                        }
                    });
                }

                LastRequestedSyncTime = Current;
            }

            return SyncTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Terminate()
        {
            CommandQueue.CompleteAdding();
            CommandThread.Join();
            CommandThread = null;

            if (Repository != null)
            {
                Repository.Connection.Disconnect(null);
            }

            Repository = null;
            ServerInstance = null;
        }
    }
}
