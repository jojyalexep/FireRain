using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Entity
{
    public class EPUser
    {
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual string EmailUsername { get; set; }
        public virtual string EmailPassword { get; set; }
        public virtual string SmsUsername { get; set; }
        public virtual string SmsPassword { get; set; }
        public virtual string Role { get; set; }
        public virtual Department Department { get; set; }
    }
}