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

            List<LeaveBalancesView> _LeaveBalancesList = new List<LeaveBalancesView>();

            dynamic mymodel = new ExpandoObject();

            //get leave balances for each of the leave types

            //get number of days entilted

            using (var dbEntities = new LeaveManagementEntities())
            {
                int ApprovalStatus = (int)DocumentApprovalStatus.Approved;
                var ApprovedLeaves = dbEntities.ApprovalEntries.Where(s => s.SenderId == username && s.Status == ApprovalStatus).ToList().Count();

                ApprovalStatus = (int)DocumentApprovalStatus.Rejected;
                var RejectedLeaves = dbEntities.ApprovalEntries.Where(s => s.SenderId == username && s.Status == ApprovalStatus).ToList().Count();

                ApprovalStatus = (int)DocumentApprovalStatus.ApprovalPending;
                var ApprovalPendingLeaves = dbEntities.ApprovalEntries.Where(s => s.SenderId == username && s.Status == ApprovalStatus).ToList().Count();

                var LeaveTypes = dbEntities.LeaveTypes.ToList();
                mymodel.LeaveTypesCount = LeaveTypes.Count;
                mymodel.ApprovedLeaves = ApprovedLeaves;
                mymodel.RejectedLeaves = RejectedLeaves;
                mymodel.ApprovalPendingLeaves = ApprovalPendingLeaves;



                if (LeaveTypes != null)
                {
                    foreach(var leavetype in LeaveTypes)
                    {
                        var result = dbEntities.EmployeeLedgerEntries.Where(s => s.EmployeeNo == username && s.LeaveType == leavetype.Code).GroupBy(o => o.LeaveType)
                                                                 .Select(g => new { leavetype = g.Key, total = g.Sum(i => i.Quantity) });
                        int TotalDaysTaken = 0;

                        foreach (var group in result)
                        {
                            TotalDaysTaken = Convert.ToInt32(group.total);
                        }

                        int DaysEntitled = Convert.ToInt32(leavetype.TotalAbsence);

                        int LeaveBalance = (DaysEntitled - TotalDaysTaken);

                        _LeaveBalancesList.Add(new LeaveBalancesView { LeaveType = leavetype.Description, DaysEntitled = DaysEntitled.ToString(), DaysTaken = TotalDaysTaken.ToString(), Balance = LeaveBalance.ToString() });

                    }
                }
            }
           
            mymodel.LeaveBalancesList = _LeaveBalancesList;
           
            return View(mymodel);
        }
    }
}