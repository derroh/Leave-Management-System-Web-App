using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{
    public class LeaveBalancesView
    {
        public string LeaveType { get; set; }
        public string DaysEntitled { get; set; }
        public string DaysTaken { get; set; }
        public string Balance { get; set; }
        public string BalanceBroughtFoward { get; set; }
        public string Code { get; set; }
    }
    public class EmployeeLeaveBalancesView
    {
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public List<LeaveBalancesView> LeaveBalances { get; set; }
    }
}