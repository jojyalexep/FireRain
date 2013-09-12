using EP.BulkMessage.Service.Entity;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories.Mappings
{
    public class CampaignMap : ClassMap<Campaign>
    {
        public CampaignMap()
        {
            Table("CAMPAIGN");
            Id(x => x.Id).Not.Nullable().GeneratedBy.Native(builder => builder.AddParam("sequence", "SEQ_CAMPAIGN"));
            Map(x => x.Name);
            Map(x => x.TypeId);
            Map(x => x.StatusDate);
            Map(x => x.CreatedDate);
            Map(x => x.CreatedBy);
            Map(x => x.StatusId);
            Map(x => x.RecipientField);
            Map(x => x.ContentTemplate);
            Map(x => x.SubjectTemplate);
            Map(x => x.FileName);
            Map(x => x.DepartmentId);
        }
    }
}