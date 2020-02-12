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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProcessUtils
    {
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        /// 
        /// </summary>
        public static void AttachToParentConsole()
        {
           // AttachConsole(ATTACH_PARENT_PROCESS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Launch"></param>
        public static void SetLaunchOnStartup(string AppName, bool Launch)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (Launch)
            {
                rk.SetValue(AppName, Application.ExecutablePath);
            }
            else
            {
                rk.DeleteValue(AppName, false);
            }
        }
    }
}
