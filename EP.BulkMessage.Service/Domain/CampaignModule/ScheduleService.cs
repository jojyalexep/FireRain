using EP.BulkMessage.Service.Domain.UserModule;
using EP.BulkMessage.Service.Entity;
using EP.BulkMessage.Service.Entity.Enum;
using EP.BulkMessage.Service.ExternalServices;
using EP.BulkMessage.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Domain.CampaignModule
{
    public class ScheduleService
    {
        CampaignService campaignService;
        int smsCount;
        int emailCount;
        int _emailBatchSize;
        int _smsBatchSize;
        public ScheduleService()
        {
            campaignService = new CampaignService();
        }

        public IEnumerable<Schedule> GetSchedules()
        {
            return campaignService.GetSchedules();
        }


        #region Scheduled Campaign
        public bool RunScheduledCampaigns(int emailBatchSize, int smsBatchSize)
        {


            //campaignService.UpdateCampaignEmails

            emailCount = 0;
            smsCount = 0;
            _emailBatchSize = emailBatchSize;
            _smsBatchSize = smsBatchSize;

            var campaigns = campaignService.GetAllScheduledCampaigns();

            if (campaigns != null && campaigns.Count() > 0)
            {
                var jobService = new JobService();
                int jobId = jobService.AddJob(emailBatchSize, smsBatchSize);


                foreach (var campaign in campaigns)
                {
                    SendCampaign(campaign);
                }
                jobService.FinishJob(jobId, smsCount, emailCount);
                return true;
            }
            return false;
        }

        public bool HasScheduledCampaigns()
        {
            var campaigns = campaignService.GetAllScheduledCampaigns();

            if (campaigns != null && campaigns.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public bool RunScheduledCampaignsByType(int campaignType, int size)
        {


            //campaignService.UpdateCampaignEmails

            emailCount = 0;
            smsCount = 0;
            _emailBatchSize = size;
            _smsBatchSize = size;

            var campaigns = campaignService.GetScheduledCampaignsByType(campaignType);

            if (campaigns != null && campaigns.Count() > 0)
            {
                var jobService = new JobService();
                int jobId = jobService.AddJob(size, size);


                foreach (var campaign in campaigns)
                {
                    SendCampaign(campaign);
                }
                jobService.FinishJob(jobId, emailCount, smsCount);
                return true;
            }
            return false;
        }


        private bool SendCampaign(int campaignId)
        {

            return SendCampaign(campaignService.GetCampaign(campaignId));
        }


        private bool SendCampaign(Campaign campaign)
        {
            var user = new UserService().GetUser(campaign.CreatedBy);
            if (campaign != null && user != null)
            {
                if (campaign.StatusId == (int)CampaignStatus.Scheduled)
                {
                    campaignService.UpdateCampaignStatus(campaign.Id, CampaignStatus.Progress);
                }
                if (campaign.TypeId == (int)CampaignType.Email)
                {
                    if (emailCount < _emailBatchSize)
                    {
                        var emailList = campaignService.GetCampaignEmails(campaign.Id, MessageStatus.New).Take(_emailBatchSize - emailCount).ToList();
                        SendEmails(user, emailList);
                    }
                }
                else
                {
                    if (smsCount < _smsBatchSize)
                    {
                        var smsList = campaignService.GetCampaignSmses(campaign.Id, MessageStatus.New).Take(_smsBatchSize - smsCount);
                        SendSmes(user, smsList);
                    }
                }
                UpdateCampaignPercentage(campaign);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool UpdateCampaignPercentage(Campaign campaign)
        {
            bool response = false;
            if (campaign.TypeId == (int)CampaignType.Email)
            {
                var emailList = campaignService.GetCampaignEmails(campaign.Id, MessageStatus.All);
                var sendEmails = emailList.Where(p => p.StatusId != (int)MessageStatus.New);
                if (emailList.Count() != 0 && sendEmails.Count() != 0)
                {
                    campaign.PercentComplete = sendEmails.Count() / emailList.Count() * 100;
                    if (campaign.PercentComplete == 100)
                        campaignService.UpdateCampaignStatus(campaign.Id, CampaignStatus.Completed);
                    campaignService.UpdateCampaign(campaign);
                    response = true;
                }
            }
            else
            {
                var smsList = campaignService.GetCampaignSmses(campaign.Id, MessageStatus.All);
                var sendSmses = smsList.Where(p => p.StatusId != (int)MessageStatus.New);
                if (smsList.Count() != 0 && sendSmses.Count() != 0)
                {
                    campaign.PercentComplete = sendSmses.Count() / smsList.Count() * 100;
                    if (campaign.PercentComplete == 100)
                        campaignService.UpdateCampaignStatus(campaign.Id, CampaignStatus.Completed);
                    campaignService.UpdateCampaign(campaign);
                    response = true;
                }
            }
            return response;
        }
        #endregion

        private void SendSmes(EPUser user, IEnumerable<Sms> smsList)
        {

            if (smsList.Count() > 0)
            {
                SmsModule smsModule = new SmsModule(user.EmailUsername, user.EmailPassword);
                foreach (var sms in smsList)
                {
                    bool smsSuccess = smsModule.SendSMS(sms.ToNumber, sms.Content);
                    sms.StatusId = smsSuccess ? (int)MessageStatus.Send : (int)MessageStatus.Failed;
                    sms.StatusDate = DateTime.Now;
                    smsCount += 1;
                }
                campaignService.UpdateCampaignSmses(smsList);
            }
        }

        private void SendEmails(EPUser user, IEnumerable<Email> emailList)
        {
            if (emailList.Count() > 0)
            {
                EmailModule emailModule = new EmailModule(user.EmailUsername, user.EmailPassword);
                foreach (var email in emailList)
                {
                    bool emailSuccess = emailModule.SendEmail(user.Name, email.ToAddress, "", "", email.Subject, email.Body);
                    email.StatusId = emailSuccess ? (int)MessageStatus.Send : (int)MessageStatus.Failed;
                    email.StatusDate = DateTime.Now;
                    emailCount += 1;
                }
                campaignService.UpdateCampaignEmails(emailList);


            }
        }

    }
}