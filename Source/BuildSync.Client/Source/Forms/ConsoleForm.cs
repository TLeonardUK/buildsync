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
using System.Windows.Forms;
using BuildSync.Core.Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class ConsoleForm : DockContent
    {
        /// <summary>
        /// </summary>
        public class TextBoxLogSink : LogSink
        {
            private readonly ConsoleForm OutputForm;

            public TextBoxLogSink(ConsoleForm Form)
            {
                OutputForm = Form;
            }

            public override void Close()
            {
                // Nothing to do
            }

            public override void Log(LogLevel Level, LogCategory Category, string Message, string RawMessage)
            {
                lock (OutputForm.BufferLock)
                {
                    OutputForm.Buffer += Message + Environment.NewLine;
                }
            }
        }

        /// <summary>
        /// </summary>
        internal string Buffer = "";

        /// <summary>
        /// </summary>
        internal object BufferLock = new object();

        /// <summary>
        /// </summary>
        private TextBoxLogSink Sink;

        /// <summary>
        /// </summary>
        public ConsoleForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            UpdateTimer.Enabled = true;
            Sink = new TextBoxLogSink(this);
            Logger.RegisterSink(Sink);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartClosing(object sender, FormClosingEventArgs e)
        {
            UpdateTimer.Enabled = false;
            Logger.UnregisterSink(Sink);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object sender, EventArgs e)
        {
            lock (BufferLock)
            {
                logTextBox.AppendText(Buffer);
                Buffer = "";
            }
        }
    }
}