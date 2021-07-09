using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using HumanResources.Models;

namespace HumanResources
{
    public class AppFunctions
    {
        private static  LeaveManagementSystemEntities _db = new LeaveManagementSystemEntities();

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);
        public static bool IsInternetAvailable()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
        public static string GetReadableDate(string Date)
        {
           DateTime DateToConvert = Convert.ToDateTime(Date);

            return DateToConvert.ToString("d MMMM yyyy",
                     System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        }
        public static bool SendTextMessage(string recipts, string message)
        {
            bool _status = false;

            if (true)
            {
                string status = null;

                var recipients = recipts;

                string timenow = DateTime.Now.ToString("yyyy-MM-d");

                var settings = _db.Settings.Where(s => s.Id == 1).SingleOrDefault();

                string username = settings.AfricasTalkingAppName;

                string apiKey = settings.AfricasTalkingApiKey;

                if (AppFunctions.IsInternetAvailable())
                {
                    AfricasTalkingGateway gateway = new AfricasTalkingGateway(username, apiKey);

                    try
                    {
                        dynamic results = gateway.sendMessage(recipients, message);


                        foreach (dynamic result in results)
                        {
                            status = (string)result["status"];
                            int statusCode = (int)result["statusCode"];
                            string number = (string)result["number"];
                            string cost = (string)result["cost"];
                            string messageId = (string)result["messageId"];
                            _status = true;
                        }
                    }
                    catch (AfricasTalkingGatewayException ex)
                    {
                        _status = false;
                        WriteLog("Encountered an error: " + ex.ToString());
                    }
                }
                else
                {
                    WriteLog("Internet Connection not available");
                }
            }
            return _status;
        }
        public static bool IsEmailAvailable(string Email)
        {
            bool status = false;

            //var check = _db.Users.FirstOrDefault(s => s.Email == Email);
            //if (check == null)
            //{

            //    status = true;
            //}
            return status;
        }
        public static bool IsPhoneAvailable(string Phone)
        {
            bool status = false;

            //var check = _db.Users.FirstOrDefault(s => s.Phone == Phone);
            //if (check == null)
            //{

            //    status = true;
            //}
            return status;
        }
        public static bool IsStudentAvailable(string StudentId)
        {
            bool status = false;

            //var check = _db.Users.FirstOrDefault(s => s.StudentId == StudentId);
            //if (check == null)
            //{

            //    status = true;
            //}
            return status;
        }
        public static void WriteLog(string text)
        {
            try
            {
                //set up a filestream
                string strPath = @"C:\Logs\ElectionSystem";
                string fileName = DateTime.Now.ToString("MMddyyyy") + "_logs.txt";
                string filenamePath = strPath + '\\' + fileName;
                Directory.CreateDirectory(strPath);
                FileStream fs = new FileStream(filenamePath, FileMode.OpenOrCreate, FileAccess.Write);
                //set up a streamwriter for adding text
                StreamWriter sw = new StreamWriter(fs);
                //find the end of the underlying filestream
                sw.BaseStream.Seek(0, SeekOrigin.End);
                //add the text
                sw.WriteLine(DateTime.Now.ToString() + " : " + text);
                //add the text to the underlying filestream
                sw.Flush();
                //close the writer
                sw.Close();
            }
            catch (Exception ex)
            {
                //throw;
                ex.Data.Clear();
            }
        }
        public static string GetNewDocumentNumber(string NumberSeriesCode, string LastUsedNumber)
        {
            string RecordNumber = LastUsedNumber.Substring(NumberSeriesCode.Length);
            int DecimalPlaces = RecordNumber.Length;
            int LastAutoIncrementId = Convert.ToInt32(RecordNumber);

            string FormatterPrefix = "";
            string fmt = ".##";

            char pad = '0';
            FormatterPrefix = FormatterPrefix.PadLeft(DecimalPlaces, pad);
            FormatterPrefix = FormatterPrefix + fmt;

            int NextAutoIncrementId = LastAutoIncrementId + 1;
            return NumberSeriesCode + NextAutoIncrementId.ToString(FormatterPrefix);
        }
        public static bool UpdateNumberSeries(string NumberSeriesCode, string LastUsedNumber)
        {
            bool status = false;

            //var check = _db.NumberSeries.FirstOrDefault(s => s.Code == NumberSeriesCode);
            //if (check != null)
            //{
            //    check.LastUsedNumber = LastUsedNumber;
            //    _db.SaveChanges();
            //    status = true;
            //}
            return status;
        }
    }
    static class RegexFunctions
    {
        /**
        * Function validates email address
        */
        public static bool IsValidEmailAddress(this string s)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"); //  "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$"
            return regex.IsMatch(s);
        }
        /**
        * Function validates phone number
        */
        public static bool IsValidPhoneNumber(this string s)
        {
            

            string inputPhoneNumber = ""; //todo: populate correct number string validPhoneNumber = null;
            var regEx = new Regex("^(?:254|\\+254|0)?(7(?:(?:[12][0-9])|(?:0[0-8])|(?:9[0-2]))[0-9]{6})$");
            var match = regEx.Match(inputPhoneNumber);
            if (match.Success)
            {
               string validPhoneNumber = "254" + match.Groups[1].Value;
            }
            //safaricom new numbers from 2020 regex
            var newSafRegex = new Regex("^(?:254|\\+254|0)?((?:(?:7(?:(?:[01249][0-9])|(?:5[789])|(?:6[89])))|(?:1(?:[1][0-5])))[0-9]{6})$");
            var newAirtelRegex = new Regex("^(?:254|\\+254|0)?((?:(?:7(?:(?:3[0-9])|(?:5[0-6])|(8[5-9])))|(?:1(?:[0][0-2])))[0-9]{6})$");

            Regex regex = new Regex("(\\+?254)7(\\d){8}");
            return regex.IsMatch(s);
        }
        public static bool IsPasswordStrong(this string s)
        {            
            Regex regex = new Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{6,20}$");
            return regex.IsMatch(s);
        }
    }
    public static class HtmlUtility
    {
 
        public static string IsActive(this HtmlHelper html,
                                  string control,
                                  string action)
        {
            var routeData = html.ViewContext.RouteData;
 
            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];
 
            // must match both
            var returnActive = control == routeControl &&
                               action == routeAction;

            return returnActive ? "active" : "";
        }
        public static string IsParentActive(this HtmlHelper html,
                                 string control,
                                 string action)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            string returnText = "";

            if((routeAction != null || routeControl != null) && control == routeControl  )
            {
                returnText = "active open";
            }

            return returnText;
        }
        public static string IsParent2Active(this HtmlHelper html,
                                 string control,
                                 string action)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            string returnText = "";

            if ((routeAction != null || routeControl != null) && control == routeControl &&
                               action == routeAction)
            {
                returnText = "active open";
            }

            return returnText;
        }
        public static string IsLastChildActive(this HtmlHelper html,
                                  string control,
                                  string action,
                                  string status)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];
       

            // must match both
            var returnActive = control == routeControl &&
                               action == routeAction ;

            // return returnActive ? "active" : "";
            return returnActive ? "" : "";
        }
    }
    /// <summary>
    /// Helper/extension class for manipulating date and time values.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Adds the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be added.</param>
        /// <param name="holidays">An optional list of holiday (non-business) days to consider.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime AddBusinessDays(
            this DateTime current,
            int days,
            IEnumerable<DateTime> holidays = null)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    current = current.AddDays(sign);
                }
                while (current.DayOfWeek == DayOfWeek.Saturday
                    || current.DayOfWeek == DayOfWeek.Sunday
                    || (holidays != null && holidays.Contains(current.Date))
                    );
            }
            return current;
        }

        /// <summary>
        /// Subtracts the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <param name="holidays">An optional list of holiday (non-business) days to consider.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime SubtractBusinessDays(
            this DateTime current,
            int days,
            IEnumerable<DateTime> holidays)
        {
            return AddBusinessDays(current, -days, holidays);
        }

        /// <summary>
        /// Retrieves the number of business days from two dates
        /// </summary>
        /// <param name="startDate">The inclusive start date</param>
        /// <param name="endDate">The inclusive end date</param>
        /// <param name="holidays">An optional list of holiday (non-business) days to consider.</param>
        /// <returns></returns>
        public static int GetBusinessDays(
            this DateTime startDate,
            DateTime endDate,
            IEnumerable<DateTime> holidays)
        {
            if (startDate > endDate)
                throw new NotSupportedException("ERROR: [startDate] cannot be greater than [endDate].");

            int cnt = 0;
            for (var current = startDate; current < endDate; current = current.AddDays(1))
            {
                if (current.DayOfWeek == DayOfWeek.Saturday
                    || current.DayOfWeek == DayOfWeek.Sunday
                    || (holidays != null && holidays.Contains(current.Date))
                    )
                {
                    // skip holiday 
                }
                else cnt++;
            }
            return cnt;
        }

        /// <summary>
        /// Calculate Easter Sunday for any given year.
        /// src.: https://stackoverflow.com/a/2510411/1233379
        /// </summary>
        /// <param name="year">The year to calcolate Easter against.</param>
        /// <returns>a DateTime object containing the Easter month and day for the given year</returns>
        public static DateTime GetEasterSunday(int year)
        {
            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Retrieve holidays for given years
        /// </summary>
        /// <param name="years">an array of years to retrieve the holidays</param>
        /// <param name="countryCode">a country two letter ISO (ex.: "IT") to add the holidays specific for that country</param>
        /// <param name="cityName">a city name to add the holidays specific for that city</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetHolidays(IEnumerable<int> years, string countryCode = null, string cityName = null)
        {
            var lst = new List<DateTime>();

            foreach (var year in years.Distinct())
            {
                lst.AddRange(new[] {
                new DateTime(year, 1, 1),       // 1 gennaio (capodanno)
                new DateTime(year, 1, 6),       // 6 gennaio (epifania)
                new DateTime(year, 5, 1),       // 1 maggio (lavoro)
                new DateTime(year, 8, 15),      // 15 agosto (ferragosto)
                new DateTime(year, 11, 1),      // 1 novembre (ognissanti)
                new DateTime(year, 12, 8),      // 8 dicembre (immacolata concezione)
                new DateTime(year, 12, 25),     // 25 dicembre (natale)
                new DateTime(year, 12, 26)      // 26 dicembre (s. stefano)
            });

                // add easter sunday (pasqua) and monday (pasquetta)
                var easterDate = GetEasterSunday(year);
                lst.Add(easterDate);
                lst.Add(easterDate.AddDays(1));

                // country-specific holidays
                if (!String.IsNullOrEmpty(countryCode))
                {
                    switch (countryCode.ToUpper())
                    {
                        case "IT":
                            lst.Add(new DateTime(year, 4, 25));     // 25 aprile (liberazione)
                            break;
                        case "US":
                            lst.Add(new DateTime(year, 7, 4));     // 4 luglio (Independence Day)
                            break;

                        // todo: add other countries

                        default:
                            // unsupported country: do nothing
                            break;
                    }
                }

                // city-specific holidays
                if (!String.IsNullOrEmpty(cityName))
                {
                    switch (cityName)
                    {
                        case "Rome":
                        case "Roma":
                            lst.Add(new DateTime(year, 6, 29));  // 29 giugno (s. pietro e paolo)
                            break;
                        case "Milano":
                        case "Milan":
                            lst.Add(new DateTime(year, 12, 7));  // 7 dicembre (s. ambrogio)
                            break;

                        // todo: add other cities

                        default:
                            // unsupported city: do nothing
                            break;

                    }
                }
            }
            return lst;
        }
    }
    public static class DateTimeUtil
    {       
        public static long DateDiff(DateInterval interval, DateTime date1, DateTime date2)
        {

            TimeSpan ts = date2 - date1;

            switch (interval)
            {
                case DateInterval.Year:
                    return date2.Year - date1.Year;
                case DateInterval.Month:
                    return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));
                case DateInterval.Weekday:
                    return Fix(ts.TotalDays) / 7;
                case DateInterval.Day:
                    return Fix(ts.TotalDays);
                case DateInterval.Hour:
                    return Fix(ts.TotalHours);
                case DateInterval.Minute:
                    return Fix(ts.TotalMinutes);
                default:
                    return Fix(ts.TotalSeconds);
            }
        }

        private static long Fix(double Number)
        {
            if (Number >= 0)
            {
                return (long)Math.Floor(Number);
            }
            return (long)Math.Ceiling(Number);
        }
    }
    public enum DateInterval
    {
        Year,
        Month,
        Weekday,
        Day,
        Hour,
        Minute,
        Second
    }
}