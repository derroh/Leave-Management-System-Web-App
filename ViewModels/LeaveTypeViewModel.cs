using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{ 
    public class LeaveTypeViewModel
    {
        public string Code { get; set; }
        public string OldCode { get; set; }
        public string Description { get; set; }
        public string TotalAbsenceDays { get; set; }
        public string UnitofMeasure { get; set; }
        public string AnnualLeaveDaysType { get; set; }
    }
}