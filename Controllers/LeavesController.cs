using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    using HumanResources.Models;
    using HumanResources.ViewModels;
    using Newtonsoft.Json;
    using System.IO;
    using HumanResources.CustomFunctions;

    [Authorize]
    public class LeavesController : Controller
    {
        private static HumanResourcesManagementSystemEntities _db = new HumanResourcesManagementSystemEntities();
        static string LeaveDocumentNo = null;
        public ActionResult Index()
        {
            string status = Request.QueryString["status"];
            string SenderId = HttpContext.Session["EmployeeNo"].ToString();

            List<LeavesListViewModel> _LeavesListViewModel = new List<LeavesListViewModel>();

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "pending") status = "Pending Approval";
                var leaves = _db.Leaves.Where(a => a.ApprovalStatus == status && a.EmployeeNo == SenderId).ToList();

                foreach (var leave in leaves)
                {
                    _LeavesListViewModel.Add(new LeavesListViewModel { DocumentNo = leave.DocumentNo, EmployeeName = "Derrick Witness Abucheri", ApprovalStatus = status, DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()), EndDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), StartDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), LeaveDays = leave.LeaveDaysApplied.ToString(), DocumentType = "Leave", LeaveType = "Annual Leave", ApprovalProgress = GetApprovalProgress(leave.DocumentNo) });
                }
            }
            return View(_LeavesListViewModel);
        }
        [HttpGet]
        public ActionResult List(string status)
        {
            //  string s = Request.QueryString["status"];
            List<LeavesListViewModel> _LeavesListViewModel = new List<LeavesListViewModel>();
            string SenderId = HttpContext.Session["EmployeeNo"].ToString();

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "pending") status = "Pending Approval";
                var leaves = _db.Leaves.Where(a => a.ApprovalStatus == status && a.EmployeeNo == SenderId).ToList();

                foreach (var leave in leaves)
                {
                    _LeavesListViewModel.Add(new LeavesListViewModel { DocumentNo = leave.DocumentNo, EmployeeName = "Derrick Witness Abucheri", ApprovalStatus = status, DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()), EndDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), StartDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), LeaveDays = leave.LeaveDaysApplied.ToString(), DocumentType = "Leave", LeaveType = "Annual Leave", ApprovalProgress = GetApprovalProgress(leave.DocumentNo) });
                }
            }    
            
            return View(_LeavesListViewModel);
        }
       
        public ActionResult Create()
        {
            var leavetypes = _db.LeaveTypes.ToList();
            ViewBag.LeaveTypes = leavetypes;

            return View();
        }
        //Get LeaveTypes
        public JsonResult ListLeaveTypes()
        {
            List<LeaveType> leavetypelist = new List<LeaveType>();

            using (HumanResourcesManagementSystemEntities dbEntities = new HumanResourcesManagementSystemEntities())
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

            using (HumanResourcesManagementSystemEntities dbEntities = new HumanResourcesManagementSystemEntities())
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
            using (HumanResourcesManagementSystemEntities dbEntities = new HumanResourcesManagementSystemEntities())
            {              
                var leaveType = dbEntities.LeaveTypes.Where(s => s.Code == Code).SingleOrDefault();
                //

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
            return Json(GetLeaveEndDateAndReturnDate(Code, StartDate, LeaveDaysApplied), JsonRequestBehavior.AllowGet);
        }

        private string GetLeaveEndDateAndReturnDate(string Code, string StartDate, string LeaveDaysApplied)
        {
            string EndDate = null;

            using (HumanResourcesManagementSystemEntities dbEntities = new HumanResourcesManagementSystemEntities())
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

            return JsonConvert.SerializeObject(_ResponseLeaveQuantityAndReturnDate);
        }

        public ActionResult SaveSelection(LeaveApplicationViewModel ep)
        {
            string message = "", DocumentNo = "", status = "";

            //create Leave Header here
            HumanResourcesManagementSystemEntities _db = new HumanResourcesManagementSystemEntities();

            try
            {
                var settings = _db.Settings.Where(s => s.Id == 1).SingleOrDefault();

                if(settings != null)
                {
                    string LeaveCode = settings.LeaveNumbers;

                    var NumberSeriesData = _db.NumberSeries.Where(s => s.Code == LeaveCode).SingleOrDefault();

                    string LastUsedNumber = NumberSeriesData.LastUsedNumber;

                    if (LastUsedNumber != "")
                    {
                        DocumentNo = AppFunctions.GetNewDocumentNumber(LeaveCode.Trim(), LastUsedNumber.Trim());

                        LeaveDocumentNo = DocumentNo;
                    }

                    var leave = new Leaf
                    {
                        DocumentNo = DocumentNo,
                        LeaveType = ep.LeaveType,
                      //  EmployeeNo = "Derrick",
                        ApprovalStatus = "Open"
                    };

                    using (HumanResourcesManagementSystemEntities dbEntities = new HumanResourcesManagementSystemEntities())
                    {
                        dbEntities.Configuration.ValidateOnSaveEnabled = false;
                        dbEntities.Leaves.Add(leave);
                        dbEntities.SaveChanges();
                        status = "000";
                        message = "Leave type saved successfully";
                    }

                    //update last used number
                    AppFunctions.UpdateNumberSeries(LeaveCode, DocumentNo);
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
        public ActionResult SaveLeaveSelection(LeaveApplicationViewModel ep)
        {
            string message = "", status = "";

            try
            {             
                using (HumanResourcesManagementSystemEntities dbEntities = new HumanResourcesManagementSystemEntities())
                {
                    var leave = dbEntities.Leaves.Where(s => s.DocumentNo == LeaveDocumentNo).SingleOrDefault();

                    if (leave != null)
                    {
                        string response = GetLeaveEndDateAndReturnDate(leave.LeaveType, ep.StartDate, ep.LeaveDaysApplied);

                        ResponseLeaveQuantityAndReturnDate leavedates = JsonConvert.DeserializeObject<ResponseLeaveQuantityAndReturnDate>(response);

                        leave.SelectionType = "RangeSelection";
                        leave.StartDate = ep.StartDate;
                        leave.ReturnDate = Convert.ToDateTime(leavedates.ReturnDate);
                        leave.EndDate =Convert.ToDateTime(leavedates.LeaveEndDate);
                        leave.LeaveDaysApplied = Convert.ToInt32(ep.LeaveDaysApplied);
                       // leave.LeaveDates = GetListOfDates(Convert.ToDateTime(ep.StartDate), Convert.ToDateTime(ep.EndDate));
                        dbEntities.SaveChanges();

                        message = "Leave Created successfully";
                        status = "000";
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
        [HttpPost]
        public ActionResult SaveLeaveAttachments()
        {
            string msg = null;

            try
            {
                string path = Server.MapPath("~/UploadedFiles/");

                HttpFileCollectionBase files = Request.Files;

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    file.SaveAs(path + file.FileName);

                    UploadDocuments(path, file);
                }
                msg = files.Count + " Files Uploaded!";
            }
            catch(Exception es)
            {
                AppFunctions.WriteLog(es.Message);

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
        public FileResult DownloadFile(int? fileId)
        {
            HumanResourcesManagementSystemEntities entities = new HumanResourcesManagementSystemEntities();
            var file = entities.Attachments.ToList().Find(p => p.Id == fileId.Value);
            return File(file.Data, file.ContentType, file.FileName);
        }

        private void UploadDocuments(string path, HttpPostedFileBase file)
        {
            try
            {
                using (HumanResourcesManagementSystemEntities dbEntities = new HumanResourcesManagementSystemEntities())
                {
                    byte[] bytes;

                    using (BinaryReader br = new BinaryReader(file.InputStream))
                    {
                        bytes = br.ReadBytes(file.ContentLength);
                    }

                    var Attachment = new Attachment { FileName = Path.GetFileName(file.FileName), ContentType = file.ContentType, Data = bytes, DateUploaded = DateTime.Now, DocumentNo = LeaveDocumentNo,DocumentType = "Leave" };

                    dbEntities.Configuration.ValidateOnSaveEnabled = false;
                    dbEntities.Attachments.Add(Attachment);
                    dbEntities.SaveChanges();

                }
            } 
            catch (Exception es)
            {
                AppFunctions.WriteLog(es.Message);
            }
        }
        // Leave Entry Type -> Opening Balance",Accrue,Deduct,Use,Closing,Recall

        //Approal stuff
        public ActionResult SubmitForApproval()
        {
            string status = "", message = "";
            //submit for approval
            string approvalrequestresponse = MakerChecker.SendApprovalRequest(LeaveDocumentNo);

            ApprovalRequestResponse googlecalendar = JsonConvert.DeserializeObject<ApprovalRequestResponse>(approvalrequestresponse);

            status = googlecalendar.Status;

            if (status=="000")
            {
                string body = string.Empty;

                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);

                string url = Url.Action("Login", "Account");

                string pathToTemplate = Server.MapPath("~/MailTemplates/ApprovalNotification.html");

                using (StreamReader reader = new StreamReader(pathToTemplate))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{Link}", domainName+url);
                body = body.Replace("{UserName}", googlecalendar.ApproverEmail);

                bool IsSendEmail = EmailFunctions.SendMail(googlecalendar.ApproverEmail, googlecalendar.ApproverEmail, "Approval Notification", body);

                status = "000";
                message = "Submit Success! for leave " + LeaveDocumentNo;
            }
            else
            {
                status = "999";
                message = "Submit Failed for leave " + LeaveDocumentNo;
            }


            var _RequestResponse = new RequestResponse
            {
                Status = status,
                Message = message
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit(string DocumentNo)
        {
            string status = "", message = "";
            //submit for approval

            string approvalrequestresponse = MakerChecker.SendApprovalRequest(LeaveDocumentNo);

            ApprovalRequestResponse _ApprovalRequestResponse = JsonConvert.DeserializeObject<ApprovalRequestResponse>(approvalrequestresponse);

            status = _ApprovalRequestResponse.Status;


            if (status == "000")
            {
                string body = string.Empty;

                string domainName = Request.Url.GetLeftPart(UriPartial.Authority);

                string url = Url.Action("Login", "Account");

                string pathToTemplate = Server.MapPath("~/MailTemplates/ApprovalNotification.html");

                using (StreamReader reader = new StreamReader(pathToTemplate))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{Link}", domainName + url);
                body = body.Replace("{UserName}", _ApprovalRequestResponse.ApproverEmail);

                bool IsSendEmail = EmailFunctions.SendMail(_ApprovalRequestResponse.ApproverEmail, _ApprovalRequestResponse.ApproverEmail, "Approval Notification", body);

                status = "000";
                message = "Submit Success! for leave " + DocumentNo;
            }
            else
            {
                status = "999";
                message = "Submit Failed for leave " + DocumentNo;
            }               


            var _RequestResponse = new RequestResponse
            {
                Status = status,
                Message = message
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewLeave(string id)
        {
            return View();
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
        public ActionResult Delete(string DocumentNo)
        {
            var _RequestResponse = new RequestResponse
            {
                Status = "900",
                Message = "Delete Success! for leave " + DocumentNo
            };

            return Json(JsonConvert.SerializeObject(_RequestResponse), JsonRequestBehavior.AllowGet);
        }
        //get approve
        public string GetListOfDates(DateTime startdate, DateTime enddate)
        {
            var dates = new List<DateTime>();

            for (var dt = startdate; dt <= enddate; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            return dates.ToString();
        }
        private static int GetApprovalProgress( string DocumentNumber)
        {
            int progress = 0;

            try
            {
                using (var db = new HumanResourcesManagementSystemEntities())
                {
                    var NoOfApprovals = db.ApprovalEntries.Where(x =>  x.DocumentNo == DocumentNumber).ToList();
                    var NoOfApproved = db.ApprovalEntries.Where(x => x.DocumentNo == DocumentNumber && x.Status =="Approved").ToList();

                    progress = (Convert.ToInt32(NoOfApproved) / Convert.ToInt32(NoOfApprovals)) * 100;
                }
            }
            catch (Exception ex)
            {
                progress = 0;
            }
            return progress;
        }
    }
}