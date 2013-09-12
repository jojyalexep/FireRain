using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EP.BulkMessage.Scheduler
{
    public static class TimerService
    {
        public static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Logger.Write("Timer Elapsed");
                var timer = (Timer)sender;
                var scheduleManager = new ScheduleManager();
                var schedule = scheduleManager.GetSchedule();
                if (schedule != null)
                {

                    Scheduler.Start(schedule);
                    if (schedule.EndTime < DateTime.Now)
                        schedule = scheduleManager.GetSchedule();
                    timer.Interval = DateTime.Now.AddMinutes(schedule.Frequency).RoundUp().TotalMilliseconds;
                    Logger.Write("Next Call in :" + DateTime.Now.AddMinutes(schedule.Frequency).RoundUp());
                }
                else
                {
                    var nextSchedule = scheduleManager.GetNextSchedule();
                    timer.Interval = (nextSchedule.StartTime.RoundUp()).TotalMilliseconds;
                    Logger.Write("No schedules till :" + nextSchedule.StartTime);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        public static void Run()
        {
            try
            {
                Logger.Write("FireRain Scheduler started");

                var interval = 10; // minutes
                Logger.Write("Timer called in " + DateTime.Now.RoundUp(interval).ToString());
                Timer timer = new Timer(DateTime.Now.RoundUp(interval).TotalMilliseconds);
                timer.Elapsed += timer_Elapsed;
                timer.AutoReset = true;
                timer.Start();
            }
            catch (Exception ex)
            {
                Logger.Write("Exception :" + ex.Message);
            }
           
        }


        private static int GetInterval(int minutes)
        {
            return minutes * 1000 * 60;
        }

       
        
    }
}


static class DateTimeHelper
{
    public static TimeSpan RoundUp(this DateTime dateTime, int minute = 10)
    {
        var roundUpTime = new DateTime(dateTime.Year, dateTime.Month,
             dateTime.Day, dateTime.Hour, ((dateTime.Minute / minute) * minute), 0).AddMinutes(minute);

        return roundUpTime.Subtract(DateTime.Now);
    }
}
