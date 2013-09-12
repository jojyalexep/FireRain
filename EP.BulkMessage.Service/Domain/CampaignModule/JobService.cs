using EP.BulkMessage.Service.Entity;
using EP.BulkMessage.Service.Entity.Enum;
using EP.BulkMessage.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Domain.CampaignModule
{
    public class JobService
    {
        public int AddJob(int emailBatchSize, int smsBatchSize)
        {
            var unitOfWork = new MainUnitOfWork();
            unitOfWork.Begin();
            Job job = new Job
            {
                Name = "JOB" + DateTime.Now.Ticks,
                StartDate = DateTime.Now,
                EmailBatch = emailBatchSize,
                SmsBatch = smsBatchSize,
                StatusId = (int)JobStatus.JobStarted
            };
            var id =  (int)unitOfWork.Session.Save(job);
            unitOfWork.Commit();
            return id;

        }

        public bool FinishJob(int jobId, int emailCount, int smsCount)
        {
            var unitOfWork = new MainUnitOfWork();
            unitOfWork.Begin();
            Job job = unitOfWork.Session.Get<Job>(jobId);
            job.EndDate = DateTime.Now;
            job.EmailSent = emailCount;
            job.SmsSent = smsCount;
            job.StatusId = (int)JobStatus.JobCompleted;
            unitOfWork.Session.Update(job);
            unitOfWork.Commit();
            return true;

        }

        public bool IsLastJobCompleted()
        {
            var unitOfWork = new MainUnitOfWork();
            unitOfWork.Begin();
            var jobs = unitOfWork.Session.QueryOver<Job>().OrderBy(p => p.Id).Desc.List();
            if (jobs == null || jobs.Count == 0)
            {
                return true;
            }
            else if (jobs.First().StatusId == (int)JobStatus.JobCompleted)
            {
                return true;
            }
            return false;
        }
    }
}