using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    using Models;
    public class ApprovalEntriesController : Controller
    {
        // GET: ApprovalEntries
        public ActionResult Index()
        {
            List<ApprovalEntriesListViewModel> _ApprovalEntriesListViewModel = new List<ApprovalEntriesListViewModel>();
            _ApprovalEntriesListViewModel.Add(new ApprovalEntriesListViewModel {EntryNo = 1, DocumentType = "Leave", DocumentNo = "DOC0001", DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()),EmployeeName = "Derrick Witness Abucheri", EndDate = "July 6 2021", LeaveDays ="1", StartDate = "July 6 2021", ApprovedLeaveType = "Annual Leave", ApprovalStatus = "Open" });
            _ApprovalEntriesListViewModel.Add(new ApprovalEntriesListViewModel {EntryNo = 2, DocumentType = "Leave", DocumentNo = "DOC0002", DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()), EmployeeName = "Derrick Witness Abucheri", EndDate = "July 6 2021", LeaveDays = "1", StartDate = "July 6 2021", ApprovedLeaveType = "Annual Leave", ApprovalStatus = "Open" });
            return View(_ApprovalEntriesListViewModel);
        }
    }
}