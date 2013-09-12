using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EP.BulkMessage.Service.ExternalServices
{
    public class EmailModule
    {

        private string _username;
        private string _password;

        public EmailModule(string username, string password)
        {
            _username = username;
            _password = password;
        }


        public bool SendEmail(string from, string to, string cc, string bcc, string subject, string body)
        {

            try
            {
                var middlewareService = new MiddlewareService.MiddlewareServicePortTypeClient();
                string response = middlewareService.doService(_username, _password, FormatEmail(from, to, cc, bcc, subject, body));
                if (response == "<SENT />")
                {
                    return true;
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


        private string FormatEmail(string from, string to, string cc, string bcc, string subject, string body)
        {
              string emailFormat  =  "<EMAIL_SEND><Sender>{0}</Sender><Receiver>{1}</Receiver><Subject>{2}</Subject><Content><![CDATA[{3}]]></Content></EMAIL_SEND>";
              return String.Format(emailFormat, from, to, subject, body);
        }



    }
}