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

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConsoleForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public class ConsoleTextBoxRedirecter : TextWriter
        {
            private Control OutputControl;
            private TextWriter OutputBaseWriter;

            public ConsoleTextBoxRedirecter(Control control, TextWriter BaseWriter)
            {
                OutputControl = control;
                OutputBaseWriter = BaseWriter;
            }

            public override void Write(char value)
            {
                if (OutputControl.IsHandleCreated)
                {
                    OutputControl.Invoke((MethodInvoker)(() =>
                    {
                        OutputControl.Text += value;
                    }));
                }
                OutputBaseWriter.Write(value);
            }

            public override void Write(string value)
            {
                if (OutputControl.IsHandleCreated)
                {
                    OutputControl.Invoke((MethodInvoker)(() =>
                    {
                        OutputControl.Text += value;
                    }));
                }
                OutputBaseWriter.Write(value);
            }

            public override Encoding Encoding
            {
                get { return Encoding.Unicode; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private TextWriter OriginalConsoleOut = null;

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
            Hide();
            Console.SetOut(OriginalConsoleOut);
            e.Cancel = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            OriginalConsoleOut = Console.Out;
            Console.SetOut(new ConsoleTextBoxRedirecter(logTextBox, Console.Out));
        }
    }
}
