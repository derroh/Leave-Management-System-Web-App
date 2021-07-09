using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    using HumanResources.Models;
    using Newtonsoft.Json;

    public class LeaveRecallsController : Controller
    {        
        [HttpPost]
        public ActionResult Index()
        {
            List<LeaveRecallsListViewModel> _LeaveRecallsListViewModel = new List<LeaveRecallsListViewModel>();

            for (int i = 1; i <= 10; i++)
            {
                _LeaveRecallsListViewModel.Add(new LeaveRecallsListViewModel { DocumentNo = "LEAVE00" + 1, EmployeeName = "Derrick Witness Abucheri", ApprovalStatus = "open", DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()), EndDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), StartDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), LeaveDaysRecalled = i.ToString(), DocumentType = "Leave", LeaveTypeRecalled = "Annual Leave", ApprovalProgress = i * 10 });
            }
            return View(_LeaveRecallsListViewModel);
        }
       [HttpGet]
        public ActionResult Index(string status)
        {
            List<LeaveRecallsListViewModel> _LeaveRecallsListViewModel = new List<LeaveRecallsListViewModel>();

            for (int i = 1; i <= 10; i++)
            {
                _LeaveRecallsListViewModel.Add(new LeaveRecallsListViewModel { DocumentNo = "LEAVE00" + 1, EmployeeName = "Derrick Witness Abucheri", ApprovalStatus = "open", DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()), EndDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), StartDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), LeaveDaysRecalled = i.ToString(), DocumentType = "Leave", LeaveTypeRecalled = "Annual Leave", ApprovalProgress = i * 10 });
            }
            return View(_LeaveRecallsListViewModel);
        }
        public ActionResult Create()
        {
            return View();
        }
        //Approal stuff
        public ActionResult Submit(string DocumentNo)
        {
            var _RequestResponse = new RequestResponse
            {
                Status = "900",
                Message = "Submit Success! for leave recall " + DocumentNo
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Cancel(string DocumentNo)
        {
            var _RequestResponse = new RequestResponse
            {
                Status = "900",
                Message = "Cancel Success! for leave recall " + DocumentNo
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
    }
}