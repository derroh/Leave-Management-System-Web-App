using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    using Models;
    using Newtonsoft.Json;

    public class ApprovalEntriesController : Controller
    {
        // GET: ApprovalEntries
        public ActionResult Index()
        {
            List<ApprovalEntriesListViewModel> _ApprovalEntriesListViewModel = new List<ApprovalEntriesListViewModel>();
            for(int i =1; i <= 17; i++)
            {
                _ApprovalEntriesListViewModel.Add(new ApprovalEntriesListViewModel { EntryNo = i, DocumentType = "Leave", DocumentNo = "DOC000"+i, DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()), EmployeeName = "Derrick Witness Abucheri", EndDate = "July 6 2021", LeaveDays = "1", StartDate = "July 6 2021", ApprovedLeaveType = "Annual Leave", ApprovalStatus = "Open" });
            }
            return View(_ApprovalEntriesListViewModel);
        }
        public JsonResult Approve(int EntryNo)
        {           

            var _RequestResponse = new RequestResponse
            {
                Status = "900",
                Message = "Approve Success! for " + EntryNo.ToString()
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