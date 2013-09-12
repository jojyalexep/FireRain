using EP.BulkMessage.Service.Entity;
using EP.BulkMessage.Service.Entity.Enum;
using EP.BulkMessage.Service.ExternalServices;
using EP.BulkMessage.Service.Repositories;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Domain.CampaignModule
{
    public class CampaignService
    {
        MainUnitOfWork unitOfWork;
        public CampaignService()
        {
            unitOfWork = new MainUnitOfWork();
        }

        public int SaveCampaign(Campaign campaign)
        {
            unitOfWork.Begin();

            campaign.StatusId = (int)CampaignStatus.New;
            campaign.StatusDate = DateTime.Now;
            var campaignId = (int)unitOfWork.Session.Save(campaign);
            unitOfWork.Commit();
            return campaignId;
        }

       

        public bool UpdateCampaign(Campaign campaign)
        {
            unitOfWork.Begin();
            unitOfWork.Session.SaveOrUpdate(campaign);
            unitOfWork.Commit();
            return true;
        }

        public bool UpdateCampaignStatus(int campaignId, CampaignStatus status)
        {
            return UpdateCampaignStatus(campaignId, status, DateTime.Now);
        }

        public bool UpdateCampaignStatus(int campaignId, CampaignStatus status, DateTime date)
        {
            try
            {
                var campaign = GetCampaign(campaignId);
                campaign.StatusId = (int)status;
                campaign.StatusDate = date;
                UpdateCampaign(campaign);
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool ReCreateCampaign(Campaign campaign,int oldTypeId)
        {
            try
            {

                if (oldTypeId == (int)CampaignType.Email)
                {
                    DeleteCampaignEmails(campaign.Id);
                }
                else
                {
                    DeleteCampaignSmses(campaign.Id);
                }
                campaign.StatusId = (int)CampaignStatus.New;
                campaign.StatusDate = DateTime.Now;
                UpdateCampaign(campaign);
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool DeleteCampaign(int campaignId)
        {
            try
            {
                UpdateCampaignStatus(campaignId, CampaignStatus.Deleted);
                var campaign = GetCampaign(campaignId);
                if (campaign.TypeId == (int)CampaignType.Email)
                {
                    DeleteCampaignEmails(campaignId);
                }
                else
                {
                    DeleteCampaignSmses(campaignId);
                }
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool DeleteCampaignEmails(int campaignId)
        {
            unitOfWork.Begin();
            var emailList = GetCampaignEmails(campaignId, MessageStatus.All);
            foreach (var email in emailList)
            {
                email.StatusId = (int)MessageStatus.Deleted;
                email.StatusDate = DateTime.Now;
                unitOfWork.Session.SaveOrUpdate(email);
                //unitOfWork.Session.Delete(email);
            }
            unitOfWork.Commit();
            return true;

        }

        public bool DeleteCampaignSmses(int campaignId)
        {
            unitOfWork.Begin();
            var smsList = GetCampaignSmses(campaignId, MessageStatus.All);
            foreach (var sms in smsList)
            {
                sms.StatusId = (int)MessageStatus.Deleted;
                sms.StatusDate = DateTime.Now;
                unitOfWork.Session.SaveOrUpdate(sms);
                //unitOfWork.Session.Delete(sms);
            }
            unitOfWork.Commit();
            return true;

        }

        public void SaveCampaignEmails(List<Email> emailList)
        {
            BulkRepository.Current.InsertBulkEmail(emailList);
        }

        public bool UpdateCampaignEmails(IEnumerable<Email> emailList)
        {
            unitOfWork.Begin();
            foreach (var email in emailList)
            {
                unitOfWork.Session.Update(email);
            }
            unitOfWork.Commit();
            return true;

        }

        public bool UpdateCampaignSmses(IEnumerable<Sms> smsList)
        {
            unitOfWork.Begin();
            foreach (var sms in smsList)
            {
                unitOfWork.Session.Update(sms);
            }
            unitOfWork.Commit();
            return true;

        }

        public void SaveCampaignSmses(List<Sms> smsList)
        {
            DateTime startTime = DateTime.Now;

            BulkRepository.Current.InsertBulkSMS(smsList);

            var time = DateTime.Now.Subtract(startTime);
            int day = time.Days;
        }


        public IEnumerable<Campaign> GetAllCampaigns()
        {
            return unitOfWork.Session.QueryOver<Campaign>().Where(p => p.StatusId != (int)CampaignStatus.Deleted).List();
        }

        public IEnumerable<Campaign> GetAllCampaigns(int departmentId)
        {
            return unitOfWork.Session.QueryOver<Campaign>().Where(p => p.StatusId != (int)CampaignStatus.Deleted && p.DepartmentId == departmentId).List();
        }


        public IEnumerable<Campaign> GetApprovalCampaigns(int departmentId)
        {
            return unitOfWork.Session.QueryOver<Campaign>().Where(p => p.StatusId == (int)CampaignStatus.New && p.DepartmentId == departmentId).List();
        }

        public IEnumerable<Campaign> GetAllScheduledCampaigns()
        {
            return unitOfWork.Session.QueryOver<Campaign>()
                .Where(p => (p.StatusId == (int)CampaignStatus.Scheduled || p.StatusId == (int)CampaignStatus.Progress) && p.StatusDate < DateTime.Now).List();
        }

        public IEnumerable<Campaign> GetScheduledCampaignsByType(int campaignType)
        {
            return unitOfWork.Session.QueryOver<Campaign>()
                .Where(p => (p.StatusId == (int)CampaignStatus.Scheduled || p.StatusId == (int)CampaignStatus.Progress) && p.StatusDate < DateTime.Now && p.TypeId == campaignType).List();
        }


        public Campaign GetCampaign(int campaignId)
        {
            return unitOfWork.Session.Get<Campaign>(campaignId);
        }

        public IEnumerable<Email> GetCampaignEmails(int campaignId, MessageStatus status)
        {
            if (status == MessageStatus.All)
            {
                return unitOfWork.Session.QueryOver<Email>()
                            .Where(p => p.CampaignId == campaignId && p.StatusId != (int)MessageStatus.Deleted).List();
            }
            else
            {
                return unitOfWork.Session.QueryOver<Email>()
                            .Where(p => p.CampaignId == campaignId && p.StatusId == (int)status).List();
            }
        }

        public IEnumerable<Sms> GetCampaignSmses(int campaignId, MessageStatus status)
        {
            if (status == MessageStatus.All)
            {
                return unitOfWork.Session.QueryOver<Sms>()
                            .Where(p => p.CampaignId == campaignId && p.StatusId != (int)MessageStatus.Deleted).List();
            }
            else
            {
                return unitOfWork.Session.QueryOver<Sms>()
                            .Where(p => p.CampaignId == campaignId && p.StatusId == (int)status).List();
            }
        }


        public int GetCampaignSmsCount(int campaignId)
        {
            return unitOfWork.Session.QueryOver<Sms>().Where(p => p.CampaignId == campaignId)
                    .Select(Projections.RowCount())
                    .FutureValue<int>()
                    .Value;
        }

        public int GetCampaignEmailCount(int campaignId)
        {
            return unitOfWork.Session.QueryOver<Email>().Where(p => p.CampaignId == campaignId)
                    .Select(Projections.RowCount())
                    .FutureValue<int>()
                    .Value;
        }

        public IEnumerable<Schedule> GetSchedules()
        {
            return unitOfWork.Session.CreateCriteria<Schedule>().List<Schedule>();
        }
    }
}