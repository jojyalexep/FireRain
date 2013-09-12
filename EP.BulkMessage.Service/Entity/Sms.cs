using EP.BulkMessage.Service.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Entity
{
    [Serializable]
    public class Sms
    {
        public virtual int Id { get; set; }
        public virtual string ToNumber { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime StatusDate { get; set; }
        public virtual int StatusId { get; set; }
        public virtual string Status { get { return ((MessageStatus)StatusId).ToString(); } set { } }
        public virtual int CampaignId { get; set; }
    }
}