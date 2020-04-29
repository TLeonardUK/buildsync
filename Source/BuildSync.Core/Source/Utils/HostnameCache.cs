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
using System.Collections.Generic;
using System.Net;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// </summary>
    public static class HostnameCache
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<string, string> Resolved = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Addr"></param>
        /// <returns></returns>
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
                Logger.Log(LogLevel.Verbose, LogCategory.Transport, "Resolving hostname '{0}' ...", Addr);
                Dns.BeginGetHostEntry(
                    Addr, DnsResult =>
                    {
                        try
                        {
                            IPHostEntry HostEntry = Dns.EndGetHostEntry(DnsResult);
                            lock (Resolved)
                            {
                                Logger.Log(LogLevel.Verbose, LogCategory.Transport, "Resolved '{0}' to '{1}'", Addr, HostEntry.HostName);
                                Resolved[Addr] = HostEntry.HostName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogLevel.Verbose, LogCategory.Transport, "Failed to resolved hostname '{0}' with error: {1}", Addr, ex.Message);
                        }
                    }, null
                );
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Transport, "Failed to resolved hostname '{0}' with error: {1}", Addr, ex.Message);
            }

            return Addr;
        }
    }
}