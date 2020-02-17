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
    ///     Base class for launch mode implemented in build scripts.
    /// </summary>
    public abstract class ScriptLaunchMode
    {
        /// <summary>
        ///     Name of this launch mode as displayed in the UI.
        /// </summary>
        public string Name = "";

        /// <summary>
        ///     If this mode is available to the user, can be used to hide the mode on systems that do not
        ///     have the prerequisites installed.
        /// </summary>
        public bool IsAvailable = false;

        /// <summary>
        ///     Invoked when this mode should install a build to an appropriate location.
        /// </summary>
        /// <param name="Build">Build that has been downloaded locally.</param>
        public abstract bool Install(ScriptBuild Build);

        /// <summary>
        ///     Invoked when this mode should begin running a build.
        /// </summary>
        /// <param name="Build">Build that has been downloaded locally.</param>
        public abstract bool Launch(ScriptBuild Build);
    }
}
