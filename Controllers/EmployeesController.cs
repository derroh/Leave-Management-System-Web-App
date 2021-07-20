using HumanResources.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        LeaveManagementEntities _db = new LeaveManagementEntities();

        // GET: Employees
        public ActionResult Index()
        {
            return View(from employees in _db.Employees.Where(e => e.Status == 1)
                        select employees);
        }
        public ActionResult Create()
        {
            var departments = _db.Departments.ToList();

            ViewBag.Departments = departments;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEmployee(ViewModels.CreateEmployeeViewModel ep)
        {
            string message = "", DocumentNo = "", status = "";

            //create Leave Header here
            LeaveManagementEntities _db = new LeaveManagementEntities();

            try
            {
                var settings = _db.Settings.Where(s => s.Id == 1).SingleOrDefault();

                if (settings != null)
                {
                    string EmployeeCode = settings.EmployeeNumbers;

                    var NumberSeriesData = _db.NumberSeries.Where(s => s.Code == EmployeeCode).SingleOrDefault();

                    string LastUsedNumber = NumberSeriesData.LastUsedNumber;

                    if (LastUsedNumber != "")
                    {
                        DocumentNo = AppFunctions.GetNewDocumentNumber(EmployeeCode.Trim(), LastUsedNumber.Trim());

                      //  LeaveDocumentNo = DocumentNo;
                    }

                    var employee = new Employee
                    {
                        EmployeeNo = DocumentNo,
                        FirstName = ep.FirstName,
                        LastName = ep.LastName,
                        FullName = ep.Name,
                        CellularPhoneNumber = ep.Phone,
                        Gender = ep.Gender,                        
                        EMail = ep.Email,
                        JobTitle = "Dev",
                        Date_Of_Joining_the_Company = DateTime.Now,
                        Department_Name = ep.DepartmentName,
                        AnnualLeaveDaysEntitled = 28,
                        CurrentYear = DateTime.Now.Year,
                        Status = 1

                    };

                    using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                    {
                        dbEntities.Configuration.ValidateOnSaveEnabled = false;
                        dbEntities.Employees.Add(employee);
                        dbEntities.SaveChanges();
                        status = "000";
                        message = "Employee saved successfully";
                    }

                    //update last used number
                    AppFunctions.UpdateNumberSeries(EmployeeCode, DocumentNo);
                }
                else
                {
                    message = "Looks like leave numbers have not been set up";
                    status = "900";
                }
            }
            catch (Exception es)
            {
                message = es.Message;
                status = "900";
            }

            var _RequestResponse = new RequestResponse
            {
                Message = message,

                Status = status
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string Id)
        {
            var departments = _db.Departments.ToList();

            ViewBag.Departments = departments;

            ViewModels.CreateEmployeeViewModel ep = new ViewModels.CreateEmployeeViewModel();

            var employee = _db.Employees.Where(e => e.EmployeeNo == Id).FirstOrDefault();

            if (employee != null)
            {
                ep.DepartmentName = employee.Department_Name;
                ep.Email = employee.EMail;
                ep.EmployeeNo = employee.EmployeeNo;
                ep.FirstName = employee.FirstName;
                ep.LastName = employee.LastName;
                ep.Name = employee.FullName;
                ep.Gender = employee.Gender;
                ep.JobTitle = employee.JobTitle;
                ep.Phone = employee.CellularPhoneNumber;
            }                

            return View(ep);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEmployee(ViewModels.CreateEmployeeViewModel ep)
        {
            string message = "", status = "";
            try
            {
                using (var _db = new LeaveManagementEntities())
                {
                    var employee = _db.Employees.Where(s => s.EmployeeNo == ep.EmployeeNo).SingleOrDefault();

                    if (employee != null)
                    {
                        employee.FirstName = ep.FirstName;
                        employee.LastName = ep.LastName;
                        employee.FullName = ep.Name;
                        employee.CellularPhoneNumber = ep.Phone;
                        employee.Gender = ep.Gender;
                        employee.EMail = ep.Email;
                        employee.JobTitle = "Dev";
                        employee.Date_Of_Joining_the_Company = DateTime.Now;
                        employee.Department_Name = ep.DepartmentName;
                        employee.AnnualLeaveDaysEntitled = 28;
                        employee.CurrentYear = DateTime.Now.Year;
                        employee.Status = 1;

                        _db.SaveChanges();

                        status = "000";
                        message = "Employee updated successfully";

                    }
                    else
                    {
                        message = "Employee was not found";
                        status = "900";
                    }
                }
                   
            }
            catch (Exception es)
            {
                message = es.Message;
                status = "900";
            }

            var _RequestResponse = new RequestResponse
            {
                Message = message,
                Status = status
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
    }
}