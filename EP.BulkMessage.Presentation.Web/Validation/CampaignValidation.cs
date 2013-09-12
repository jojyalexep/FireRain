using EP.BulkMessage.Presentation.Web.Entity.Enum;
using EP.BulkMessage.Service.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace EP.BulkMessage.Presentation.Web.Validation
{
    public class CampaignValidation
    {

        public CampaignValidation()
        {
            Messages = new List<CampaignValidationMessage>();
            IsValid = true;
        }


        public List<CampaignValidationMessage> Messages { get; set; }
        public int TotalRecordCount { get; set; }
        public int TotalRecordFailed { get; set; }
        public bool IsValid { get; set; }
        public void AddError(CampaignValidationMessage message)
        {
            TotalRecordFailed += 1;
            Messages.Add(message);
        }

        public bool Validate(string fieldValue, CampaignType type, int rowNumber)
        {

            if (String.IsNullOrEmpty(fieldValue))
            {
                AddError(new CampaignValidationMessage("-", CampaignError.ReceipientEmpty, rowNumber));
                return false;
            }
            else
            {
                if (type == CampaignType.Email)
                {

                    if (Regex.IsMatch(fieldValue, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                        return true;
                    else
                        AddError(new CampaignValidationMessage(fieldValue, CampaignError.EmailFieldError, rowNumber));

                    return false;
                }
                else
                {
                    fieldValue = fieldValue.Replace("+", "").Replace("-", "").Replace(" ", "");

                    //if (Regex.IsMatch(fieldValue, @"^(\+97[\s]{0,1}[\-]{0,1}[\s]{0,1}1|0)5[\s]{0,1}[\-]{0,1}[\s]{0,1}[0-9]{1}[0-9]{7}$"))
                    if (Regex.IsMatch(fieldValue, @"(9715)([0-9]{8})|(05)([0-9]{8})|(5)([0-9]{8})"))
                        return true;
                    else
                        AddError(new CampaignValidationMessage(fieldValue, CampaignError.MobileFieldError, rowNumber));
                    return false;
                }
            }
        }
    }

    public class CampaignValidationMessage
    {

        public CampaignValidationMessage(string errorInput, CampaignError error,int rowNumber)
        {
            RowNumber = rowNumber;
            ErrorInput = errorInput;
            Error = error;
            switch (error)
            {
                case(CampaignError.EmailFieldError) : 
                    {
                        Message = ErrorInput + " is not a valid email address";
                        break;
                    }

                case (CampaignError.MobileFieldError):
                    {
                        Message = ErrorInput + " is not a valid mobile number";
                        break;
                    }

                case (CampaignError.FieldMissing):
                    {
                        Message = ErrorInput + " is not found in the excel file";
                        break;
                    }

                case (CampaignError.ReceipientEmpty):
                    {
                        Message = "An empty string is found in the recipient column";
                        break;
                    }

            }
            
        }
        public CampaignValidationMessage(string message, string errorInput, CampaignError error, int rowNumber)
        {
            Message = message;
            ErrorInput = errorInput;
            Error = error;
            RowNumber = rowNumber;
        }

        public int RowNumber { get; set; }
        public string Message { get; set; }
        public string ErrorInput { get; set; }
        public CampaignError Error { get; set; }
    }


    public enum CampaignError
    {
        FieldMissing,
        MobileFieldError,
        EmailFieldError,
        ReceipientEmpty

    }

}