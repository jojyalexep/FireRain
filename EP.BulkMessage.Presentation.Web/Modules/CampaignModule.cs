using EP.BulkMessage.Presentation.Web.DataAdapter;
using EP.BulkMessage.Presentation.Web.Entity;
using EP.BulkMessage.Presentation.Web.Entity.Enum;
using EP.BulkMessage.Presentation.Web.Validation;
using EP.BulkMessage.Presentation.Web.ViewModel;
using EP.BulkMessage.Service.Domain.CampaignModule;
using EP.BulkMessage.Service.Domain.UserModule;
using EP.BulkMessage.Service.Entity;
using EP.BulkMessage.Service.Entity.Enum;
using EP.BulkMessage.Service.ExternalServices;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EP.BulkMessage.Presentation.Web.Modules
{
    public class CampaignModule
    {

        public List<KeyValue> GetHeaders(HttpPostedFileBase file)
        {
            var fileName = SaveExcelFile(file);
            return GetExcelHeader(fileName);
        }


        public CampaignVM GetCampaign(int campaignId)
        {
            CampaignService campaignService = new CampaignService();
            var campaign = campaignService.GetCampaign(campaignId);

            CampaignAdapter adapter = new CampaignAdapter();
            return adapter.GetVMFromCampaign(campaign);

        }

        #region Services

        public bool ScheduleCampaign(int campaignId, DateTime scheduledDate)
        {
            CampaignService campaignService = new CampaignService();
            return campaignService.UpdateCampaignStatus(campaignId, CampaignStatus.Scheduled, scheduledDate);
        }

        public bool StopCampaign(int campaignId)
        {
            CampaignService campaignService = new CampaignService();
            return campaignService.UpdateCampaignStatus(campaignId, CampaignStatus.Stopped);
        }

        public bool ApproveCampaign(int campaignId)
        {
            CampaignService campaignService = new CampaignService();
            return campaignService.UpdateCampaignStatus(campaignId, CampaignStatus.Approved);
        }

        public bool RejectCampaign(int campaignId)
        {
            CampaignService campaignService = new CampaignService();
            return campaignService.UpdateCampaignStatus(campaignId, CampaignStatus.Rejected);
        }

        public bool DeleteCampaign(int campaignId)
        {
            CampaignService campaignService = new CampaignService();
            return campaignService.DeleteCampaign(campaignId);
        }


        public bool SendTestCampaign(int campaignId, string recepient)
        {
            CampaignService campaignService = new CampaignService();
            var campaign = campaignService.GetCampaign(campaignId);
            var user = new UserService().GetUser(campaign.CreatedBy);
            bool response = false;
            if (campaign.TypeId == (int)CampaignType.SMS)
            {
                SmsModule smsModule = new SmsModule(user.SmsUsername, user.SmsPassword);
                response = smsModule.SendSMS(recepient, campaign.ContentTemplate);
            }
            else
            {
                EmailModule emailModule = new EmailModule(user.EmailUsername, user.EmailPassword);
                response = emailModule.SendEmail(user.Name, recepient, "", "", campaign.SubjectTemplate, campaign.ContentTemplate);
            }
            return response;

        }


        public IEnumerable<Email> GetCampaignEmails(int campaignId, MessageStatus status = MessageStatus.All)
        {
            CampaignService campaignService = new CampaignService();
            return campaignService.GetCampaignEmails(campaignId, status);
        }

        public IEnumerable<Sms> GetCampaignSmses(int campaignId, MessageStatus status = MessageStatus.All)
        {
            CampaignService campaignService = new CampaignService();
            return campaignService.GetCampaignSmses(campaignId, status);
        }


        public List<CampaignVM> GetAllCampaigns()
        {
            List<CampaignVM> campaignVMList = new List<CampaignVM>();
            CampaignService campaignService = new CampaignService();
            int departmentId = UserModule.GetUsersDepartment();
            var campaigns = campaignService.GetAllCampaigns(departmentId);


            CampaignAdapter adapter = new CampaignAdapter();

            foreach (var campaign in campaigns)
            {
                campaignVMList.Add(adapter.GetVMFromCampaign(campaign));
            }

            return campaignVMList;

        }

        public List<CampaignVM> GetApprovalCampaigns()
        {
            List<CampaignVM> campaignVMList = new List<CampaignVM>();
            CampaignService campaignService = new CampaignService();
            int departmentId = UserModule.GetUsersDepartment();
            var campaigns = campaignService.GetApprovalCampaigns(departmentId);

            CampaignAdapter adapter = new CampaignAdapter();
            foreach (var campaign in campaigns)
            {
                campaignVMList.Add(adapter.GetVMFromCampaign(campaign));
            }
            return campaignVMList;
        }
        #endregion




        public CampaignReport ValidateCampaign(CampaignVM campaignVM)
        {
            CampaignValidation validations = new CampaignValidation();
            var fileName = campaignVM.FileName;
            var fields = GetAllMappingFields(campaignVM.Content);
            fields.AddRange(GetAllMappingFields(campaignVM.RecipientField));
            if (campaignVM.TypeId == (int)CampaignType.Email)
                fields.AddRange(GetAllMappingFields(campaignVM.Subject));

            var excelRows = GetExcelMappings(fileName);
            ValidateExcelForFields(ref validations, fields, excelRows);

            var validRows = new List<Row>();
            if (validations.IsValid)
            {
                int rowCount = 2;
                foreach (var row in excelRows)
                {
                    var recepient = row[GetAllMappingFields(campaignVM.RecipientField)[0]];
                    if (validations.Validate(recepient, (CampaignType)campaignVM.TypeId, rowCount))
                        validRows.Add(row);
                    rowCount += 1;
                }

            }
            validations.TotalRecordCount = excelRows.Count();
            var report = new CampaignReport
            {
                Campaign = campaignVM,
                ExcelRows = validRows,
                Validations = validations
            };
            return report;
        }

        public int SaveCampaign(CampaignReport report)
        {
            try
            {
                var campaignVM = report.Campaign;

                CampaignService campaignService = new CampaignService();
                var campaign = new CampaignAdapter().GetCampaignFromVM(campaignVM);
                campaign.DepartmentId = UserModule.GetUsersDepartment();
                

                if (campaign.Id == 0)
                    campaign.Id = campaignService.SaveCampaign(campaign);
                else
                {
                    campaignService.ReCreateCampaign(campaign, GetCampaign(campaign.Id).TypeId);
                }


                if (campaignVM.TypeId == (int)CampaignType.Email)
                {
                    List<Email> emailList = new List<Email>();
                    foreach (var row in report.ExcelRows)
                    {
                        var recipient = ReplaceContentWithValue(row, campaignVM.RecipientField);
                        Email email = new Email
                        {
                            Body = ReplaceContentWithValue(row, campaignVM.Content),
                            CampaignId = campaign.Id,
                            Id = 0,
                            StatusId = (int)MessageStatus.New,
                            StatusDate = DateTime.Now,
                            Subject = ReplaceContentWithValue(row, campaignVM.Subject),
                            ToAddress = recipient
                        };
                        emailList.Add(email);
                    }
                    campaignService.SaveCampaignEmails(emailList);
                }

                else
                {
                    List<Sms> smsList = new List<Sms>();
                    foreach (var row in report.ExcelRows)
                    {
                        var recipient = ReplaceContentWithValue(row, campaignVM.RecipientField);
                        string number = recipient.Replace("+", "").Replace("-", "").Replace(" ", ""); ;
                        Sms sms = new Sms
                        {

                            Content = ReplaceContentWithValue(row, campaignVM.Content),
                            CampaignId = campaign.Id,
                            Id = 0,
                            StatusId = (int)MessageStatus.New,
                            StatusDate = DateTime.Now,
                            ToNumber = number
                        };
                        smsList.Add(sms);
                    }

                    

                    campaignService.SaveCampaignSmses(smsList);
                }
                return campaign.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateCampaignFile(int campaignId)
        {
            CampaignService campaignService = new CampaignService();
            var campaign = campaignService.GetCampaign(campaignId);
            var newFileName = "camp" + campaign.Id + Path.GetExtension(campaign.FileName);

            File.Move(GetFileName(campaign.FileName), GetFileName(newFileName));
            campaign.FileName = newFileName;
            campaignService.UpdateCampaign(campaign);
        }

        private string SaveExcelFile(HttpPostedFileBase excelFile)
        {
            var fileName = GetFileName(excelFile.FileName);
            excelFile.SaveAs(fileName);
            return fileName;
        }

        private static string GetFileName(string rawName, string directory = "campaign")
        {
            // campaign
            var fileName = HttpContext.Current.Server.MapPath("~\\Data\\" + directory + "\\") + Path.GetFileName(rawName);
            return fileName;
        }


        private string ReplaceContentWithValue(Row excelRow, string content)
        {

            var fields = GetAllMappingFields(content);
            foreach (var field in fields)
            {
                content = content.Replace("{" + field + "}", excelRow[field]);
            }
            return content;

        }

        private IQueryable<Row> GetExcelMappings(string fileName)
        {
            List<ExcelMapping> excelMapList = new List<ExcelMapping>();
            var excel = new ExcelQueryFactory(GetFileName(fileName));
            var rows = from row in excel.Worksheet(0) select row;
            
            return rows;

        }

        private List<KeyValue> GetExcelHeader(string fileName)
        {
            List<ExcelMapping> excelMapList = new List<ExcelMapping>();
            var excel = new ExcelQueryFactory(fileName);
            var headers = excel.WorksheetNoHeader(0).FirstOrDefault();
            List<KeyValue> keyValueColl = new  List<KeyValue>();
            headers.ForEach(p=>keyValueColl.Add(new KeyValue{ Key =  p.Value.ToString(), Value = p.Value.ToString()}));
            return keyValueColl;

        }

        private void ValidateExcelForFields(ref CampaignValidation validations, List<string> fields, IQueryable<Row> excelRows)
        {
            foreach (var field in fields)
            {
                try
                {
                    var chkField = excelRows.First()[field];
                }
                catch (Exception ex)
                {
                    validations.Messages.Add(new CampaignValidationMessage(field, CampaignError.FieldMissing, 0));
                    validations.IsValid = false;
                }
            }
        }



        private List<string> GetAllMappingFields(string content)
        {
            string pattern = @"(?<=\{)(.*?)(?=\})";
            var matches = Regex.Matches(content, pattern);
            var fields = new List<string>();
            foreach (Match match in matches)
            {
                fields.Add(match.Value.Replace("{", "").Replace("}", ""));
            }
            return fields;
        }




    }
}
