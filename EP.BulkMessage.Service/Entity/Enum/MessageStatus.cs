using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Entity.Enum
{
    public enum MessageStatus
    {
        New = 1,
        NotSend = 2,
        Send = 3,
        Failed = 4,
        All = 5,
        Deleted = 6
    }
}