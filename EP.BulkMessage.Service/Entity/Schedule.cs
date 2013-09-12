using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Entity
{
    public class Schedule
    {
        public virtual int Id { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual int Frequency { get; set; }
        public virtual int EmailBatchSize { get; set; }
        public virtual int SmsBatchSize { get; set; }
    }
}