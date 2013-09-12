using EP.BulkMessage.Scheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FireRainScheduler
{
    public partial class FireRainService : ServiceBase
    {
        public FireRainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TimerService.Run();
        }

        protected override void OnStop()
        {
        }
    }
}
