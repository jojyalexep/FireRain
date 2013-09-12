using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Entity
{
    [Serializable]
    public class Campaign
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int TypeId { get; set; }
        
        public virtual DateTime StatusDate { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual int StatusId { get; set; }
        public virtual string RecipientField { get; set; }
        
        public virtual string ContentTemplate { get; set; }
        public virtual string SubjectTemplate { get; set; }
        public virtual string FileName { get; set; }

        public virtual int PercentComplete { get; set; }

        public virtual int DepartmentId { get; set; }
    }
}