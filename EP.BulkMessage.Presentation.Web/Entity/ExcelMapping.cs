using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Presentation.Web.Entity
{
    public class ExcelMapping
    {
         public  ExcelMapping()
         {
             ValueList = new List<string>();
         }

        public string Name { get; set; }
        public List<string> ValueList { get; set; }
    }
}