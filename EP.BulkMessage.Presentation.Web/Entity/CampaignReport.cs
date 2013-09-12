using EP.BulkMessage.Presentation.Web.Validation;
using EP.BulkMessage.Presentation.Web.ViewModel;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Presentation.Web.Entity
{
    public class CampaignReport
    {
        public CampaignVM Campaign { get; set; }
        public List<Row> ExcelRows { get; set; }
        public CampaignValidation Validations { get; set; }
    }
}