using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BuildSync.Server
{
    public partial class MainService : ServiceBase
    {
        private Timer PollTimer;

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif

            PollTimer = new Timer(1);
            PollTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                lock (PollTimer)
                {
                    Program.OnPoll();
                }
            };
            PollTimer.Start();

            Program.OnStart();
        }

        protected override void OnStop()
        {
            PollTimer.Stop();

            Program.OnStop();
        }
    }
}
