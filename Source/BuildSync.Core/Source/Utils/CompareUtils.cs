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
using System.Collections;
using System.Runtime.InteropServices;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    public class FileSizeStringComparer : IComparer
    {
        public int Compare(object a, object b)
        {
            string sa = a as string;
            string sb = b as string;
            if (sa != null && sb != null)
            {
                long XValue = 0;
                long YValue = 0;

                XValue = StringUtils.SizeFormatToBytes(sa);
                YValue = StringUtils.SizeFormatToBytes(sb);

                return XValue.CompareTo(YValue);
            }

            return Comparer.Default.Compare(a, b);
        }
    }

    /// <summary>
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    public class TransferRateStringComparer : IComparer
    {
        public int Compare(object a, object b)
        {
            string sa = a as string;
            string sb = b as string;
            if (sa != null && sb != null)
            {
                long XValue = 0;
                long YValue = 0;

                XValue = StringUtils.TransferRateFormatToBytes(sa);
                YValue = StringUtils.TransferRateFormatToBytes(sb);

                return XValue.CompareTo(YValue);
            }

            return Comparer.Default.Compare(a, b);
        }
    }
}