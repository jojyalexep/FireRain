using EP.BulkMessage.Service.Entity;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories.Mappings
{
    public class ScheduleMap : ClassMap<Schedule>
    {
        public ScheduleMap()
        {
         Table("SCHEDULE");
            Id(x => x.Id);
            Map(x => x.StartTime);
            Map(x => x.EndTime);
            Map(x => x.Frequency);
            Map(x => x.EmailBatchSize);
            Map(x => x.SmsBatchSize);
        }
    }
}