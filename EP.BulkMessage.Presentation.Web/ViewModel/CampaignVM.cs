using EP.BulkMessage.Presentation.Web.Entity;
using EP.BulkMessage.Service.Entity.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EP.BulkMessage.Presentation.Web.ViewModel
{
    public class CampaignVM
    {
        public CampaignVM()
        {

        }

        public CampaignVM(CampaignType campaignType)
        {
            TypeId = (int)campaignType;
        }



        [Required]
        [AllowHtml]
        public string Content { get; set; }

        [Required]
        public string FileName { get; set; }
        [Required]
        [Display(Name = "Email Field")]
        public string RecipientField { get; set; }

        public string Subject { get; set; }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Campaign Type")]
        [Required]
        public int TypeId { get; set; }
        public string Type { get { return ((CampaignType)TypeId).ToString(); } }

        [Display(Name = "Status Date")]
        public DateTime StatusDate { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        public int StatusId { get; set; }
        public string Status { get { return ((CampaignStatus)StatusId).ToString(); } }

        public int PercentComplete { get; set; }
    }
}