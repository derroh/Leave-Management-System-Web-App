using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    public class LeavesController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult List(string status)
        {
            string s = Request.QueryString["page"];
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}