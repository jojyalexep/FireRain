using EP.BulkMessage.Service.Entity;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Repositories.Mappings
{
    public class DepartmentMap : ClassMap<Department>
    {

        public DepartmentMap()
        {
            Table("DEPARTMENT");
            Id(x => x.Id).Not.Nullable();
            Map(x => x.Name);
        }
    }
}