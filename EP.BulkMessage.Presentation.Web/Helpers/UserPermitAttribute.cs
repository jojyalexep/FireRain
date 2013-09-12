using EP.BulkMessage.Presentation.Web.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EP.BulkMessage.Presentation.Web.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class UserPermitAttribute : ActionFilterAttribute
    {

        public string Permission { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!String.IsNullOrEmpty(Permission))
            {
                if (UserPermission.UserHasPermission(Permission))
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    throw new HttpException(404, "No permission to Access");
                }
            }
            else
            {
                throw new ArgumentNullException("Permission");
            }

            
        }
    }
}