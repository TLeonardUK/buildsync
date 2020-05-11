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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Downloads;
using BuildSync.Core.Manifests;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ManifestAvailabilityPanel : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        private ManifestBlockListState InternalBlockListState;

        /// <summary>
        /// 
        /// </summary>
        private bool FirstUpdate = true;

        /// <summary>
        /// 
        /// </summary>
        public ManifestBlockListState BlockListState
        {
            get
            {
                return InternalBlockListState;
            }
            set
            {
                InternalBlockListState = value;
                UpdateState();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string NameOverride = "";

        /// <summary>
        /// 
        /// </summary>
        public void UpdateState()
        {
            if (NameOverride != "")
            {
                NameLabel.Text = NameOverride;
            }
            else
            {
                BuildManifest Manifest = Program.NetClient.GetOrRequestManifestById(InternalBlockListState.Id, !FirstUpdate);
                if (Manifest != null)
                {
                    NameLabel.Text = Manifest.VirtualPath;
                }
                else
                {
                    NameLabel.Text = InternalBlockListState.Id.ToString();
                }
            }

            AvailabilityStatus.ApplyLocalStates = false;
            AvailabilityStatus.UseOuterBorder = false;
            AvailabilityStatus.ManifestBlockState = InternalBlockListState;

            FirstUpdate = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public ManifestAvailabilityPanel()
        {
            InitializeComponent();
        }
    }
}
