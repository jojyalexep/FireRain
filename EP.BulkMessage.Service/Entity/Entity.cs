using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Entity
{
    public class Entity
    {
        public virtual int Id { get; set; }
        public virtual bool StatusId { get; set; }

        public virtual bool IsTransient()
        {
            return this.Id == 0;
        }
    }
}