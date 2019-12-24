using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class TimeUtils
    {
        [DllImport("kernel32.dll")]
        public static extern UInt64 GetTickCount64();

        public static ulong Ticks
        {
            get
            {
                return GetTickCount64();
            }
        }
    }
}
