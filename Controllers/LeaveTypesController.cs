using HumanResources.Models;
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
    }
}