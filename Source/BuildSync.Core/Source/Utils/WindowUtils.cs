using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class WindowUtils
    {
        const int SW_RESTORE = 9;

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AppWindowName"></param>
        /// <returns></returns>
        public static bool BringOtherAppInstanceToFront(string AppWindowName)
        {
            // This is a shitty way to do this. We need to find a way to find the window in a better
            // way. The main reason we do this is because if a form is set to ShowInTaskbar=false we 
            // can't get it the nicer way (Process.MainWindowHandle).
            IntPtr handle = FindWindow(null, AppWindowName);
            if (handle == IntPtr.Zero)
            {
                return false;
            }

            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            Console.WriteLine("Bring app to front.");
            SetForegroundWindow(handle);
            return true;


/*            Process CurrentProc = Process.GetCurrentProcess();
            Process[] Procs = Process.GetProcessesByName(CurrentProc.ProcessName);
            foreach (Process Proc in Procs)
            {
                if (Proc.Id != CurrentProc.Id)
                {
                    BringProcessToFront(Proc);
                    return true;
                }
            }
            return false;
*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        public static void BringProcessToFront(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalIPAddress()
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }
            return IPAddress.Any;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBar"></param>
        /// <param name="state"></param>
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
