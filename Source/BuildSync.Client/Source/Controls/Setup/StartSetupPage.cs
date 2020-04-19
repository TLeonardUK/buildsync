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
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using BuildSync.Client.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls.Setup
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm" /> to allow the user to configure all
    ///     server settings.
    /// </summary>
    public partial class StartSetupPage : SetupPageControlBase
    {
        private readonly bool SkipValidity;

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string Title => "Setup Start";

        /// <summary>
        /// 
        /// </summary>
        public override bool PreviousEnabled => false;

        /// <summary>
        /// 
        /// </summary>
        public override bool NextEnabled => true;


        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings" /> class.
        /// </summary>
        public StartSetupPage()
        {
            InitializeComponent();
        }
    }
}