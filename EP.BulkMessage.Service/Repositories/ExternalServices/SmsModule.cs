using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace EP.BulkMessage.Service.ExternalServices
{
    public class SmsModule
    {


        private string _username;
        private string _password;

        public SmsModule(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public bool SendSMS(string to, string content)
        {

            try
            {
                var middlewareService = new MiddlewareService.MiddlewareServicePortTypeClient();
                //string asdf = middlewareService.healthCheck();
                string response =  middlewareService.doSMS(_username, _password, content, to);
                if (response.Contains("<STATUS>OK</STATUS>"))
                {
                    return true;
                    //"<SENT><SEND_STATUS><+971528785483><STATUS>OK</STATUS></+971528785483></SEND_STATUS></SENT>"
                }
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(response));
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }

            
        }

    }
}