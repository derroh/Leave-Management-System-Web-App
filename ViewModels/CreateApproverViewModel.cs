using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{
    public class CreateApproverViewModel
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string Approver { get; set; }
        public int ApprovalSequence { get; set; }
        public string ApproverEmail { get; set; }
        public string SubstituteApprover { get; set; }
        public string SubstituteApproverEmail { get; set; }
    }
}