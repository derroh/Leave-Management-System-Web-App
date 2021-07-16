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
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult Index()
        {
            var client = new RestClient("https://www.googleapis.com/calendar/v3/calendars/en.ke%23holiday%40group.v.calendar.google.com/events?key=AIzaSyBQF1TR_4h4mFj9UZMOl07GRJbH_SiRxxY");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);

            string jsonGoogleCal = response.Content;

            GoogleCalendarAPI jsonGetMemberInfo = JsonConvert.DeserializeObject<GoogleCalendarAPI>(jsonGoogleCal);

            List<PublicHolidays> _PublicHolidays = new List<PublicHolidays>();

            foreach (var holiday in jsonGetMemberInfo.items)
            {
                _PublicHolidays.Add(new PublicHolidays { HolidayDate = Convert.ToDateTime(holiday.start.date), Id = holiday.id, Name = holiday.summary });
                //Console.WriteLine(holiday.id + " - " + holiday.summary +" - "+ holiday.start.date);
            }
            // = MembershipNumber;

            List<PublicHolidays> sortedList = _PublicHolidays.Where(f => f.HolidayDate > DateTime.Now).OrderBy(o => o.HolidayDate).ToList();
            //  Console.WriteLine((int)ElectionStatus.Created);

            foreach (var holday in sortedList)
            {
                Console.WriteLine("Holiday Date " + holday.HolidayDate); //insert to Database here
            }

            return View();
        }
    }
}