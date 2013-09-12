using EP.BulkMessage.Service.Entity;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories.Mappings
{
    public class EPUserMap : ClassMap<EPUser>
    {
        public EPUserMap()
        {
            Table("EPUSER");
            Id(x => x.Id).Not.Nullable().GeneratedBy.Native(builder => builder.AddParam("sequence", "SEQ_USER"));
            Map(x => x.Name);
            Map(x => x.Username);
            Map(x => x.Password);
            Map(x => x.EmailUsername);
            Map(x => x.EmailPassword);
            Map(x => x.SmsUsername);
            Map(x => x.SmsPassword);
            Map(x => x.Role);
            References(c => c.Department);
        }
    }
}