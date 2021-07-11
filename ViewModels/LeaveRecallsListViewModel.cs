using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{
    public class LeaveRecallsListViewModel
    {
        public string DocumentType { get; set; }
        public string DateSubmitted { get; set; }
        public string DocumentNo { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveTypeRecalled { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LeaveDaysRecalled { get; set; }
        public string ApprovalStatus { get; set; }
        public int ApprovalProgress { get; set; }
    }
}