using HumanResources.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        LeaveManagementEntities _db = new LeaveManagementEntities();
        // GET: Settings
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Holidays()
        {
            try
            {
                var client = new RestClient("https://www.googleapis.com/calendar/v3/calendars/en.ke%23holiday%40group.v.calendar.google.com/events?key=AIzaSyBQF1TR_4h4mFj9UZMOl07GRJbH_SiRxxY");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                //Console.WriteLine(response.Content);

                string jsonGoogleCal = response.Content;

                GoogleCalendarAPI googlecalendar = JsonConvert.DeserializeObject<GoogleCalendarAPI>(jsonGoogleCal);

                List<PublicHolidays> _PublicHolidays = new List<PublicHolidays>();

                foreach (var holiday in googlecalendar.items)
                {
                    _PublicHolidays.Add(new PublicHolidays { HolidayDate = Convert.ToDateTime(holiday.start.date), Id = holiday.id, Name = holiday.summary });
                    //Console.WriteLine(holiday.id + " - " + holiday.summary +" - "+ holiday.start.date);
                }
                // = MembershipNumber;

                List<PublicHolidays> sortedList = _PublicHolidays.Where(f => f.HolidayDate > DateTime.Now).OrderBy(o => o.HolidayDate).ToList();
                //  Console.WriteLine((int)ElectionStatus.Created);

                using (var db = new LeaveManagementEntities())
                {
                    foreach (var holday in sortedList)
                    {
                        if (!db.PublicHolidays.Any(o => o.Id == holday.Id))
                        {
                            var publicholiday = new PublicHoliday { Id = holday.Id, HolidayDate = holday.HolidayDate, HolidayName = holday.Name, IsObserved = 1 };
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.PublicHolidays.Add(publicholiday);
                            db.SaveChanges();
                        }
                    }

                }
            }
            catch(Exception es)
            {
                AppFunctions.WriteLog(es.Message);
            }
            DateTime SixmonthsFromToday = DateTime.Now.AddMonths(6); 

            return View(from publicholidays in _db.PublicHolidays.Where(h =>h.HolidayDate <= SixmonthsFromToday).OrderBy(h=>h.HolidayDate)
                        select publicholidays);
        }
        public ActionResult ApprovalUsers()
        {
            return View( from approvers in _db.ApprovalUsers 
                         select approvers);
        }
        public ActionResult EditApprover()
        {
            return View();
        }
        public ActionResult DeleteApprover(string id)
        {
            var _RequestResponse = new RequestResponse
            {
                Status = "900",
                Message = "Delete Success! for leave approver" + id
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

        public ActionResult LeaveTypes()
        {
            return View(from leaveTypes in _db.LeaveTypes
                        select leaveTypes);
        }
    }
}