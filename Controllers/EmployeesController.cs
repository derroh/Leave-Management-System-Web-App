using HumanResources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    public class EmployeesController : Controller
    {
        LeaveManagementSystemEntities _db = new LeaveManagementSystemEntities();

        // GET: Employees
        public ActionResult Index()
        {
            return View(from employees in _db.Employees.Where(e => e.Status == 0)
                        select employees);
        }
        public ActionResult Create()
        {
            var departments = _db.Departments.ToList();

            ViewBag.Departments = departments;

            return View();
        }
    }
}