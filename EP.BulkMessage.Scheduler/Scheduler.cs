using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace EP.BulkMessage.Scheduler
{
    public sealed class Scheduler
    {
        private static object syncRoot = new Object();

        private Scheduler() { }

        public static void Start(Schedule schedule)
        {

            lock (syncRoot)
            {
                var scheduleManager = new ScheduleManager();
                if (scheduleManager.CanStartJob())
                {
                    Logger.Write("Job to be started");
                    scheduleManager.RunSheduledCampaigns(schedule.EmailBatchSize, schedule.SmsBatchSize);
                }
                else
                {
                    Logger.Write("No scheduled jobs pending / Last job not completed. Job stopped");
                }
            }

        }
    }
}
