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
using System.Threading.Tasks;

namespace BuildSync.Core.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="State"></param>
    /// <param name="Progress"></param>
    public delegate void ScriptBuildProgressDelegate(string State, float Progress);

    /// <summary>
    ///     Represents a the properties of a downloaded build that is exposed to build scripts.
    /// </summary>
    public class ScriptBuild
    {
        /// <summary>
        ///     Local directory this build is downloaded in.
        /// </summary>
        public string Directory = "";

        /// <summary>
        ///     User defined device name or ip that build should be installed to.
        /// </summary>
        public string InstallDevice = "";

        /// <summary>
        ///     User defined location or workspace name that build should be installed to.
        /// </summary>
        public string InstallLocation = "";

        /// <summary>
        ///     Delegate to call to provide the progress of a specific process happening to this build.
        /// </summary>
        public ScriptBuildProgressDelegate ProgressCallback;

        /// <summary>
        ///     Error message describing while action failed.
        /// </summary>
        public string ErrorMessage = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <param name="Progress"></param>
        public void ReportProgress(string State, float Progress)
        {
            ProgressCallback?.Invoke(State, Progress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public void ReportError(string Message)
        {
            ErrorMessage = Message;
        }
    }
}
