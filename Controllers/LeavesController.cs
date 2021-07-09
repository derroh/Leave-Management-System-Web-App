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
        public ActionResult Index()
        {
            List<LeavesListViewModel> _LeavesListViewModel = new List<LeavesListViewModel>();

            for(int i=1; i <=10; i++)
            {
                _LeavesListViewModel.Add(new LeavesListViewModel { DocumentNo ="LEAVE00"+1, EmployeeName = "Derrick Witness Abucheri", ApprovalStatus = "open", DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()) , EndDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()) , StartDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()) ,LeaveDays = i.ToString(),DocumentType = "Leave", LeaveType = "Annual Leave", ApprovalProgress = i*10 });
            }
            return View(_LeavesListViewModel);
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

                //if Male add Paternity. If female add maternity

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
        public JsonResult LeaveQuantityAndReturnDate(string Code, string StartDate, string EndDate)
        {
            using (LeaveManagementSystemEntities dbEntities = new LeaveManagementSystemEntities())
            {              
                var leaveType = dbEntities.LeaveTypes.Where(s => s.Code == Code).SingleOrDefault();

                string AnnualLeaveDaysType = leaveType.AnnualLeaveDaysType;

                if(AnnualLeaveDaysType .Trim() == "Consecutive Days")
                {

                }
                else if (AnnualLeaveDaysType.Trim() == "Working Days")
                {
                    var dtResult = DateTimeExtensions.AddBusinessDays(Convert.ToDateTime(StartDate), 10);
                }

            }

            var _ResponseLeaveQuantityAndReturnDate = new ResponseLeaveQuantityAndReturnDate
            {
                Code = Code,
                LeaveDaysApplied = 1,
                ReturnDate = "2020/01/01"
            };
            return Json(JsonConvert.SerializeObject(_ResponseLeaveQuantityAndReturnDate), JsonRequestBehavior.AllowGet);
        }
        public JsonResult LeaveEndDateAndReturnDate(string Code, string StartDate, string LeaveDaysApplied)
        {
            string EndDate = null;

            using (LeaveManagementSystemEntities dbEntities = new LeaveManagementSystemEntities())
            {
                var leaveType = dbEntities.LeaveTypes.Where(s => s.Code == Code).SingleOrDefault();

                string AnnualLeaveDaysType = leaveType.AnnualLeaveDaysType;

                if (AnnualLeaveDaysType.Trim() == "Consecutive Days")
                {
                    var dtResult = DateTimeExtensions.AddBusinessDays(Convert.ToDateTime(StartDate), Convert.ToInt32(LeaveDaysApplied));
                    EndDate = dtResult.ToString("MM/dd/yyyy");
                }
                else if (AnnualLeaveDaysType.Trim() == "Working Days")
                {
                    var dtResult = DateTimeExtensions.AddBusinessDays(Convert.ToDateTime(StartDate), Convert.ToInt32(LeaveDaysApplied));
                    EndDate = dtResult.ToString("MM/dd/yyyy");
                }

            }

            var _ResponseLeaveQuantityAndReturnDate = new ResponseLeaveQuantityAndReturnDate
            {
                Code = Code,
                LeaveDaysApplied = 1,
                LeaveEndDate = EndDate,
                ReturnDate = EndDate
            };
            return Json(JsonConvert.SerializeObject(_ResponseLeaveQuantityAndReturnDate), JsonRequestBehavior.AllowGet);
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

                UploadDocuments(path, file);
            }
            msg = files.Count + " Files Uploaded!";

            var _RequestResponse = new RequestResponse
            {
                Message = msg,

                Status = "000"
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult DownloadFile(int? fileId)
        {
            LeaveManagementSystemEntities entities = new LeaveManagementSystemEntities();
            var file = entities.Attachments.ToList().Find(p => p.Id == fileId.Value);
            return File(file.Data, file.ContentType, file.FileName);
        }

        private void UploadDocuments(string path, HttpPostedFileBase file)
        {
            using (LeaveManagementSystemEntities dbEntities = new LeaveManagementSystemEntities())
            {
                byte[] bytes;

                using (BinaryReader br = new BinaryReader(file.InputStream))
                {
                    bytes = br.ReadBytes(file.ContentLength);
                }

                var Attachment = new Attachment { FileName = Path.GetFileName(file.FileName) ,ContentType = file.ContentType, Data = bytes, DateUploaded = DateTime.Now, DocumentNo ="DOC1"  };

                dbEntities.Configuration.ValidateOnSaveEnabled = false;
                dbEntities.Attachments.Add(Attachment);
                dbEntities.SaveChanges();

            }
        }
        // Leave Entry Type -> Opening Balance",Accrue,Deduct,Use,Closing,Recall

        //Approal stuff
        public ActionResult Submit(string DocumentNo)
        {
            var _RequestResponse = new RequestResponse
            {
                Status = "900",
                Message = "Submit Success! for leave " + DocumentNo
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Cancel(string DocumentNo)
        {
            var _RequestResponse = new RequestResponse
            {
                Status = "900",
                Message = "Cancel Success! for leave " + DocumentNo
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
    }
}