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
                    EmployeeProfile.EMail = employee.EMail;
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
        
    }
}