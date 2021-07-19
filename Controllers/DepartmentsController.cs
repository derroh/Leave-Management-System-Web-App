using HumanResources.Models;
using HumanResources.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    public class DepartmentsController : Controller
    {
        private static HumanResourcesManagementSystemEntities _db = new HumanResourcesManagementSystemEntities();
        // GET: Departments
        public ActionResult Index()
        {
            List<DepartmentsViewModel> _DepartmentsViewModel = new List<DepartmentsViewModel>();

            var departments = _db.Departments.ToList();

            foreach(var department in departments)
            {
                _DepartmentsViewModel.Add(new DepartmentsViewModel { Code = department.Code, Name = department.Description });
            }

            return View(_DepartmentsViewModel);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDepartment(DepartmentsViewModel deparment)
        {
            string DocumentNo = "", status = "", message = "";

            try
            {
                using (var db = new HumanResourcesManagementSystemEntities())
                {
                    //Generate Department number here
                    var settings = db.Settings.Where(s => s.Id == 1).SingleOrDefault();

                    if(settings != null)
                    {
                        string DepartmentsCode = settings.DepartmentNumbers;

                        var NumberSeriesData = _db.NumberSeries.Where(s => s.Code == DepartmentsCode).SingleOrDefault();

                        if(NumberSeriesData != null)
                        {
                            string LastUsedNumber = NumberSeriesData.LastUsedNumber;

                            if (LastUsedNumber != "")
                            {
                                DocumentNo = AppFunctions.GetNewDocumentNumber(DepartmentsCode.Trim(), LastUsedNumber.Trim());
                            }
                            var dept = new Department
                            {
                                Code = DocumentNo,
                                Description = deparment.Name
                            };

                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.Departments.Add(dept);
                            db.SaveChanges();

                            AppFunctions.UpdateNumberSeries(DepartmentsCode, DocumentNo);

                            status = "000";
                            message = "Department created successfully";
                        }
                        else
                        {
                            status = "900";
                            message = "number series have not beein properly set up";
                        }

                    }
                    else
                    {
                        status = "900";
                        message = "App settings not found";
                    }
                }
            }
            catch (Exception es)
            {
                status = "900";
                message = es.Message;
            }
            var _RequestResponse = new RequestResponse
            {
                Status = status,
                Message = message
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewDepartment(string Id)
        {
            if (string.IsNullOrEmpty(Id))

                return HttpNotFound();
            var dept = _db.Departments.Where(d => d.Code == Id).FirstOrDefault();

            var _DepartmentsViewModel = new DepartmentsViewModel
            {
                Code = dept.Code,
                Name = dept.Description
            };

            return View(_DepartmentsViewModel);
        }
        public ActionResult Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))

                return HttpNotFound();

            var dept = _db.Departments.Where(d => d.Code == Id).FirstOrDefault();

            var _DepartmentsViewModel = new DepartmentsViewModel
            {
                Code = dept.Code,
                Name = dept.Description
            };

            return View(_DepartmentsViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDepartment(DepartmentsViewModel deparment)
        {
            string status = "", message = "";

            try
            {
                using (var db = new HumanResourcesManagementSystemEntities())
                {                   

                    var dept = _db.Departments.Where(s => s.Code == deparment.Code).SingleOrDefault();

                    if (dept != null)
                    {
                        dept.Description = deparment.Name;
                        db.SaveChanges();
                        status = "000";
                        message = "Department updated successfully";
                    }
                    else
                    {
                        status = "900";
                        message = "Department not found";
                    }                   
                }
            }
            catch (Exception es)
            {
                status = "900";
                message = es.Message;
            }
            var _RequestResponse = new RequestResponse
            {
                Status = status,
                Message = message
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(string DocumentNo)
        {
            string status = "", message = "";

            try
            {
                using (var db = new HumanResourcesManagementSystemEntities())
                {
                    var department = db.Departments.Where(d => d.Code == DocumentNo).FirstOrDefault();

                    if (department != null)
                    {
                        db.Departments.Remove(department);
                        db.SaveChanges();

                        status = "000";
                        message = "Delete for department " + DocumentNo + " successful";
                    }
                    else
                    {
                        status = "900";
                        message = "Department not found";
                    }
                }
            }
            catch(Exception es)
            {
                status = "900";
                message = es.Message;
            }
            var _RequestResponse = new RequestResponse
            {
                Status = status,
                Message = message
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
    }
}