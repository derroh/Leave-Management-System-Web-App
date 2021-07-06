using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    public class ApprovalEntriesController : Controller
    {
        // GET: ApprovalEntries
        public ActionResult Index()
        {
            return View();
        }
    }
}