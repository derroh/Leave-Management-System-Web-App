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
            var EmployeeProfile = new EmployeeProfileViewModel 
            {
                FullName = "Derrick Witness Abucheri",
                Status = "active",
                CellularPhoneNumber = "254701964636",
                City = "Nairobi",
                CompanyEMail ="derrick@dev.co.ke",
                Country = "kenya",
                CurrentYear = 2021,
                DateOfBirth = AppFunctions.GetReadableDate("01/29/1996"),
                Retirementdate = DateTimeUtil.DateDiff(DateInterval.Year, Convert.ToDateTime("01/29/1996"), Convert.ToDateTime("01/29/2006")).ToString(),
                DateOfLeaving = AppFunctions.GetReadableDate("01/29/1996"),
                Date_Of_Joining_the_Company = AppFunctions.GetReadableDate("01/29/1996"),
                Date_Of_Leaving_the_Company = AppFunctions.GetReadableDate("01/29/1996"),
                EMail ="Derrickwitness@gmai.com",
                Gender = "Male",
                EmployeeNo = "0002302",
                JobTitle = "Complex Software Developer",
                PostCode = "30600",
                WorkPhoneNumber = "0720000000"

            };
            dynamic mymodel = new ExpandoObject();

            mymodel.EmployeeProfile = EmployeeProfile;

            return View(mymodel);
        }
        
    }
}