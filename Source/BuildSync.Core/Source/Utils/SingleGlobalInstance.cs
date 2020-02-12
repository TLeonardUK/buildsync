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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;

namespace BuildSync.Core.Utils
{
    /// <summary>
    ///     Allows a single instance of the application to run.
    ///     Based on https://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c/229567
    /// </summary>
    public class SingleGlobalInstance : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public bool HasHandle = false;

        /// <summary>
        /// 
        /// </summary>
        Mutex MainMutex;

        /// <summary>
        /// 
        /// </summary>
        private void InitMutex()
        {
            string appGuid = ((GuidAttribute)Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value;
            string mutexId = string.Format("Global\\{{{0}}}", appGuid);
            MainMutex = new Mutex(false, mutexId);

            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            MainMutex.SetAccessControl(securitySettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Timeout"></param>
        public SingleGlobalInstance(int Timeout)
        {
            InitMutex();
            try
            {
                if (Timeout < 0)
                {
                    HasHandle = MainMutex.WaitOne(System.Threading.Timeout.Infinite, false);
                }
                else
                {
                    HasHandle = MainMutex.WaitOne(Timeout, false);
                }

                if (HasHandle == false)
                {
                    throw new TimeoutException("Timeout waiting for exclusive access on SingleInstance");
                }
            }
            catch (AbandonedMutexException)
            {
                HasHandle = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (MainMutex != null)
            {
                if (HasHandle)
                {
                    MainMutex.ReleaseMutex();
                }
                MainMutex.Close();
            }
        }
    }
}
