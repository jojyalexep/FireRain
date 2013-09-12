using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace EP.BulkMessage.Scheduler
{
    

    public class ScheduleManager
    {
        private static object _lock = new object();
        private static string Cache_Schedules_String = "Cache_Schedules";


        public Schedule GetSchedule()
        {
            return GetAllSchedules().FirstOrDefault(p => p.StartTime < DateTime.Now && p.EndTime > DateTime.Now);
        }

        public Schedule GetNextSchedule()
        {
            var schedule =  GetAllSchedules().OrderBy(p=>p.StartTime).FirstOrDefault(p=> p.EndTime > DateTime.Now);
            if (schedule == null)
            {
                schedule = GetAllSchedules().OrderBy(p => p.StartTime).FirstOrDefault();
                schedule.StartTime = schedule.StartTime.AddDays(1);
                schedule.EndTime = schedule.EndTime.AddDays(1);
            }
            return schedule;
        }

        public List<Schedule> GetAllSchedules()
        {
            List<Schedule> schedules;
            ObjectCache cache = MemoryCache.Default;
            lock (_lock)
            {
                schedules = (List<Schedule>)cache[Cache_Schedules_String];
                if (schedules == null)
                {
                    CacheItemPolicy policy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(1, 0, 0, 0) };
                    schedules = GetSchedulesFromDB();
                    if (schedules != null & schedules.Count > 0)
                        cache.Set(Cache_Schedules_String, schedules, policy);
                }
            }
            return schedules;
        }

        private static List<Schedule> GetSchedulesFromDB()
        {
            List<Schedule> schedules = new List<Schedule>();
            HttpResponseMessage response = MakeWebApiCall("api/Scheduler/GetSchedules");
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    schedules = response.Content.ReadAsAsync<List<Schedule>>().Result;
                }
                catch (Exception ex)
                {
                    Logger.Write("Exception :" + ex.Message);
                }
                
            }
            else
            {
                Logger.Write("Exception :" + String.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
            }
            return schedules;
        }

        public bool CanStartJob()
        {
            HttpResponseMessage response = MakeWebApiCall("api/Scheduler/CanStartJob");
            if (response != null && response.IsSuccessStatusCode)
            {
                try
                {
                    // Parse the response body. Blocking!
                    return response.Content.ReadAsAsync<bool>().Result;
                    
                }
                catch (Exception ex)
                {
                    Logger.Write("Exception :" + ex.Message);
                }
            }
            else
            {
                Logger.Write("Exception :" + String.Format("{0} ({1})", "Failed", "Negative Response"));
            }
            return false;
        }
        public bool RunSheduledCampaigns(int emailBatchSize, int smsBatchSize)
        {
            Parallel.Invoke(
                () => ScheduledCampaign("RunSheduledEmailCampaigns", emailBatchSize), 
                () => ScheduledCampaign("RunSheduledSmsCampaigns", smsBatchSize));
            
            return true;
        }

        private static void ScheduledCampaign(string method, int size)
        {
            HttpResponseMessage response = MakeWebApiCall("api/Scheduler/" + method + "?size=" + size);
            if (response != null && response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                Logger.Write("Info :" + String.Format("{0} ({1})", response.Content.ReadAsAsync<string>().Result, response.ReasonPhrase));
            }
            else
            {
                Logger.Write("Exception :" + String.Format("{0} ({1})", "Failed", "Negative Response: " + response.ReasonPhrase));
            }
        }

        private static HttpResponseMessage MakeWebApiCall(string clientAddress)
        {
            try
            {
                Logger.Write("Calling webservice : " + clientAddress);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(ConfigManager.GetConfiguration("api-baseaddress"));

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(clientAddress).Result;  // Blocking call!
                return response;
            }
            catch (Exception ex)
            {
                Logger.Write("Exception :" + ex.Message);
                return null;
            }
            
        }
    }
}
