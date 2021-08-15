using HumanResources.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        LeaveManagementEntities _db = new LeaveManagementEntities();

        // GET: Employees
        public ActionResult Index()
        {
            return View(from employees in _db.Employees.Where(e => e.Status == 1)
                        select employees);
        }
        public ActionResult Create()
        {
            var departments = _db.Departments.ToList();

            ViewBag.Departments = departments;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEmployee(ViewModels.CreateEmployeeViewModel ep)
        {
            string message = "", DocumentNo = "", status = "";

            //create Leave Header here
            LeaveManagementEntities _db = new LeaveManagementEntities();

            try
            {
                var settings = _db.Settings.Where(s => s.Id == 1).SingleOrDefault();

                if (settings != null)
                {
                    string EmployeeCode = settings.EmployeeNumbers;

                    var NumberSeriesData = _db.NumberSeries.Where(s => s.Code == EmployeeCode).SingleOrDefault();

                    string LastUsedNumber = NumberSeriesData.LastUsedNumber;

                    if (LastUsedNumber != "")
                    {
                        DocumentNo = AppFunctions.GetNewDocumentNumber(EmployeeCode.Trim(), LastUsedNumber.Trim());

                      //  LeaveDocumentNo = DocumentNo;
                    }

                    var employee = new Employee
                    {
                        EmployeeNo = DocumentNo,
                        FirstName = ep.FirstName,
                        LastName = ep.LastName,
                        FullName = ep.Name,
                        CellularPhoneNumber = ep.Phone,
                        Gender = ep.Gender,                        
                        EMail = ep.Email,
                        JobTitle = "Dev",
                        Date_Of_Joining_the_Company = DateTime.Now,
                        Department_Name = ep.DepartmentName,
                        AnnualLeaveDaysEntitled = 28,
                        CurrentYear = DateTime.Now.Year,
                        Status = 1

                    };

                    using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                    {
                        dbEntities.Configuration.ValidateOnSaveEnabled = false;
                        dbEntities.Employees.Add(employee);
                        dbEntities.SaveChanges();
                        status = "000";
                        message = "Employee saved successfully";

                        AppFunctions.UpdateNumberSeries(EmployeeCode, DocumentNo);                                             
                    }

                    var user = new User
                    {
                        Email = ep.Email,
                        Password = GetMD5(ep.Password),
                        FirstName = ep.FirstName,
                        LastName = ep.LastName,
                        // Phone = _user.Phone,
                        Role = "Staff",
                        EmployeeNo = employee.EmployeeNo

                    };

                    using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                    {
                        dbEntities.Configuration.ValidateOnSaveEnabled = false;
                        dbEntities.Users.Add(user);
                        dbEntities.SaveChanges();


                        string statsus = await Task.Run(() => SendNotification(DocumentNo));
                    }


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

        private async Task<string> SendNotification(string documentNo)
        {
            using(var dbEntities= new LeaveManagementEntities())
            {
                var user = dbEntities.Users.Where(x => x.EmployeeNo == documentNo).FirstOrDefault();

                if (user != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    var verifyUrl = "/Account/ResetPassword/" + resetCode;
                    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                    user.ResetPasswordCode = resetCode;

                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 

                    dbEntities.Configuration.ValidateOnSaveEnabled = false;
                    dbEntities.SaveChanges();

                    string domainName = Request.Url.GetLeftPart(UriPartial.Authority);

                    string body = string.Empty;

                    string pathToTemplate = Server.MapPath("~/MailTemplates/SignUp.html");

                    using (StreamReader reader = new StreamReader(pathToTemplate))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{ResetLink}", domainName + verifyUrl);
                    body = body.Replace("{UserName}", user.FirstName);

                    bool IsSendEmail = await Task.Run(() => EmailFunctions.SendMailAsync(user.Email, user.FirstName, "Account creation Success", body));
                }
            }
            return "success";
        }

        public ActionResult Edit(string Id)
        {
            var departments = _db.Departments.ToList();

            ViewBag.Departments = departments;

            ViewModels.CreateEmployeeViewModel ep = new ViewModels.CreateEmployeeViewModel();

            var employee = _db.Employees.Where(e => e.EmployeeNo == Id).FirstOrDefault();

            if (employee != null)
            {
                ep.DepartmentName = employee.Department_Name;
                ep.Email = employee.EMail;
                ep.EmployeeNo = employee.EmployeeNo;
                ep.FirstName = employee.FirstName;
                ep.LastName = employee.LastName;
                ep.Name = employee.FullName;
                ep.Gender = employee.Gender;
                ep.JobTitle = employee.JobTitle;
                ep.Phone = employee.CellularPhoneNumber;
            }                

            return View(ep);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEmployee(ViewModels.CreateEmployeeViewModel ep)
        {
            string message = "", status = "";
            try
            {
                using (var _db = new LeaveManagementEntities())
                {
                    var employee = _db.Employees.Where(s => s.EmployeeNo == ep.EmployeeNo).SingleOrDefault();

                    if (employee != null)
                    {
                        employee.FirstName = ep.FirstName;
                        employee.LastName = ep.LastName;
                        employee.FullName = ep.Name;
                        employee.CellularPhoneNumber = ep.Phone;
                        employee.Gender = ep.Gender;
                        employee.EMail = ep.Email;
                        employee.JobTitle = "Dev";
                        employee.Date_Of_Joining_the_Company = DateTime.Now;
                        employee.Department_Name = ep.DepartmentName;
                        employee.AnnualLeaveDaysEntitled = 28;
                        employee.CurrentYear = DateTime.Now.Year;
                        employee.Status = 1;

                        _db.SaveChanges();

                        status = "000";
                        message = "Employee updated successfully";

                    }
                    else
                    {
                        message = "Employee was not found";
                        status = "900";
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
        public ActionResult Delete(string EmployeeNo)
        {
            string username = HttpContext.Session["EmployeeNo"].ToString();

            string status = "", message = "";

            try
            {
                if(username != EmployeeNo)
                {
                    using (var db = new LeaveManagementEntities())
                    {
                        var employee = db.Employees.Where(x => x.EmployeeNo == EmployeeNo).SingleOrDefault();

                        if (employee != null)
                        {
                            db.Employees.Remove(employee);
                            db.SaveChanges();
                            status = "000";
                            message = "Employee " + EmployeeNo + " has been successfully deleted";
                        }
                        else
                        {
                            status = "900";
                            message = "Couldn't find Employee " + EmployeeNo;
                        }
                    }
                }
                else
                {
                    status = "900";
                    message = "Cannot proceed with action. You cannot delete your own account " + EmployeeNo;
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

        public ActionResult ViewEmployee(string Id)
        {
            var departments = _db.Departments.ToList();

            ViewBag.Departments = departments;

            ViewModels.CreateEmployeeViewModel ep = new ViewModels.CreateEmployeeViewModel();

            var employee = _db.Employees.Where(e => e.EmployeeNo == Id).FirstOrDefault();

            if (employee != null)
            {
                ep.DepartmentName = employee.Department_Name;
                ep.Email = employee.EMail;
                ep.EmployeeNo = employee.EmployeeNo;
                ep.FirstName = employee.FirstName;
                ep.LastName = employee.LastName;
                ep.Name = employee.FullName;
                ep.Gender = employee.Gender;
                ep.JobTitle = employee.JobTitle;
                ep.Phone = employee.CellularPhoneNumber;
            }

            return View(ep);
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