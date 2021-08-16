using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    using HumanResources.Models;
    using HumanResources.ViewModels;
    using Newtonsoft.Json;
    using System.Security.Cryptography;
    using System.Text;

    [Authorize]
    public class ProfileController : Controller
    {
        LeaveManagementEntities _db = new LeaveManagementEntities();
        // GET: Profile
        public ActionResult Index()
        {
            string username = HttpContext.Session["EmployeeNo"].ToString();
            var EmployeeProfile = new EmployeeProfileViewModel { };

            try
            {
                using(var db = new LeaveManagementEntities())
                {
                    var employee = db.Employees.Where(x => x.EmployeeNo == username).FirstOrDefault();
                    var user = db.Users.Where(x => x.EmployeeNo == username).FirstOrDefault();

                    EmployeeProfile.FullName = employee.FullName;
                    EmployeeProfile.Status = "Active";
                    EmployeeProfile.CellularPhoneNumber = employee.CellularPhoneNumber;
                    EmployeeProfile.City = "Nairobi";
                    EmployeeProfile.CompanyEMail = employee.CompanyEMail;
                    EmployeeProfile.Country = "Kenya";
                    EmployeeProfile.CurrentYear = DateTime.Now.Year;
                    EmployeeProfile.DateOfBirth = AppFunctions.GetReadableDate(employee.DateOfBirth.ToString());
                    EmployeeProfile.Retirementdate = AppFunctions.GetReadableDate(employee.Retirementdate.ToString());
                    EmployeeProfile.DateOfLeaving = AppFunctions.GetReadableDate(employee.DateOfLeaving.ToString());
                    EmployeeProfile.Date_Of_Joining_the_Company = AppFunctions.GetReadableDate(employee.Date_Of_Joining_the_Company.ToString());
                    EmployeeProfile.Date_Of_Leaving_the_Company = AppFunctions.GetReadableDate(employee.Date_Of_Leaving_the_Company.ToString());
                    ViewBag.EMail = user.Email;
                    EmployeeProfile.Gender = employee.Gender;
                    EmployeeProfile.EmployeeNo = employee.EmployeeNo;
                    EmployeeProfile.JobTitle = employee.JobTitle;
                    EmployeeProfile.PostCode = employee.PostCode;
                    EmployeeProfile.WorkPhoneNumber = employee.WorkPhoneNumber;
                }
            }
            catch (Exception ex)
            {
                AppFunctions.WriteLog("Employee profile: " + ex.Message);
            }

            return View(EmployeeProfile);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(ChangePasswordViewModel changepass)
        {
            string status = "", message = "";
            string username = HttpContext.Session["EmployeeNo"].ToString();
            string oldPassword = changepass.OldPassword;
            string newPassword = changepass.NewPassword;
            string confirmPassword = changepass.ConfirmPassword;

            try
            {
                if (newPassword == confirmPassword)
                {
                    confirmPassword = GetMD5(confirmPassword);
                    oldPassword = GetMD5(oldPassword);

                    using (var db = new LeaveManagementEntities())
                    {
                        //Generate Department number here
                        var user = db.Users.Where(s => s.EmployeeNo == username && s.Password == oldPassword).SingleOrDefault();

                        if (user != null)
                        {
                            user.Password = confirmPassword;
                            db.SaveChanges();

                            status = "000";
                            message = "Password updated successfully";

                        }
                        else
                        {
                            status = "900";
                            message = "Failed to change password. Your old password is incorrect";
                        }
                    }
                }
                else
                {
                    status = "900";
                    message = "Failed to change password. Your passwords do not match";
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
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}