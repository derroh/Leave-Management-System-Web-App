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
        HumanResourcesManagementSystemEntities _db = new HumanResourcesManagementSystemEntities();
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

                using (var db = new HumanResourcesManagementSystemEntities())
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
    }
}