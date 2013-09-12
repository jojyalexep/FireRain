using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Entity
{
    public class Job
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual int StatusId { get; set; }

        public virtual int EmailBatch { get; set; }
        public virtual int SmsBatch { get; set; }
        public virtual int EmailSent { get; set; }
        public virtual int SmsSent { get; set; }
    }
}