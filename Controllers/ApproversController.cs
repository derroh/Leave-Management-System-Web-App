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
    public class ApproversController : Controller
    {
        LeaveManagementEntities _db = new LeaveManagementEntities();
        // GET: Approvers

        public ActionResult Index()
        {
            return View(from approvers in _db.ApprovalUsers
                        select approvers);
        }
        public ActionResult EditApprover(int Id)
        {
            ViewModels.CreateApproverViewModel _CreateApproverViewModel = new ViewModels.CreateApproverViewModel();
            var Employees = _db.Employees.ToList();
            var ApprovalDDocumentTypes = _db.ApprovalDocumentTypes.ToList();

            ViewBag.Employees = Employees;
            ViewBag.ApprovalDDocumentTypes = ApprovalDDocumentTypes;

            var approver = _db.ApprovalUsers.Where(x => x.Id == Id).FirstOrDefault();

            if (approver != null)
            {
                _CreateApproverViewModel.Id = approver.Id;
                _CreateApproverViewModel.DocumentType = approver.DocumentType;
                _CreateApproverViewModel.Approver = approver.Approver;
                _CreateApproverViewModel.ApprovalSequence = approver.ApprovalSequence;
                _CreateApproverViewModel.ApproverEmail = approver.ApproverEmail;
                _CreateApproverViewModel.SubstituteApprover = approver.SubstituteApprover;
                _CreateApproverViewModel.SubstituteApproverEmail = approver.SubstituteApproverEmail;
            }

            return View(_CreateApproverViewModel);
        }
        public ActionResult DeleteApprover(string id)
        {
            string status = "", message = "";

            int Id = Convert.ToInt32(id);

            try
            {
                using (var db = new LeaveManagementEntities())
                {
                    var approver = db.ApprovalUsers.Where(d => d.Id == Id).FirstOrDefault();

                    if (approver != null)
                    {
                        db.ApprovalUsers.Remove(approver);
                        db.SaveChanges();

                        status = "000";
                        message = "Delete for approver " + approver.Approver + " successful";
                    }
                    else
                    {
                        status = "900";
                        message = "Approver not found";
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
        public ActionResult CreateApprover()
        {
            ViewModels.CreateApproverViewModel approver = new ViewModels.CreateApproverViewModel();
            var Employees = _db.Employees.ToList();
            var ApprovalDDocumentTypes = _db.ApprovalDocumentTypes.ToList();

            ViewBag.Employees = Employees;
            ViewBag.ApprovalDDocumentTypes = ApprovalDDocumentTypes;

            return View(approver);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateApprovalUser(ViewModels.CreateApproverViewModel app)
        {
            string message = "", status = "";

            try
            {
                var approvaluser = new ApprovalUser
                {
                    ApprovalSequence = app.ApprovalSequence,
                    Approver = app.Approver,
                    ApproverEmail = app.ApproverEmail,
                    DocumentType = app.DocumentType,
                    SubstituteApprover = app.SubstituteApprover,
                    SubstituteApproverEmail = app.SubstituteApproverEmail
                };

                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                {
                    dbEntities.Configuration.ValidateOnSaveEnabled = false;
                    dbEntities.ApprovalUsers.Add(approvaluser);
                    dbEntities.SaveChanges();

                    status = "000";
                    message = "Approval user saved successfully";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateApprovalUser(ViewModels.CreateApproverViewModel app)
        {
            string message = "", status = "";

            try
            {
                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                {
                    var approver = dbEntities.ApprovalUsers.Where(x => x.Id == app.Id).FirstOrDefault();

                    if (approver != null)
                    {

                        approver.Id = app.Id;
                        approver.DocumentType = app.DocumentType;
                        approver.Approver = app.Approver;
                        approver.ApprovalSequence = app.ApprovalSequence;
                        approver.ApproverEmail = app.ApproverEmail;
                        approver.SubstituteApprover = app.SubstituteApprover;
                        approver.SubstituteApproverEmail = app.SubstituteApproverEmail;
                        dbEntities.SaveChanges();
                        status = "000";
                        message = "Approval user changes saved successfully";
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

    }
}