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
        private static LeaveManagementEntities _db = new LeaveManagementEntities();
        static string LeaveDocumentNo = null;
        public ActionResult Index()
        {
            string status = Request.QueryString["status"];
            string SenderId = HttpContext.Session["EmployeeNo"].ToString();

            int approvalstatus = 0;

            if (string.IsNullOrEmpty(status))
            {
                approvalstatus = (int)DocumentApprovalStatus.Open;
            }            

            List<LeavesListViewModel> _LeavesListViewModel = new List<LeavesListViewModel>();

           
            if (status == "open")
            {
                approvalstatus = (int)DocumentApprovalStatus.Open;
            }

            if (status == "pending")
            {
                approvalstatus = (int)DocumentApprovalStatus.ApprovalPending;
            }

            if (status == "approved")
            {
                approvalstatus = (int)DocumentApprovalStatus.Approved;
            }

            if (status == "rejected")
            {
                approvalstatus = (int)DocumentApprovalStatus.Rejected;
            }

            ViewBag.ListType = (DocumentApprovalStatus)Convert.ToInt32(approvalstatus);

            var leaves = _db.Leaves.Where(a => a.ApprovalStatus == approvalstatus && a.EmployeeNo == SenderId).ToList();

            foreach (var leave in leaves)
            {
                _LeavesListViewModel.Add(new LeavesListViewModel { DocumentNo = leave.DocumentNo, EmployeeName = "Derrick Witness Abucheri", ApprovalStatus = leave.ApprovalStatus.ToString(), DateSubmitted = AppFunctions.GetReadableDate(DateTime.Now.ToString()), EndDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), StartDate = AppFunctions.GetReadableDate(DateTime.Now.ToString()), LeaveDays = leave.LeaveDaysApplied.ToString(), DocumentType = "Leave", LeaveType = "Annual Leave", ApprovalProgress = GetApprovalProgress(leave.DocumentNo, approvalstatus) });
            }
            
            return View(_LeavesListViewModel.OrderByDescending(x =>x.DocumentNo));
        }       
       
        public ActionResult Create()
        {
            var leavetypes = _db.LeaveTypes.ToList();
            ViewBag.LeaveTypes = leavetypes;

            return View();
        }
        public ActionResult Edit()
        {
            var leavetypes = _db.LeaveTypes.ToList();
            ViewBag.LeaveTypes = leavetypes;

            return View();
        }
        //Get LeaveTypes
        public JsonResult ListLeaveTypes()
        {
            List<LeaveType> leavetypelist = new List<LeaveType>();

            using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
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

            using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
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
            using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
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

            Code = Code.Trim();

            DateTime LeaveStartDate = Convert.ToDateTime(StartDate);

            IEnumerable<DateTime> holidays;

            holidays = _db.PublicHolidays.Where(h => h.HolidayDate >= LeaveStartDate).Select(h => h.HolidayDate).ToList();

            //only holidays on business days

            using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
            {
                var leaveType = dbEntities.LeaveTypes.Where(s => s.Code == Code).SingleOrDefault();

                if(leaveType != null)
                {
                    string AnnualLeaveDaysType = leaveType.AnnualLeaveDaysType;

                    if (AnnualLeaveDaysType.Trim() == "Consecutive Days")
                    {
                        var dtResult = LeaveStartDate.AddDays(Convert.ToInt32(LeaveDaysApplied));

                        EndDate = dtResult.ToString("MM/dd/yyyy");//
                    }
                    else if (AnnualLeaveDaysType.Trim() == "Working Days")
                    {
                        var dtResult = DateTimeExtensions.AddBusinessDays(Convert.ToDateTime(StartDate), Convert.ToInt32(LeaveDaysApplied), holidays);
                        EndDate = dtResult.ToString("MM/dd/yyyy");
                    }
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

            string SenderId = HttpContext.Session["EmployeeNo"].ToString();

            //create Leave Header here
            LeaveManagementEntities _db = new LeaveManagementEntities();

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
                        EmployeeNo = SenderId,
                        ApprovalStatus = (int)DocumentApprovalStatus.Open
                    };

                    using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
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
                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
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
            LeaveManagementEntities entities = new LeaveManagementEntities();
            var file = entities.Attachments.ToList().Find(p => p.Id == fileId.Value);
            return File(file.Data, file.ContentType, file.FileName);
        }

        private void UploadDocuments(string path, HttpPostedFileBase file)
        {
            try
            {
                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
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

            //check if user has leave days based on ledger entries
            //count number of leave days taken

            try 
            {
                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                {
                    var leave = dbEntities.Leaves.Where(s => s.DocumentNo == LeaveDocumentNo).SingleOrDefault();

                    if (leave != null)
                    {

                        //get number of days entilted

                        var LeaveType = dbEntities.LeaveTypes.Where(lt => lt.Code == leave.LeaveType).FirstOrDefault();

                        if (LeaveType != null)
                        {
                            var result = dbEntities.EmployeeLedgerEntries.Where(s => s.EmployeeNo == leave.EmployeeNo).GroupBy(o => o.LeaveType)
                                                                         .Select(g => new { leavetype = g.Key, total = g.Sum(i => i.Quantity) });
                            int TotalDaysTaken = 0;

                            foreach (var group in result)
                            {
                                TotalDaysTaken = Convert.ToInt32(group.total);
                            }

                            int DaysEntitled = Convert.ToInt32(LeaveType.TotalAbsence);

                            int LeaveBalance = (DaysEntitled - TotalDaysTaken);

                            //Balance - Appled  should be more than or equal to 0
                            if (LeaveBalance - leave.LeaveDaysApplied > 0)
                            {
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
                                    message = "Submit Success! for leave " + LeaveDocumentNo;
                                }
                                else
                                {
                                    status = "999";
                                    message = "Submit Failed for leave " + LeaveDocumentNo;
                                }
                            }
                            else
                            {
                                status = "999";
                                message = "Submit Failed for leave " + LeaveDocumentNo + ". You have exhausted your leave days. Your leave balance is " + LeaveBalance.ToString();
                            }
                        }
                    }
                }
            }
            catch(Exception es)
            {
                status = "999";
                message = es.Message;
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

            //check if user has leave days based on ledger entries
            //count number of leave days taken
            //
            try
            {
                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                {
                    var leave = dbEntities.Leaves.Where(s => s.DocumentNo == DocumentNo).SingleOrDefault();

                    if (leave != null)
                    {

                        //get number of days entilted

                        var LeaveType = dbEntities.LeaveTypes.Where(lt => lt.Code == leave.LeaveType).FirstOrDefault();

                        if (LeaveType != null)
                        {
                            var result = dbEntities.EmployeeLedgerEntries.Where(s => s.EmployeeNo == leave.EmployeeNo).GroupBy(o => o.LeaveType)
                                                                         .Select(g => new { leavetype = g.Key, total = g.Sum(i => i.Quantity) });
                            int TotalDaysTaken = 0;

                            foreach (var group in result)
                            {
                                TotalDaysTaken = Convert.ToInt32(group.total);
                            }

                            int DaysEntitled = Convert.ToInt32(LeaveType.TotalAbsence);

                            int LeaveBalance = (DaysEntitled - TotalDaysTaken);

                            //Balance - Appled  should be more than or equal to 0
                            if (LeaveBalance - leave.LeaveDaysApplied > 0)
                            {
                                //submit for approval

                                string approvalrequestresponse = MakerChecker.SendApprovalRequest(DocumentNo);

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
                            }
                            else
                            {
                                status = "999";
                                message = "Submit Failed for leave " + DocumentNo + ". You have exhausted your leave days. Your leave balance is " + LeaveBalance.ToString();
                            }
                        }
                    }
                }
            }
            catch(Exception es)
            {
                status = "999";
                message = es.Message;
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
            LeaveApplicationViewModel leaveApp = new LeaveApplicationViewModel();
            var leavetypes = _db.LeaveTypes.ToList();
            ViewBag.LeaveTypes = leavetypes;

            var leave = _db.Leaves.Where(x => x.DocumentNo == id).FirstOrDefault();

            leaveApp.LeaveType = leave.LeaveType;
            leaveApp.LeaveDaysEntitled = "0";
            leaveApp.LeaveDaysTaken = "0";
            leaveApp.LeaveBalance = "0";
            leaveApp.LeaveAccruedDays = "0";
            leaveApp.LeaveOpeningBalance = "0";
            leaveApp.DocumentNo = id;

            //selection
            leaveApp.SelectionType = "";
            leaveApp.StartDate = Convert.ToDateTime(leave.StartDate).ToString("MM/dd/yyyy");
            leaveApp.EndDate = Convert.ToDateTime(leave.EndDate).ToString("MM/dd/yyyy");
            leaveApp.LeaveDates = leave.LeaveDates;
            leaveApp.LeaveDaysApplied = leave.LeaveDaysApplied.ToString();
            leaveApp.LeaveStartDate = Convert.ToDateTime(leave.StartDate).ToString("MM/dd/yyyy");
            leaveApp.LeaveEndDate = Convert.ToDateTime(leave.EndDate).ToString("MM/dd/yyyy");
            leaveApp.ReturnDate = Convert.ToDateTime(leave.ReturnDate).ToString("MM/dd/yyyy");

            return View(leaveApp);
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

            string status = "", message = "";

            try
            {
                using (var db = new LeaveManagementEntities())
                {
                    var leave = db.Leaves.Where(x => x.DocumentNo == DocumentNo).SingleOrDefault();

                    if (leave != null)
                    {
                        db.Leaves.Remove(leave);
                        db.SaveChanges();
                        status = "000";
                        message = "Leave " + DocumentNo +" has been successfully deleted";
                    }
                    else
                    {
                        status = "900";
                        message = "Couldn't find leave " + DocumentNo;
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
        private static int GetApprovalProgress( string DocumentNumber, int status)
        {
            int progress = 0;

            try
            {
                if(status != (int)DocumentApprovalStatus.Approved)
                {
                    using (var db = new LeaveManagementEntities())
                    {
                        var NoOfApprovals = db.ApprovalEntries.Where(x => x.DocumentNo == DocumentNumber).ToList();
                        var NoOfApproved = db.ApprovalEntries.Where(x => x.DocumentNo == DocumentNumber && x.Status == (int)DocumentApprovalStatus.Approved).ToList();

                        progress = (Convert.ToInt32(NoOfApproved) / Convert.ToInt32(NoOfApprovals)) * 100;
                    }
                }
                else
                {
                    progress = 100;
                }
               
            }
            catch (Exception ex)
            {
                progress = 0;
            }
            return progress;
        }
        //partial views

        public PartialViewResult LeaveAttachments(string id)
        {
            var attachments = _db.Attachments.Where(a => a.DocumentNo == id);
            List<LeaveAttachmentsViewModel> _LeaveAttachmentsViewModel = new List<LeaveAttachmentsViewModel>();

            if(attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    _LeaveAttachmentsViewModel.Add(new LeaveAttachmentsViewModel { Id = attachment.Id, Name = attachment.FileName, DateUploaded = attachment.DateUploaded.ToString(), FileType = attachment.ContentType });
                }
            }

            return PartialView("LeaveAttachmentsView", _LeaveAttachmentsViewModel);
        }
    }
}