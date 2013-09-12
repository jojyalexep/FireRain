﻿using EP.BulkMessage.Service.Entity;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories.Mappings
{
    public class EmailMap : ClassMap<Email>
    {
        public EmailMap()
        {
            Table("EMAIL");
            Id(x => x.Id).Not.Nullable().GeneratedBy.Native(builder => builder.AddParam("sequence", "SEQ_EMAIL"));
            Map(x => x.ToAddress);
            Map(x => x.Subject);
            Map(x => x.Body);
            Map(x => x.StatusDate);
            Map(x => x.StatusId);
            Map(x => x.CampaignId);
        }
    }
}