using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdvanceClaimsController : Controller
    {
        // GET: AdvanceClaims
        public ActionResult Index()
        {
            return View();
        }
    }
}