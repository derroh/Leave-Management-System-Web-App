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
    [Authorize]
    public class LeaveTypesController : Controller
    {
        LeaveManagementEntities _db = new LeaveManagementEntities();
        // GET: LeaveTypes
        public ActionResult Index()
        {
            return View(from leaveTypes in _db.LeaveTypes
                        select leaveTypes);
        }
        public ActionResult Create()
        {
            List<AnnualLeaveDaysType> _AnnualLeaveDaysType = new List<AnnualLeaveDaysType>();
            _AnnualLeaveDaysType.Add(new AnnualLeaveDaysType { Code = "Working Days", Description = "Working Days" });
            _AnnualLeaveDaysType.Add(new AnnualLeaveDaysType { Code = "Consecutive Days", Description = "Consecutive Days" });

            //
            List<UnitOfMeasure> _UnitOfMeasure = new List<UnitOfMeasure>();
            _UnitOfMeasure.Add(new UnitOfMeasure { Code = "DAY", Description = "Days" });


            ViewBag.UnitofMeasure = _UnitOfMeasure;
            ViewBag.AnnualLeaveDaysType = _AnnualLeaveDaysType;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLeaveType(ViewModels.LeaveTypeViewModel _LeaveTypeViewModel)
        {
            string message = null, status = null;

            try
            {
                var leavetype = new LeaveType { Code = _LeaveTypeViewModel.Code, Description = _LeaveTypeViewModel.Description, TotalAbsence = Convert.ToInt16(_LeaveTypeViewModel.TotalAbsenceDays), AnnualLeaveDaysType = _LeaveTypeViewModel.AnnualLeaveDaysType, UnitOfMeasure = _LeaveTypeViewModel.UnitofMeasure };
                
                using (var dbEntities = new LeaveManagementEntities())
                {
                    dbEntities.Configuration.ValidateOnSaveEnabled = false;
                    dbEntities.LeaveTypes.Add(leavetype);
                    dbEntities.SaveChanges();
                    status = "000";
                    message = "Leave type saved successfully";
                }
            }
            catch(Exception es)
            {
                message = es.Message;
                status = "999";
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
            List<AnnualLeaveDaysType> _AnnualLeaveDaysType = new List<AnnualLeaveDaysType>();
            _AnnualLeaveDaysType.Add(new AnnualLeaveDaysType { Code = "Working Days", Description = "Working Days" });
            _AnnualLeaveDaysType.Add(new AnnualLeaveDaysType { Code = "Consecutive Days", Description = "Consecutive Days" });

            
            List<UnitOfMeasure> _UnitOfMeasure = new List<UnitOfMeasure>();
            _UnitOfMeasure.Add(new UnitOfMeasure { Code = "DAY", Description = "Days" });

            var _LeaveTypeViewModel = new LeaveTypeViewModel();

            try
            {
                using(var db = new LeaveManagementEntities())
                {
                    var leavetype = db.LeaveTypes.Where(x => x.Code == Id).FirstOrDefault();
                    if(leavetype != null)
                    {
                        _LeaveTypeViewModel.OldCode = Id;
                        _LeaveTypeViewModel.Code = leavetype.Code;
                        _LeaveTypeViewModel.TotalAbsenceDays = leavetype.TotalAbsence.ToString();
                        _LeaveTypeViewModel.Description = leavetype.Description;
                        _LeaveTypeViewModel.UnitofMeasure = leavetype.UnitOfMeasure;
                        _LeaveTypeViewModel.AnnualLeaveDaysType = leavetype.AnnualLeaveDaysType;
                    }
                }
            }
            catch(Exception es)
            {
                AppFunctions.WriteLog(es.Message);
            }


            ViewBag.UnitofMeasure = _UnitOfMeasure;
            ViewBag.AnnualLeaveDaysType = _AnnualLeaveDaysType;
            return View(_LeaveTypeViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateLeaveType(ViewModels.LeaveTypeViewModel leavetype)
        {
            string message = null, status = null;

            try
            {               
                using (var dbEntities = new LeaveManagementEntities())
                {
                    var _LeaveType = dbEntities.LeaveTypes.Where(x => x.Code == leavetype.OldCode).FirstOrDefault();

                    if (_LeaveType != null)
                    {
                        _LeaveType.Code = leavetype.Code;
                        _LeaveType.TotalAbsence = Convert.ToInt32(leavetype.TotalAbsenceDays);
                        _LeaveType.Description = leavetype.Description;
                        _LeaveType.UnitOfMeasure = leavetype.UnitofMeasure;
                        _LeaveType.AnnualLeaveDaysType = leavetype.AnnualLeaveDaysType;
                        dbEntities.SaveChanges();

                        status = "000";
                        message = "Leave type updated successfully";
                    }
                }
            }
            catch (Exception es)
            {
                message = es.Message;
                status = "999";
            }
            var _RequestResponse = new RequestResponse
            {
                Message = message,

                Status = status
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string Id)
        {
            List<AnnualLeaveDaysType> _AnnualLeaveDaysType = new List<AnnualLeaveDaysType>();
            _AnnualLeaveDaysType.Add(new AnnualLeaveDaysType { Code = "Working Days", Description = "Working Days" });
            _AnnualLeaveDaysType.Add(new AnnualLeaveDaysType { Code = "Consecutive Days", Description = "Consecutive Days" });

            //
            List<UnitOfMeasure> _UnitOfMeasure = new List<UnitOfMeasure>();
            _UnitOfMeasure.Add(new UnitOfMeasure { Code = "DAY", Description = "Days" });

            var _LeaveTypeViewModel = new LeaveTypeViewModel();

            try
            {
                using (var db = new LeaveManagementEntities())
                {
                    var leavetype = db.LeaveTypes.Where(x => x.Code == Id).FirstOrDefault();
                    if (leavetype != null)
                    {
                        _LeaveTypeViewModel.OldCode = Id;
                        _LeaveTypeViewModel.Code = leavetype.Code;
                        _LeaveTypeViewModel.TotalAbsenceDays = leavetype.TotalAbsence.ToString();
                        _LeaveTypeViewModel.Description = leavetype.Description;
                        _LeaveTypeViewModel.UnitofMeasure = leavetype.UnitOfMeasure;
                        _LeaveTypeViewModel.AnnualLeaveDaysType = leavetype.AnnualLeaveDaysType;
                    }
                }
            }
            catch (Exception es)
            {
                AppFunctions.WriteLog(es.Message);
            }


            ViewBag.UnitofMeasure = _UnitOfMeasure;
            ViewBag.AnnualLeaveDaysType = _AnnualLeaveDaysType;
            return View();
        }
        [HttpPost]
        public ActionResult Delete(string Code)
        {
            string status = "", message = "";

            try
            {
                using (var db = new LeaveManagementEntities())
                {
                    var leavetype = db.LeaveTypes.Where(x => x.Code == Code).SingleOrDefault();

                    if (leavetype != null)
                    {
                        db.LeaveTypes.Remove(leavetype);
                        db.SaveChanges();
                        status = "000";
                        message = "Leave type " + Code + " has been successfully deleted";
                    }
                    else
                    {
                        status = "900";
                        message = "Couldn't find leave type " + Code;
                    }
                }
            }
            catch (Exception es)
            {
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