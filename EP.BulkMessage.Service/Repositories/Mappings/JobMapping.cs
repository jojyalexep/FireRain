using EP.BulkMessage.Service.Entity;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories.Mappings
{
    public class JobMapping: ClassMap<Job>
    {
        public JobMapping()
        {
            Table("JOB");
            Id(x => x.Id).Not.Nullable().GeneratedBy.Native(builder => builder.AddParam("sequence", "SEQ_JOB"));
            Map(x => x.Name);
            Map(x => x.StartDate);
            Map(x => x.EndDate);
            Map(x => x.StatusId);

            Map(x => x.EmailBatch);
            Map(x => x.SmsBatch);
            Map(x => x.EmailSent);
            Map(x => x.SmsSent);

        }
    }
}