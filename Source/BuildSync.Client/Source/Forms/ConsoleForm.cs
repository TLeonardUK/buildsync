using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConsoleForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public class TextBoxLogSink : LogSink
        {
            private ConsoleForm OutputForm;
            
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
        /// 
        /// </summary>
        private TextBoxLogSink Sink = null;

        /// <summary>
        /// 
        /// </summary>
        internal object BufferLock = new object();

        /// <summary>
        /// 
        /// </summary>
        internal string Buffer = "";

        /// <summary>
        /// 
        /// </summary>
        public ConsoleForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Closing(object sender, FormClosingEventArgs e)
        {
            UpdateTimer.Enabled = false;
            Logger.UnregisterSink(Sink);
        }

        /// <summary>
        /// 
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
        /// 
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
