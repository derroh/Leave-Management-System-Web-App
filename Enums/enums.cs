using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.Enums
{
    //Approval status should be mapped to what Dynamics Business Central/NAV have for neat code!
    enum ApprovalStatus
    {
        Created = 0,
        Open = 1,
        Canceled = 2,
        Approved = 3,
        Rejected = 4,
    }
}