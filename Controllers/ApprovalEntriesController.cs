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

    public class ApprovalEntriesController : Controller
    {
        private static LeaveManagementSystemEntities _db = new LeaveManagementSystemEntities();
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

                if (MakerChecker.ApproveAppovalRequest(EntryNo, SequenceNo, "Leave", DocumentNo))
                {
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
    }
}