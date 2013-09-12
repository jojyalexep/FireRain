using EP.BulkMessage.Presentation.Web.ViewModel;
using EP.BulkMessage.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Presentation.Web.DataAdapter
{
    public class CampaignAdapter
    {


        public EP.BulkMessage.Service.Entity.Campaign GetCampaignFromVM(CampaignVM viewModel)
        {

            return new Service.Entity.Campaign
            {
                ContentTemplate = viewModel.Content,
                CreatedBy = viewModel.CreatedBy,
                CreatedDate = DateTime.Now,
                FileName = viewModel.FileName,
                Id = viewModel.Id,
                Name = viewModel.Name,
                StatusDate = viewModel.StatusDate,
                StatusId = viewModel.StatusId,
                SubjectTemplate = viewModel.Subject,
                RecipientField = viewModel.RecipientField,
                TypeId = viewModel.TypeId
            };
        }


        public CampaignVM GetVMFromCampaign(Campaign campaign)
        {

            return new CampaignVM
            {
                Content = campaign.ContentTemplate,
                CreatedBy = campaign.CreatedBy,
                CreatedDate = campaign.CreatedDate,
                Id = campaign.Id,
                Name = campaign.Name,
                StatusDate = campaign.StatusDate,
                StatusId = campaign.StatusId,
                Subject = campaign.SubjectTemplate,
                TypeId = campaign.TypeId,
                RecipientField = campaign.RecipientField,
                FileName = campaign.FileName
            };
        }
    }
}