using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    public class LeaveRecallsController : Controller
    {        
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Index(string status)
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}