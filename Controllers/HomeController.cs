using HumanResources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    public class HomeController : Controller
    {
        LeaveManagementEntities _db = new LeaveManagementEntities();
        public ActionResult Index()
        {
            return View(from leaveTypes in _db.LeaveTypes
                        select leaveTypes);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}