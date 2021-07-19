using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    [Authorize]
    public class AdvanceSurrendersController : Controller
    {
        // GET: AdvanceSurrenders
        public ActionResult Index()
        {
            return View();
        }
    }
}