using CsvHelper;
using Elmah;
using EP.BulkMessage.Presentation.Web.Entity;
using EP.BulkMessage.Presentation.Web.Entity.Enum;
using EP.BulkMessage.Presentation.Web.Helpers;
using EP.BulkMessage.Presentation.Web.Modules;
using EP.BulkMessage.Presentation.Web.ViewModel;
using EP.BulkMessage.Service.Domain.CampaignModule;
using EP.BulkMessage.Service.Entity;
using EP.BulkMessage.Service.Entity.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EP.BulkMessage.Presentation.Web.Controllers
{
    [Authorize]
    [HandleErrorWithELMAH]
    public class CampaignController : Controller
    {
        //
        // GET: /Campaign/

        public ActionResult Index(int messageId = 0)
        {
            SetCampaignMessage(messageId);
            var campaigns = new CampaignModule().GetAllCampaigns().OrderByDescending(p => p.Id);
            return View(campaigns);
        }

        public ActionResult Schedules(int messageId = 0)
        {
            var schedules = new ScheduleService().GetSchedules().OrderBy(p => p.StartTime);
            return View(schedules);
        }

        [UserPermit(Permission = UserPermission.CreateCampaignPermission)]
        public ActionResult Create(int id = 0)
        {
            CampaignVM campaign = (id == 0) ? new CampaignVM() : new CampaignModule().GetCampaign(id);
            ViewBag.CampaignTypes = GetCampaignTypes();
            return View(campaign);
        }

        public ActionResult CreateSms()
        {
            return View(new CampaignVM(CampaignType.SMS));
        }

        public ActionResult CreateEmail()
        {
            return View(new CampaignVM(CampaignType.Email));
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase File)
        {
            string[] formats = new string[] { ".xls", ".xlsx", ".csv"};

            if (File != null && formats.Contains(Path.GetExtension(File.FileName)))
            {
                var headers = new CampaignModule().GetHeaders(File);
                ModelState.Clear();
                return View("FileHeaders", headers);
            }
            throw new ArgumentNullException("File corrupted");
        }

        [HttpPost]
        [UserPermit(Permission = UserPermission.CreateCampaignPermission)]
        public ActionResult Create(CampaignVM campaign)
        {
            try
            {
                ViewBag.CampaignTypes = GetCampaignTypes();
                if (ModelState.IsValid)
                {
                    ViewBag.CampaignTypes = GetCampaignTypes();
                    campaign.CreatedBy = User.Identity.Name;
                    var campaignModule = new CampaignModule();
                    var report = campaignModule.ValidateCampaign(campaign);
                    TempData["CampaignReport"] = report;
                    return View("Report", report.Validations);
                }
                ViewBag.Message = "Error in input. Please correct the errors";

                return View(campaign);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Message = "Cannot create campaign";
                return View(campaign);

            }
        }

        [UserPermit(Permission = UserPermission.CreateCampaignPermission)]
        public ActionResult Save()
        {
            var report = (CampaignReport)TempData["CampaignReport"];
            var campaignModule = new CampaignModule();
            int campaignId = campaignModule.SaveCampaign(report);
            campaignModule.UpdateCampaignFile(campaignId);
            return RedirectToAction("Index", new { messageId = (int)ViewMessage.CampaignSaved });

        }

        [UserPermit(Permission = UserPermission.ApproveCampaignPermission)]
        public ActionResult ApprovalDetails(int id)
        {
            CampaignModule campaignModule = new CampaignModule();

            var campaign = campaignModule.GetCampaign(id);
            if (campaign.TypeId == (int)CampaignType.Email)
            {
                ViewBag.EmailList = campaignModule.GetCampaignEmails(campaign.Id);
            }
            else
            {
                ViewBag.SmsList = campaignModule.GetCampaignSmses(campaign.Id);
            }


            return View(campaign);
        }

        public ActionResult Details(int id)
        {
            CampaignModule campaignModule = new CampaignModule();

            var campaign = campaignModule.GetCampaign(id);
            int newStatus = 0, send = 0, failed = 0;

            if (campaign.TypeId == (int)CampaignType.Email)
            {
                var emailList = campaignModule.GetCampaignEmails(campaign.Id);
                if (emailList != null)
                {
                    newStatus = emailList.Where(p => p.StatusId == (int)MessageStatus.New).Count();
                    send = emailList.Where(p => p.StatusId == (int)MessageStatus.Send).Count();
                    failed = emailList.Where(p => p.StatusId == (int)MessageStatus.Failed).Count();
                }

            }
            else
            {
                var smsList = campaignModule.GetCampaignSmses(campaign.Id);
                if (smsList != null)
                {
                    newStatus = smsList.Where(p => p.StatusId == (int)MessageStatus.New).Count();
                    send = smsList.Where(p => p.StatusId == (int)MessageStatus.Send).Count();
                    failed = smsList.Where(p => p.StatusId == (int)MessageStatus.Failed).Count();
                }
            }
            ViewBag.NewStatus = newStatus.ToString();
            ViewBag.Send = send.ToString();
            ViewBag.Failed = failed.ToString();

            return View(campaign);
        }

        public FileResult Export(int campaignId, int statusId)
        {

            CampaignModule campaignModule = new CampaignModule();
            var campaign = campaignModule.GetCampaign(campaignId);
            string fileName = "export-" + campaign.Id + ".csv";
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
                if (campaign.TypeId == (int)CampaignType.Email)
                {
                    var emailList = campaignModule.GetCampaignEmails(campaign.Id).Where(p => p.StatusId == statusId);
                    using (var csv = new CsvWriter(streamWriter))
                    {
                        csv.WriteRecords(emailList);
                    }
                }
                else
                {
                    var smsList = campaignModule.GetCampaignSmses(campaign.Id).Where(p => p.StatusId == statusId);
                    using (var csv = new CsvWriter(streamWriter))
                    {
                        csv.WriteRecords(smsList);
                    }
                }

                MemoryStream newStream = new MemoryStream(memoryStream.ToArray());
                newStream.Seek(0, SeekOrigin.Begin);

                return File(newStream, "text/csv", fileName);
        }

        [UserPermit(Permission= UserPermission.ApproveCampaignPermission)]
        public ActionResult Approval()
        {
            var campaigns = new CampaignModule().GetAllCampaigns().OrderByDescending(p => p.Id);
            return View(campaigns);
        }

        [UserPermit(Permission = UserPermission.ApproveCampaignPermission)]
        public ActionResult Approve(int id)
        {
            var response = new CampaignModule().ApproveCampaign(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [UserPermit(Permission = UserPermission.ApproveCampaignPermission)]
        public ActionResult Reject(int id)
        {
            var response = new CampaignModule().RejectCampaign(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [UserPermit(Permission = UserPermission.DeleteCampaignPermission)]
        public ActionResult Delete(int id)
        {
            var response = new CampaignModule().DeleteCampaign(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Schedule(int id, DateTime scheduledDate, DateTime scheduledTime)
        {
            scheduledDate = scheduledDate.Add(scheduledTime.Subtract(scheduledTime.Date));
            var response = new CampaignModule().ScheduleCampaign(id, scheduledDate);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Stop(int id)
        {
            var response = new CampaignModule().StopCampaign(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SendTestMsg(int id, string recepient)
        {
            var response = new CampaignModule().SendTestCampaign(id, recepient);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private SelectList GetCampaignTypes()
        {
            var campaignTypes = new List<KeyValue>
            {
                new KeyValue{ Key= CampaignType.Email.ToString(), Value=((int)CampaignType.Email).ToString() },
                new KeyValue{ Key= CampaignType.SMS.ToString(), Value=((int)CampaignType.SMS).ToString() }
            };

            return new SelectList(campaignTypes, "Value", "Key");
        }

        private void SetCampaignMessage(int messageId)
        {
            string message = String.Empty;
            switch (messageId)
            {

                case (int)ViewMessage.CampaignSaved:
                    {
                        message = "Campaign has been saved successfully";
                        break;
                    }
                case (int)ViewMessage.CampaignFailed:
                    {
                        message = "Campaign could not be saved";
                        break;
                    }
                case (int)ViewMessage.CampaignDeleted:
                    {
                        message = "Campaign has been deleted successfully";
                        break;
                    }
                case (int)ViewMessage.CampaignScheduled:
                    {
                        message = "Campaign has been scheduled successfully";
                        break;

                    }

                case (int)ViewMessage.CampaignStopped:
                    {
                        message = "Campaign has been stopped successfully";
                        break;

                    }

                case (int)ViewMessage.CampaignTest:
                    {
                        message = "Test message has been send successfully";
                        break;

                    }

                case (int)ViewMessage.CampaignApproved:
                    {
                        message = "Campaign has been approved successfully";
                        break;

                    }


            }
            if (!String.IsNullOrEmpty(message))
                ViewBag.Message = message;
        }
    }

}
