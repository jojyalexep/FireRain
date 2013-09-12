using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.BulkMessage.Scheduler
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Frequency { get; set; }
        public int EmailBatchSize { get; set; }
        public int SmsBatchSize { get; set; }
    }
}
