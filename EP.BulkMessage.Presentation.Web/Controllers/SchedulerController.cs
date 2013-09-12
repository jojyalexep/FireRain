using EP.BulkMessage.Presentation.Web.Helpers;
using EP.BulkMessage.Service.Domain.CampaignModule;
using EP.BulkMessage.Service.Entity;
using EP.BulkMessage.Service.Entity.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Timers;
using System.Web.Http;

namespace EP.BulkMessage.Presentation.Web.Controllers
{
    [HandleErrorWithELMAH]
    public class SchedulerController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        public string RunSheduledCampaigns(int emailBatchSize = 50, int smsBatchSize = 50)
        {
            ScheduleService scheduleService = new ScheduleService();
            scheduleService.RunScheduledCampaigns(emailBatchSize, smsBatchSize);
            return "SUCCESS";
            //Timer timer = 
        }

        [AcceptVerbs("GET", "POST")]
        public string RunSheduledEmailCampaigns(int size = 50)
        {
            ScheduleService scheduleService = new ScheduleService();
            scheduleService.RunScheduledCampaignsByType((int)CampaignType.Email, size);
            return "SUCCESS";
        }

        [AcceptVerbs("GET", "POST")]
        public string RunSheduledSmsCampaigns(int size = 50)
        {
            ScheduleService scheduleService = new ScheduleService();
            scheduleService.RunScheduledCampaignsByType((int)CampaignType.SMS, size);
            return "SUCCESS";
        }

        [AcceptVerbs("GET", "POST")]
        public IEnumerable<Schedule> GetSchedules()
        {
            //ErrorMethod();
            ScheduleService scheduleService = new ScheduleService();
            return scheduleService.GetSchedules();
        }

        

        [AcceptVerbs("GET", "POST")]
        public bool CanStartJob()
        {
            JobService jobService = new JobService();
            bool hasJobCompleted =  jobService.IsLastJobCompleted();

            ScheduleService scheduleService = new ScheduleService();
            bool hasScheduledCampaigns  = scheduleService.HasScheduledCampaigns();

            if (hasJobCompleted && hasScheduledCampaigns)
            {
                return true;
            }
            return false;

        }

        private static void ErrorMethod()
        {
            for (int c = 0; c < 5; c++)
            {

                try
                {
                    int i = 0;
                    var count = 50 / i;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new ArgumentException("My custom Exception " + c));
                }
            }
        }
    }
}
