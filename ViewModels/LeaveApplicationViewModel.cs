using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{
    public class LeaveApplicationViewModel
    {
        public string DocumentNo { get; set; }
        public string LeaveType { get; set; }
        public string LeaveDaysEntitled { get; set; }
        public string LeaveDaysTaken { get; set; }
        public string LeaveBalance { get; set; }
        public string LeaveAccruedDays { get; set; }
        public string LeaveOpeningBalance { get; set; }

        //selection
        public string SelectionType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LeaveDates { get; set; }
        public string LeaveDaysApplied { get; set; }
        public string LeaveStartDate { get; set; }
        public string LeaveEndDate { get; set; }
        public string ReturnDate { get; set; }
        public string LeaveComment { get; set; }
        public string LeaveAttachments { get; set; }
    }
}