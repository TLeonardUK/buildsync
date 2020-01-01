using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class WindowUtils
    {
        const int SW_RESTORE = 9;

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

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

        public static void BringProcessToFront(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }
    }
}
