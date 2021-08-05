using HumanResources.Models;
using HumanResources.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            string username = HttpContext.Session["EmployeeNo"].ToString();
            string CurrentYear = DateTime.Now.Year.ToString();
            string PreviousYear = (DateTime.Now.Year - 1).ToString();

            List<LeaveBalancesView> _LeaveBalancesList = new List<LeaveBalancesView>();

            dynamic mymodel = new ExpandoObject();

            //get leave balances for each of the leave types

            //get number of days entilted

            using (var dbEntities = new LeaveManagementEntities())
            {
                var employee = dbEntities.Employees.Where(x => x.EmployeeNo == username).FirstOrDefault();
                int ApprovalStatus = (int)DocumentApprovalStatus.Approved;
                var ApprovedLeaves = dbEntities.ApprovalEntries.Where(s => s.SenderId == username && s.Status == ApprovalStatus).ToList().Count();

                ApprovalStatus = (int)DocumentApprovalStatus.Rejected;
                var RejectedLeaves = dbEntities.ApprovalEntries.Where(s => s.SenderId == username && s.Status == ApprovalStatus).ToList().Count();

                ApprovalStatus = (int)DocumentApprovalStatus.ApprovalPending;
                var ApprovalPendingLeaves = dbEntities.ApprovalEntries.Where(s => s.SenderId == username && s.Status == ApprovalStatus).ToList().Count();

                var LeaveTypes = dbEntities.LeaveTypes.ToList();
                ViewBag.LeaveTypesCount = LeaveTypes.Count;
                ViewBag.ApprovedLeaves = ApprovedLeaves;
                ViewBag.RejectedLeaves = RejectedLeaves;
                ViewBag.ApprovalPendingLeaves = ApprovalPendingLeaves;



                if (LeaveTypes != null)
                {
                    foreach(var leavetype in LeaveTypes)
                    {
                        var result = dbEntities.EmployeeLedgerEntries.Where(s => s.EmployeeNo == username && s.LeaveType == leavetype.Code && s.Year == CurrentYear).GroupBy(o => o.LeaveType)
                                                                 .Select(g => new { leavetype = g.Key, total = g.Sum(i => i.Quantity) });
                        int TotalDaysTaken = 0;

                        foreach (var group in result)
                        {
                            TotalDaysTaken = Convert.ToInt32(group.total);
                        }

                        int DaysEntitled = Convert.ToInt32(leavetype.TotalAbsence);

                        int LeaveBalance = (DaysEntitled - TotalDaysTaken);

                        int BalCarriedForward = LeavebalanceFromPreviousYear(username, leavetype.Code, DaysEntitled, PreviousYear);

                        LeaveBalance = LeaveBalance + BalCarriedForward;

                        _LeaveBalancesList.Add(new LeaveBalancesView { LeaveType = leavetype.Description, DaysEntitled = DaysEntitled.ToString(), DaysTaken = TotalDaysTaken.ToString(), Balance = LeaveBalance.ToString(), Code = leavetype.Code, BalanceBroughtFoward = BalCarriedForward.ToString() });

                    }
                }

                if (employee.Gender.Trim() == "Male")
                {
                    var itemToRemove = _LeaveBalancesList.Single(r => r.Code == "MATERNITY");
                    _LeaveBalancesList.Remove(itemToRemove);
                }
                else if (employee.Gender.Trim() == "Female")
                {
                    var itemToRemove = _LeaveBalancesList.Single(r => r.Code == "PATERNITY");
                    _LeaveBalancesList.Remove(itemToRemove);
                }
            }
            

            mymodel.LeaveBalancesList = _LeaveBalancesList;
           
            return View(mymodel);
        }
        private int LeavebalanceFromPreviousYear(string username, string Code, int TotalAbsence, string Year)
        {
            int LeaveBalance = 0;
            using (var dbEntities = new LeaveManagementEntities())
            {
                var result = dbEntities.EmployeeLedgerEntries.Where(s => s.EmployeeNo == username && s.LeaveType == Code && s.Year == Year).GroupBy(o => o.LeaveType)
                                                                 .Select(g => new { leavetype = g.Key, total = g.Sum(i => i.Quantity) });
                int TotalDaysTaken = 0;

                foreach (var group in result)
                {
                    TotalDaysTaken = Convert.ToInt32(group.total);
                }

                LeaveBalance = (TotalAbsence - TotalDaysTaken);
            }
            return LeaveBalance;
        }
    }
}