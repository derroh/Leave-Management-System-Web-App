using HumanResources.Models;
using HumanResources.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.IO;
//using RotativaPDF.Models;
using OfficeOpenXml.Table;

namespace HumanResources.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        
        // GET: Dashboard
        public ActionResult Index()
        {
            string username = HttpContext.Session["EmployeeNo"].ToString();

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
            }


            //for creating Excel to export


            dynamic mymodel = new ExpandoObject();   

            mymodel.LeaveBalancesList = GetLeaveBalancesList();
            return View(mymodel);
        }

        private dynamic GetLeaveBalancesList()
        {
            string username = HttpContext.Session["EmployeeNo"].ToString();
            string CurrentYear = DateTime.Now.Year.ToString();
            string PreviousYear = (DateTime.Now.Year - 1).ToString();


            List<LeaveBalancesView> _LeaveBalancesList = new List<LeaveBalancesView>();

            using (var dbEntities = new LeaveManagementEntities())
            {
                var employee = dbEntities.Employees.Where(x => x.EmployeeNo == username).FirstOrDefault();

                var LeaveTypes = dbEntities.LeaveTypes.ToList();

                if (LeaveTypes != null)
                {
                    foreach (var leavetype in LeaveTypes)
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

            return _LeaveBalancesList;
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
                

        public ActionResult Download()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //create Datatable

            DataTable Dt = new DataTable();
            Dt.Columns.Add("LeaveCode", typeof(string));
            Dt.Columns.Add("LeaveType", typeof(string));
            Dt.Columns.Add("DaysEntitled", typeof(string));
            Dt.Columns.Add("BalanceBroughtFoward", typeof(string));
            Dt.Columns.Add("DaysTaken", typeof(string));
            Dt.Columns.Add("Balance", typeof(string));

            foreach (var leave in GetLeaveBalancesList())
            {
                DataRow row = Dt.NewRow();
                row[0] = leave.Code;
                row[1] = leave.LeaveType;
                row[2] = leave.DaysEntitled.ToString();
                row[3] = leave.BalanceBroughtFoward.ToString();
                row[4] = leave.DaysTaken.ToString();
                row[5] = leave.Balance.ToString();
                Dt.Rows.Add(row);
            }

            var memoryStream = new MemoryStream();

            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                if (Dt.Rows.Count > 0)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(Dt, true, TableStyles.None);
                    worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                    worksheet.DefaultRowHeight = 18;


                    worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.DefaultColWidth = 20;
                    worksheet.Column(2).AutoFit();

                    byte[] data = excelPackage.GetAsByteArray();
                    return File(data, "application/octet-stream", "Leavebalances.xlsx");
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
    }
}