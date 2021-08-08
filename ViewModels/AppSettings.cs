using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.ViewModels
{
    public class AppSettings
    {
        public string AfricasTalkingAppName { get; set; }
        public string AfricasTalkingApiKey { get; set; }
        public string GoogleId { get; set; }
        public string GmailAppPassword { get; set; }
        public string GmailSenderName { get; set; }
        public string GmailUsername { get; set; }
        public string GmailPassword { get; set; }
        public string EmailSender { get; set; }
        public string SMTPPort { get; set; }
        public string SMTPHost { get; set; }
        public string LeaveNumbers { get; set; }
        public string LeaveRecallNumbers { get; set; }
        public string DepartmentNumbers { get; set; }
        public string EmployeeNumbers { get; set; }
    }
}