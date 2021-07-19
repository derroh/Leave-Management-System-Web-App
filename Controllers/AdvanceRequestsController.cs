using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    [Authorize]
    public class AdvanceRequestsController : Controller
    {
        // GET: AdvanceRequests
        public ActionResult Index()
        {
            return View();
        }
    }
}