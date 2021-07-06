using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
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
}