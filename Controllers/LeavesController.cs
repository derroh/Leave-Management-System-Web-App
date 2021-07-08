using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    using HumanResources.Models;
    using Newtonsoft.Json;
    using System.IO;

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
        //Get LeaveTypes
        public JsonResult ListLeaveTypes()
        {
            List<LeaveType> leavetypelist = new List<LeaveType>();

            using (LeaveManagementSystemEntities dbEntities = new LeaveManagementSystemEntities())
            {
                var leavetypes = dbEntities.LeaveTypes.ToList();

                foreach (var leavetype in leavetypes)
                {
                    leavetypelist.Add(new LeaveType
                    {
                        Code = leavetype.Code,
                        Description = leavetype.Description
                    });
                }
            }
            return Json(JsonConvert.SerializeObject(leavetypelist), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LeaveTypeDetails(string Code)
        {
            List<LeaveType> leavetypelist = new List<LeaveType>();

            using (LeaveManagementSystemEntities dbEntities = new LeaveManagementSystemEntities())
            {
                var leavetypes = dbEntities.LeaveTypes.ToList();

                foreach (var leavetype in leavetypes)
                {
                    leavetypelist.Add(new LeaveType
                    {
                        Code = leavetype.Code,
                        Description = leavetype.Description
                    });
                }
            }

            var _RequestLeaveTypeDetailsResponse = new RequestLeaveTypeDetailsResponse
            {
                Code = Code,
                Description = "Test",
                AccruedDays = 1,
                LeaveDaysEntitled = 10,
                LeaveDaysTaken = 10,
                OpeningBalance = 10,
                RemainingDays = 10,
                IsAttachmentMandatory = false,
                RequiresAttachments = false
            };
            return Json(JsonConvert.SerializeObject(_RequestLeaveTypeDetailsResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveSelection(LeaveApplicationViewModel ep)
        {
            string msg = "";;

            try
            {
                msg = ep.LeaveType;
            }
            catch (Exception es)
            {
                msg = es.Message;
            }

            var _RequestResponse = new RequestResponse
            {
                Message = msg,

                Status = "000"
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveLeaveSelection(LeaveApplicationViewModel ep)
        {
            string msg = ""; ;

            try
            {
                msg = ep.SelectionType;
            }
            catch (Exception es)
            {
                msg = es.Message;
            }

            var _RequestResponse = new RequestResponse
            {
                Message = msg,

                Status = "000"
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveLeaveAttachment(LeaveApplicationViewModel ep)
        {
            string msg = ""; ;

            try
            {
                msg = ep.SelectionType;
            }
            catch (Exception es)
            {
                msg = es.Message;
            }

            var _RequestResponse = new RequestResponse
            {
                Message = msg,

                Status = "000"
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UploadFiles()
        {
            string msg = null;

            string path = Server.MapPath("~/UploadedFiles/");
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                file.SaveAs(path + file.FileName);
            }
            msg = files.Count + " Files Uploaded!";

            var _RequestResponse = new RequestResponse
            {
                Message = msg,

                Status = "000"
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        // Leave Entry Type -> Opening Balance",Accrue,Deduct,Use,Closing,Recall
    }
}