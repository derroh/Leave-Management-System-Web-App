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

                var observeddays = _PublicHolidays.Where(au => au.Name.Contains("observed"));

                foreach (var day in observeddays)
                {
                    sortedList.Remove(day);
                }
                //
                var offdays = _PublicHolidays.Where(au => au.Name.Contains("off"));

                foreach (var day in offdays)
                {
                    sortedList.Remove(day);
                }


                using (var db = new LeaveManagementEntities())
                {
                    foreach (var holday in sortedList)
                    {
                        if (!db.PublicHolidays.Any(o => o.Id == holday.Id))
                        {
                            var publicholiday = new PublicHoliday { Id = holday.Id, HolidayDate = holday.HolidayDate, HolidayName = holday.Name, IsObserved = "Yes" };
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
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult UpdateHoliday(string HolidayId, string IsObserved)
        {
            string status = "", message = "";

            try
            {
                using (var db = new LeaveManagementEntities())
                {

                    var holidays = _db.PublicHolidays.Where(s => s.Id == HolidayId).SingleOrDefault();

                    if (holidays != null)
                    {
                        holidays.IsObserved = IsObserved;
                        db.SaveChanges();
                        status = "000";
                        message = "Holiday updated successfully";
                    }
                    else
                    {
                        status = "900";
                        message = "Holiday not found";
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

        public ActionResult AppSettings()
        {
            var settings = _db.Settings.Where(x => x.Id == 1).FirstOrDefault();

            var _AppSettings = new ViewModels.AppSettings
            {
                AfricasTalkingApiKey = settings.AfricasTalkingApiKey,
                AfricasTalkingAppName = settings.AfricasTalkingAppName,
                GoogleId = settings.GoogleId,
                EmailSender = settings.EmailSender,
                GmailPassword = settings.GmailPassword,
                GmailAppPassword = settings.GmailAppPassword,
                GmailSenderName = settings.GmailSenderName,
                GmailUsername = settings.GmailUsername,
                SMTPHost = settings.SMTPHost,
                SMTPPort = settings.SMTPPort.ToString(),
                DepartmentNumbers = settings.DepartmentNumbers,
                LeaveNumbers = settings.LeaveNumbers,
                EmployeeNumbers = settings.EmployeeNumbers,
                LeaveRecallNumbers = settings.LeaveRecallNumbers

            };
            return View(_AppSettings);
        }

        public ActionResult SaveSettings(ViewModels.AppSettings ep)
        {
            string message = "", status = "";

            try
            {
                
                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                {
                    var settings = dbEntities.Settings.Where(s => s.Id == 1).SingleOrDefault();

                    if (settings != null)
                    {

                        settings.AfricasTalkingApiKey = ep.AfricasTalkingApiKey;
                        settings.AfricasTalkingAppName = ep.AfricasTalkingAppName;
                        settings.GoogleId = ep.GoogleId;
                        settings.EmailSender = ep.EmailSender;
                        settings.GmailPassword = ep.GmailPassword;
                        settings.GmailAppPassword = ep.GmailAppPassword;
                        settings.GmailSenderName = ep.GmailSenderName;
                        settings.GmailUsername = ep.GmailUsername;
                        settings.SMTPHost = ep.SMTPHost;
                        settings.SMTPPort = Convert.ToInt32(ep.SMTPPort);
                        settings.DepartmentNumbers = ep.DepartmentNumbers;
                        settings.LeaveNumbers = ep.LeaveNumbers;
                        settings.EmployeeNumbers = ep.EmployeeNumbers;
                        settings.LeaveRecallNumbers = ep.LeaveRecallNumbers;
                        dbEntities.SaveChanges();

                        message = "Settings updated successfully";
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
    }
}