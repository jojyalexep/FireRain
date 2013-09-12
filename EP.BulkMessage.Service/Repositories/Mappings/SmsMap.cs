using EP.BulkMessage.Service.Entity;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories.Mappings
{
    public class SmsMap : ClassMap<Sms>
    {
        public SmsMap()
        {
            Table("SMS");
            Id(x => x.Id).Not.Nullable().GeneratedBy.Native(builder => builder.AddParam("sequence", "SEQ_SMS"));
            Map(x => x.ToNumber);
            Map(x => x.Content);
            Map(x => x.StatusDate);
            Map(x => x.StatusId);
            Map(x => x.CampaignId);
        }
    }
}