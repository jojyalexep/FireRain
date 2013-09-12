using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Presentation.Web.Utilities
{
    public  class ElmahLog
    {
        public static void Create(string message)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(message));
        }

    }
}