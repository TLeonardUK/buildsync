﻿/*
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

using System.Windows.Forms;

namespace BuildSync.Client.Controls.Setup
{
    /// <summary>
    ///     Base control for all controls shown as pages in the <see cref="SetupForm" /> form.
    /// </summary>
    public partial class SetupPageControlBase : UserControl
    {
        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public virtual string Title => "Untitled";

        /// <summary>
        /// 
        /// </summary>
        public virtual bool PreviousEnabled => false;

        /// <summary>
        /// 
        /// </summary>
        public virtual bool NextEnabled => false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SetupPageControlBase" /> class.
        /// </summary>
        public SetupPageControlBase()
        {
            InitializeComponent();
        }
    }
}