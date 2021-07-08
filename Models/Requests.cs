using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.Models
{
    public class RequestResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
    }

    public class RequestLeaveTypeDetailsResponse
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int LeaveDaysEntitled { get; set; }
        public int OpeningBalance { get; set; }
        public int LeaveDaysTaken { get; set; }
        public int AccruedDays { get; set; }
        public int RemainingDays { get; set; }
        public bool RequiresAttachments { get; set; }
        public bool IsAttachmentMandatory { get; set; }
    }

    public class ResponseLeaveQuantityAndReturnDate
    {
        public string Code { get; set; }
        public int LeaveDaysApplied { get; set; }
        public string ReturnDate { get; set; }
        public string LeaveEndDate { get; set; }
    }


}