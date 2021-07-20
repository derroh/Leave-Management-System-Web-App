using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;
    using HumanResources.Models;
    using HumanResources.ViewModels;
    using System.Security.Cryptography;
    using System.Text;
    using System.IO;

    public class AccountController : Controller
    {
        #region Private Properties

        /// <summary>
        /// Database Store property.
        /// </summary>
        private LeaveManagementEntities databaseManager = new LeaveManagementEntities();

        #endregion

        #region Default Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController" /> class.
        /// </summary>
        public AccountController()
        {
        }

        #endregion

        #region Login methods

        /// <summary>
        /// GET: /Account/Login
        /// </summary>
        /// <param name="returnUrl">Return URL parameter</param>
        /// <returns>Return login view</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Verification.
                if (this.Request.IsAuthenticated)
                {
                    // Info.
                    return this.RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return this.View();
        }

        /// <summary>
        /// POST: /Account/Login
        /// </summary>
        /// <param name="model">Model parameter</param>
        /// <param name="returnUrl">Return URL parameter</param>
        /// <returns>Return login view</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                string password = GetMD5(model.Password);
                // Verification.
                if (ModelState.IsValid)
                {
                    var loginInfo = this.databaseManager.Users.Where(m => m.Email == model.Email && m.Password == password).ToList();

                    // Verification.
                    if (loginInfo != null && loginInfo.Count() > 0)
                    {
                        // Initialization.
                        var logindetails = loginInfo.First();

                        string Role = logindetails.Role.Trim();
                        string Username = logindetails.Email.Trim();

                        // Login In.
                        this.SignInUser(Username, Role, false);

                        // setting.
                        this.Session["role_id"] = Role;
                        this.Session["EmployeeNo"] = logindetails.EmployeeNo;
                        this.Session["FirstName"] = logindetails.FirstName;
                        

                        // Info.
                        if (String.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else
                        {
                            return this.RedirectToLocal(returnUrl);
                        }
                    }
                    else
                    {
                        // Setting.
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        #endregion

        #region Log Out method.

        /// <summary>
        /// POST: /Account/LogOff
        /// </summary>
        /// <returns>Return log off action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                // Setting.
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;

                // Sign Out.
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }

            // Info.
            return this.RedirectToAction("Login", "Account");
        }

        public ActionResult LogOut()
        {
            try
            {
                // Setting.
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;

                // Sign Out.
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }

            // Info.
            return this.RedirectToAction("Login", "Account");
        }

        #endregion

        #region Helpers

        #region Sign In method.

        /// <summary>
        /// Sign In User method.
        /// </summary>
        /// <param name="username">Username parameter.</param>
        /// <param name="role_id">Role ID parameter</param>
        /// <param name="isPersistent">Is persistent parameter.</param>
        private void SignInUser(string username, string role_id, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();

            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(ClaimTypes.Role, role_id.ToString()));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;

                // Sign In.
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }

        #endregion

        #region Redirect to local method.

        /// <summary>
        /// Redirect to local method.
        /// </summary>
        /// <param name="returnUrl">Return URL parameter.</param>
        /// <returns>Return redirection action</returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }

            // Info.
            return this.RedirectToAction("Index", "Home");
        }

        #endregion

        #endregion

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(HumanResources.ViewModels.RegisterViewModel ep)
        {
            if (ModelState.IsValid)
            {
                var employee = new Employee
                {
                    EmployeeNo = ep.EmployeeNo,
                    FirstName = ep.FirstName,
                    LastName = ep.LastName,
                    FullName = ep.FirstName + " "+ ep.LastName,
                    CellularPhoneNumber = "",
                    Gender = "",
                    EMail = ep.Email,
                    JobTitle = "Dev",
                    Date_Of_Joining_the_Company = DateTime.Now,
                    Department_Name = "",
                    AnnualLeaveDaysEntitled = 28,
                    CurrentYear = DateTime.Now.Year,
                    Status = 1

                };

                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                {
                    dbEntities.Configuration.ValidateOnSaveEnabled = false;
                    dbEntities.Employees.Add(employee);
                    dbEntities.SaveChanges();
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
                }
             //   AppFunctions.SendTextMessage(_user.Phone, " Dear " + _user.FirstName + ", your account on Bettie's voting system has been created at " + DateTime.Now.ToShortTimeString());

                ViewBag.Message = "Account Created successfully";

            }
            return this.View(ep);
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

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (var context = new LeaveManagementEntities())
            {
                var user = context.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordViewModel model = new ResetPasswordViewModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (var context = new LeaveManagementEntities())
                {
                    var user = context.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        //you can encrypt password here, we are not doing it
                        user.Password = GetMD5(model.NewPassword);
                        //make resetpasswordcode empty string now
                        user.ResetPasswordCode = "";
                        //to avoid validation issues, disable it
                        context.Configuration.ValidateOnSaveEnabled = false;
                        context.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Some fields were not populated";
            }
            ViewBag.Message = message;

            // return this.RedirectToAction("Login", "Account");
            return View();
        }

        [AllowAnonymous]
        public ActionResult Reset()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel p)
        {
            if (ModelState.IsValid)
            {
                using (LeaveManagementEntities dbEntities = new LeaveManagementEntities())
                {
                    var user = dbEntities.Users.Where(u => u.Email == p.Email).SingleOrDefault();

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

                        string pathToTemplate = Server.MapPath("~/MailTemplates/ForgotPassword.html");

                        using (StreamReader reader = new StreamReader(pathToTemplate))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{ResetLink}", domainName + verifyUrl);
                        body = body.Replace("{UserName}", user.FirstName);

                        bool IsSendEmail = EmailFunctions.SendMail(p.Email, user.FirstName, "Password Reset Success", body);

                      

                        ViewBag.Message = "Reset password link has been sent to your email address";
                    }
                }
            }
            return this.RedirectToAction("Login", "Account");
        }
    }
}