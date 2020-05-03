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

namespace BuildSync.Core.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SizeTextBox : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        private long InternalValue = -1;

        /// <summary>
        /// 
        /// </summary>
        private bool InternalDisplayAsTransferRate = false;

        /// <summary>
        /// 
        /// </summary>
        private bool ValueBeingSetInternally = false;

        /// <summary>
        /// 
        /// </summary>
        private bool PostfixChangingInternally = false;

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        public event EventHandler OnValueChanged;

        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Display as value?")]
        [Category("Size Text Box")]
        [Description("Value in bytes to display")]
        [Browsable(true)]
        public long Value
        {
            get
            {
                return InternalValue;
            }
            set
            {
                if (InternalValue == value)
                {
                    return;
                }

                InternalValue = value;
                OnValueChanged?.Invoke(this, null);

                if (!ValueBeingSetInternally)
                {
                    // Figure out best value.
                    long DisplayValue = InternalValue;
                    int PrefixIndex = 0;
                    while (DisplayValue > 0 && (DisplayValue % 1024) == 0)
                    {
                        PrefixIndex++;
                        DisplayValue /= 1024;
                    }

                    postfixComboBox.SelectedIndex = PrefixIndex;
                    valueTextBox.Value = DisplayValue;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Display as transfer rate?")]
        [Category("Size Text Box")]
        [Description("Display as a size or a transfer rate.")]
        [Browsable(true)]
        public bool DisplayAsTransferRate
        {
            get
            {
                return InternalDisplayAsTransferRate;
            }
            set
            {
                InternalDisplayAsTransferRate = value;

                postfixComboBox.Items.Clear();
                if (DisplayAsTransferRate)
                {
                    postfixComboBox.Items.Add("bytes/s");
                    postfixComboBox.Items.Add("KB/s");
                    postfixComboBox.Items.Add("MB/s");
                    postfixComboBox.Items.Add("GB/s");
                    postfixComboBox.Items.Add("TB/s");
                    postfixComboBox.Items.Add("PB/s");
                }
                else
                {
                    postfixComboBox.Items.Add("bytes");
                    postfixComboBox.Items.Add("KB");
                    postfixComboBox.Items.Add("MB");
                    postfixComboBox.Items.Add("GB");
                    postfixComboBox.Items.Add("TB");
                    postfixComboBox.Items.Add("PB");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SizeTextBox()
        {
            InitializeComponent();

            valueTextBox.Minimum = 0;
            valueTextBox.Maximum = long.MaxValue;

            DisplayAsTransferRate = false;
            Value = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostfixChanged(object sender, EventArgs e)
        {
            long DisplayValue = Value;
            for (int i = 0; i < postfixComboBox.SelectedIndex; i++)
            {
                DisplayValue /= 1024;
            }

            PostfixChangingInternally = true;
            valueTextBox.Value = DisplayValue;
            PostfixChangingInternally = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValueChanged(object sender, EventArgs e)
        {
            if (PostfixChangingInternally)
            {
                return;
            }

            long RealValue = (long)valueTextBox.Value;
            for (int i = 0; i < postfixComboBox.SelectedIndex; i++)
            {
                RealValue *= 1024;
            }

            ValueBeingSetInternally = true;
            Value = RealValue;
            ValueBeingSetInternally = false;
        }
    }
}
