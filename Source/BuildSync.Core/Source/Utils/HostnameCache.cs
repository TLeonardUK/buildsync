using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class HostnameCache
    {
        private static Dictionary<string, string> Resolved = new Dictionary<string, string>();

        public static string GetHostname(string Addr)
        {
            lock (Resolved)
            {
                if (Resolved.ContainsKey(Addr))
                {
                    return Resolved[Addr];
                }

                Resolved.Add(Addr, Addr);
            }

            try
            {
                Logger.Log(LogLevel.Info, LogCategory.Transport, "Resolving hostname '{0}' ...", Addr);
                Dns.BeginGetHostEntry(Addr, DnsResult =>
                {                        
                    try
                    {
                        IPHostEntry HostEntry = Dns.EndGetHostEntry(DnsResult);
                        lock (Resolved)
                        {
                            Resolved[Addr] = HostEntry.HostName;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to resolved hostname '{0}' with error: {1}", Addr, ex.Message);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Transport, "Failed to resolved hostname '{0}' with error: {1}", Addr, ex.Message);
            }

            return Addr;
        }
    }
}
