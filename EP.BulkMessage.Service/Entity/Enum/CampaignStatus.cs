using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.Entity.Enum
{
    public enum CampaignStatus
    {
        New = 1,
        Approved = 2,
        Scheduled = 3,
        Progress = 4,
        Completed = 5,
        Deleted =6,
        Stopped = 7,
        Rejected = 8,
    }
}