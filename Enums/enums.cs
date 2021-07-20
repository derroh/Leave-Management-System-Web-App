using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources
{
    //Approval status should be mapped to what Dynamics Business Central/NAV have for neat code!
    public enum DocumentApprovalStatus
    {
        Created = 0,
        Open = 1,
        Canceled = 2,
        ApprovalPending = 3,
        Approved = 4,
        Rejected = 5,
    }
}