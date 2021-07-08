using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.Models
{
    public class ApprovalEntriesListViewModel
    {
        public int EntryNo { get; set; }
        public string DocumentType { get; set; }
        public string DateSubmitted { get; set; }
        public string DocumentNo { get; set; }
        public string EmployeeName { get; set; }
        public string ApprovedLeaveType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LeaveDays { get; set; }
        public string ApprovalStatus { get; set; }
        //public string Donor { get; set; }
        //public string Project { get; set; }
        //public string MissionSummary { get; set; }
        //public string TotalAmount { get; set; }
    }
}