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
