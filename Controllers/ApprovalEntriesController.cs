using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    using Models;
    using Newtonsoft.Json;
    using HumanResources.ViewModels;
    using HumanResources.CustomFunctions;
    using System.IO;

    [Authorize(Roles = "Admin")]
    public class ApprovalEntriesController : Controller
    {
        private static HumanResourcesManagementSystemEntities _db = new HumanResourcesManagementSystemEntities();
        // GET: ApprovalEntries
        public ActionResult Index()
        {
            string status = Request.QueryString["status"];
            List<ApprovalEntriesListViewModel> _ApprovalEntriesListViewModel = new List<ApprovalEntriesListViewModel>();

            if (status == "pending") status = "open";
            var approvalentries = _db.ApprovalEntries.Where(a => a.Status == status).ToList();

            foreach (var approvalentry in approvalentries)
            {
                _ApprovalEntriesListViewModel.Add(new ApprovalEntriesListViewModel { EntryNo = approvalentry.EntryNumber, DocumentType = "Leave", DocumentNo = approvalentry.DocumentNo, DateSubmitted = AppFunctions.GetReadableDate((approvalentry.DateSent).ToString()), EmployeeName = approvalentry.SenderId, EndDate = "July 6 2021", LeaveDays = "1", StartDate = "July 6 2021", ApprovedLeaveType = "Annual Leave", ApprovalStatus = approvalentry.Status });
            }
            return View(_ApprovalEntriesListViewModel);
        }
        public JsonResult Approve(int EntryNo)
        {
            string status = "", message = "";

            var approvalentries = _db.ApprovalEntries.Where(a => a.EntryNumber == EntryNo).FirstOrDefault();

            if(approvalentries!= null)
            {
                int SequenceNo = Convert.ToInt32(approvalentries.SequenceNo);
                string DocumentNo = approvalentries.DocumentNo;

                string approvalrequestresponse = MakerChecker.ApproveAppovalRequest(EntryNo, SequenceNo, "Leave", DocumentNo);

                ApprovedRequestResponse _ApprovedRequestResponse = JsonConvert.DeserializeObject<ApprovedRequestResponse>(approvalrequestresponse);

                status = _ApprovedRequestResponse.Status;

                if (status == "000")
                {
                    string body = string.Empty;

                    string domainName = Request.Url.GetLeftPart(UriPartial.Authority);

                    string url = Url.Action("Login", "Account");

                    string pathToTemplate = Server.MapPath("~/MailTemplates/ApprovalCompletedNotification.html");

                    using (StreamReader reader = new StreamReader(pathToTemplate))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{Link}", domainName + url);
                    body = body.Replace("{LeaveNo}", _ApprovedRequestResponse.DocumentNo);
                    body = body.Replace("{UserName}", _ApprovedRequestResponse.SenderEmail);

                    bool IsSendEmail = EmailFunctions.SendMail(_ApprovedRequestResponse.SenderEmail, _ApprovedRequestResponse.SenderEmail, "Approval Complete!", body);


                    status = "000";
                    message = "Approval success for leave " + approvalentries.DocumentNo;
                }
                else
                {
                    status = "999";
                    message = "Approval failed for leave " + approvalentries.DocumentNo;
                }
            }
            else
            {
                status = "999";
                message = "Approval entry not found";
            }           

            var _RequestResponse = new RequestResponse
            {
                Status = status,
                Message = message
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Reject(int EntryNo)
        {

            var _RequestResponse = new RequestResponse
            {
                Status = "900",
                Message = "Reject Success! for " + EntryNo.ToString()
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewApprovalEntries(string id)
        {
            return RedirectToAction("ViewLeave", "Leaves", new { id = id });
        }
    }
}