using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace HumanResources
{
    class EmailFunctions
    {
        private static Models.HumanResourcesManagementSystemEntities _db = new Models.HumanResourcesManagementSystemEntities();
        //function sends emails
        public static bool SendMail(string RecipientMail, string RecipientName, string MailSubject, string MailBody)
        {
            bool status = false;
            var settings = _db.Settings.Where(s => s.Id == 1).SingleOrDefault();


            int Port = Convert.ToInt32(settings.SMTPPort);
            string Host = settings.SMTPHost;
            string Username = settings.GmailUsername;
            string Password = settings.GmailAppPassword;
            string SenderMail = settings.GmailUsername;
            string SenderName = settings.GmailUsername;

            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(settings.GmailUsername, settings.GmailAppPassword);
            client.Port = Port;
            client.Host = Host;
            client.EnableSsl = true;
            try
            {
                if (AppFunctions.IsInternetAvailable())
                {

                    using (MailMessage emailMessage = new MailMessage())
                    {
                        emailMessage.From = new MailAddress(SenderMail, SenderName);
                        emailMessage.To.Add(new MailAddress(RecipientMail, RecipientName));
                        emailMessage.Subject = MailSubject;
                        emailMessage.SubjectEncoding = Encoding.UTF8;
                        emailMessage.Body = MailBody;
                        emailMessage.IsBodyHtml = true;
                        emailMessage.BodyEncoding = Encoding.UTF8;
                        emailMessage.Priority = MailPriority.Normal;
                        using (SmtpClient MailClient = new SmtpClient(Host, Port))
                        {
                            MailClient.EnableSsl = true;
                            MailClient.Credentials = new System.Net.NetworkCredential(Username, Password);
                            MailClient.Send(emailMessage);
                        }
                    }
                    
                    status = true;
                }
            }
            catch (Exception e)
            {
                AppFunctions.WriteLog(e.Message);
            }

            return status;
        }
        //function sends emails with attachment
        public static bool SendMailAttachemnt(string RecipientMail, string RecipientName, string MailSubject, string MailBody, string file)
        {            
            bool status = false;
            var settings = _db.Settings.Where(s => s.Id == 1).SingleOrDefault();


            int Port = Convert.ToInt32(settings.SMTPPort);
            string Host = settings.SMTPHost;
            string Username = settings.GmailUsername;
            string Password = settings.GmailAppPassword;
            string SenderMail = settings.GmailUsername;
            string SenderName = settings.GmailUsername;

            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(settings.GmailUsername, settings.GmailAppPassword);
            client.Port = Port;
            client.Host = Host;
            client.EnableSsl = true;
            try
            {
                if (AppFunctions.IsInternetAvailable())
                {

                    using (MailMessage emailMessage = new MailMessage())
                    {
                        emailMessage.From = new MailAddress(SenderMail, SenderName);
                        emailMessage.To.Add(new MailAddress(RecipientMail, RecipientName));
                        emailMessage.Subject = MailSubject;
                        emailMessage.SubjectEncoding = Encoding.UTF8;
                        emailMessage.Body = MailBody;
                        emailMessage.IsBodyHtml = true;
                        emailMessage.BodyEncoding = Encoding.UTF8;
                        emailMessage.Priority = MailPriority.Normal;

                        // Create  the file attachment for this e-mail message.
                        Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(file);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                        // Add the file attachment to this e-mail message.
                        emailMessage.Attachments.Add(data);
                        emailMessage.Attachments.Add(data);


                        using (SmtpClient MailClient = new SmtpClient(Host, Port))
                        {
                            MailClient.EnableSsl = true;
                            MailClient.Credentials = new System.Net.NetworkCredential(settings.GmailUsername, settings.GmailAppPassword);
                            MailClient.Send(emailMessage);
                        }
                    }
                    
                    status = true;
                }
            }
            catch (Exception e)
            {
                AppFunctions.WriteLog(e.Message);
            }

            return status;
        }
        
        public static string EmailBody(string param1, string param2, string param3, string param4, string param5)
        {
            string template = @"<!doctype html>
                                 <html xmlns='http://www.w3.org/1999/xhtml'>
                                 <head>
                                  <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
                                  <meta name='viewport' content='initial-scale=1.0' />
                                  <meta name='format-detection' content='telephone=no' />
                                  <title></title>
                                  <style type='text/css'>
 	                                body {
		                                width: 100%;
		                                margin: 0;
		                                padding: 0;
		                                -webkit-font-smoothing: antialiased;
	                                }
	                                @media only screen and (max-width: 600px) {
		                                table[class='table-row'] {
			                                float: none !important;
			                                width: 98% !important;
			                                padding-left: 20px !important;
			                                padding-right: 20px !important;
		                                }
		                                table[class='table-row-fixed'] {
			                                float: none !important;
			                                width: 98% !important;
		                                }
		                                table[class='table-col'], table[class='table-col-border'] {
			                                float: none !important;
			                                width: 100% !important;
			                                padding-left: 0 !important;
			                                padding-right: 0 !important;
			                                table-layout: fixed;
		                                }
		                                td[class='table-col-td'] {
			                                width: 100% !important;
		                                }
		                                table[class='table-col-border'] + table[class='table-col-border'] {
			                                padding-top: 12px;
			                                margin-top: 12px;
			                                border-top: 1px solid #E8E8E8;
		                                }
		                                table[class='table-col'] + table[class='table-col'] {
			                                margin-top: 15px;
		                                }
		                                td[class='table-row-td'] {
			                                padding-left: 0 !important;
			                                padding-right: 0 !important;
		                                }
		                                table[class='navbar-row'] , td[class='navbar-row-td'] {
			                                width: 100% !important;
		                                }
		                                img {
			                                max-width: 100% !important;
			                                display: inline !important;
		                                }
		                                img[class='pull-right'] {
			                                float: right;
			                                margin-left: 11px;
                                            max-width: 125px !important;
			                                padding-bottom: 0 !important;
		                                }
		                                img[class='pull-left'] {
			                                float: left;
			                                margin-right: 11px;
			                                max-width: 125px !important;
			                                padding-bottom: 0 !important;
		                                }
		                                table[class='table-space'], table[class='header-row'] {
			                                float: none !important;
			                                width: 98% !important;
		                                }
		                                td[class='header-row-td'] {
			                                width: 100% !important;
		                                }
	                                }
	                                @media only screen and (max-width: 480px) {
		                                table[class='table-row'] {
			                                padding-left: 16px !important;
			                                padding-right: 16px !important;
		                                }
	                                }
	                                @media only screen and (max-width: 320px) {
		                                table[class='table-row'] {
			                                padding-left: 12px !important;
			                                padding-right: 12px !important;
		                                }
	                                }
	                                @media only screen and (max-width: 458px) {
		                                td[class='table-td-wrap'] {
			                                width: 100% !important;
		                                }
	                                }
                                  </style>
                                 </head>
                                 <body style='font-family: Arial, sans-serif; font-size:13px; color: #444444; min-height: 200px;' bgcolor='#E4E6E9' leftmargin='0' topmargin='0' marginheight='0' marginwidth='0'>
                                 <table width='100%' height='100%' bgcolor='#E4E6E9' cellspacing='0' cellpadding='0' border='0'>
                                 <tr><td width='100%' align='center' valign='top' bgcolor='#E4E6E9' style='background-color:#E4E6E9; min-height: 200px;'>
                                <table><tr><td class='table-td-wrap' align='center' width='458'><table class='table-space' height='18' style='height: 18px; font-size: 0px; line-height: 0; width: 450px; background-color: #e4e6e9;' width='450' bgcolor='#E4E6E9' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='18' style='height: 18px; width: 450px; background-color: #e4e6e9;' width='450' bgcolor='#E4E6E9' align='left'>&nbsp;</td></tr></tbody></table>
                                <table class='table-space' height='8' style='height: 8px; font-size: 0px; line-height: 0; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='8' style='height: 8px; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' align='left'>&nbsp;</td></tr></tbody></table>

                                <table class='table-row' width='450' bgcolor='#FFFFFF' style='table-layout: fixed; background-color: #ffffff;' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-row-td' style='font-family: Arial, sans-serif; line-height: 19px; color: #444444; font-size: 13px; font-weight: normal; padding-left: 36px; padding-right: 36px;' valign='top' align='left'>
                                  <table class='table-col' align='left' width='378' cellspacing='0' cellpadding='0' border='0' style='table-layout: fixed;'><tbody><tr><td class='table-col-td' width='378' style='font-family: Arial, sans-serif; line-height: 19px; color: #444444; font-size: 13px; font-weight: normal; width: 378px;' valign='top' align='left'>
                                    <table class='header-row' width='378' cellspacing='0' cellpadding='0' border='0' style='table-layout: fixed;'><tbody><tr><td class='header-row-td' width='378' style='font-family: Arial, sans-serif; font-weight: normal; line-height: 19px; color: #478fca; margin: 0px; font-size: 18px; padding-bottom: 10px; padding-top: 15px;' valign='top' align='left'>"+ param1 + @"</td></tr></tbody></table>
                                    <div style='font-family: Arial, sans-serif; line-height: 20px; color: #444444; font-size: 13px;'>
                                      <b style='color: #777777;'>" + param2 + @"</b>
                                      <br>
                                      " + param3 + @"
                                    </div>
                                  </td></tr></tbody></table>
                                </td></tr></tbody></table>
    
                                <table class='table-space' height='12' style='height: 12px; font-size: 0px; line-height: 0; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='12' style='height: 12px; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' align='left'>&nbsp;</td></tr></tbody></table>
                                <table class='table-space' height='12' style='height: 12px; font-size: 0px; line-height: 0; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='12' style='height: 12px; width: 450px; padding-left: 16px; padding-right: 16px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' align='center'>&nbsp;<table bgcolor='#E8E8E8' height='0' width='100%' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td bgcolor='#E8E8E8' height='1' width='100%' style='height: 1px; font-size:0;' valign='top' align='left'>&nbsp;</td></tr></tbody></table></td></tr></tbody></table>
                                <table class='table-space' height='16' style='height: 16px; font-size: 0px; line-height: 0; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='16' style='height: 16px; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' align='left'>&nbsp;</td></tr></tbody></table>

                                <table class='table-row' width='450' bgcolor='#FFFFFF' style='table-layout: fixed; background-color: #ffffff;' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-row-td' style='font-family: Arial, sans-serif; line-height: 19px; color: #444444; font-size: 13px; font-weight: normal; padding-left: 36px; padding-right: 36px;' valign='top' align='left'>
                                  <table class='table-col' align='left' width='378' cellspacing='0' cellpadding='0' border='0' style='table-layout: fixed;'><tbody><tr><td class='table-col-td' width='378' style='font-family: Arial, sans-serif; line-height: 19px; color: #444444; font-size: 13px; font-weight: normal; width: 378px;' valign='top' align='left'>
                                    <div style='font-family: Arial, sans-serif; line-height: 19px; color: #444444; font-size: 13px; text-align: center;'>
                                      <a href='" + param4 + @"' style='color: #ffffff; text-decoration: none; margin: 0px; text-align: center; vertical-align: baseline; border: 4px solid #6fb3e0; padding: 4px 9px; font-size: 15px; line-height: 21px; background-color: #6fb3e0;'>&nbsp; " + param5 + @" &nbsp;</a>
                                    </div>
                                    <table class='table-space' height='16' style='height: 16px; font-size: 0px; line-height: 0; width: 378px; background-color: #ffffff;' width='378' bgcolor='#FFFFFF' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='16' style='height: 16px; width: 378px; background-color: #ffffff;' width='378' bgcolor='#FFFFFF' align='left'>&nbsp;</td></tr></tbody></table>
                                  </td></tr></tbody></table>
                                </td></tr></tbody></table>

                                <table class='table-space' height='6' style='height: 6px; font-size: 0px; line-height: 0; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='6' style='height: 6px; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' align='left'>&nbsp;</td></tr></tbody></table>

                                <table class='table-row-fixed' width='450' bgcolor='#FFFFFF' style='table-layout: fixed; background-color: #ffffff;' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-row-fixed-td' style='font-family: Arial, sans-serif; line-height: 19px; color: #444444; font-size: 13px; font-weight: normal; padding-left: 1px; padding-right: 1px;' valign='top' align='left'>
                                  <table class='table-col' align='left' width='448' cellspacing='0' cellpadding='0' border='0' style='table-layout: fixed;'><tbody><tr><td class='table-col-td' width='448' style='font-family: Arial, sans-serif; line-height: 19px; color: #444444; font-size: 13px; font-weight: normal;' valign='top' align='left'>
                                    <table width='100%' cellspacing='0' cellpadding='0' border='0' style='table-layout: fixed;'><tbody><tr><td width='100%' align='center' bgcolor='#f5f5f5' style='font-family: Arial, sans-serif; line-height: 24px; color: #bbbbbb; font-size: 13px; font-weight: normal; text-align: center; padding: 9px; border-width: 1px 0px 0px; border-style: solid; border-color: #e3e3e3; background-color: #f5f5f5;' valign='top'>
                                      <a href='#' style='color: #428bca; text-decoration: none; background-color: transparent;'>Bettie's Voting System &copy; " + DateTime.Now.Year.ToString() +@"</a>
                                      <br>
                                      <a href='#' style='color: #478fca; text-decoration: none; background-color: transparent;'>twitter</a>
                                      .
                                      <a href='#' style='color: #5b7a91; text-decoration: none; background-color: transparent;'>facebook</a>
                                      .
                                      <a href='#' style='color: #dd5a43; text-decoration: none; background-color: transparent;'>google+</a>
                                    </td></tr></tbody></table>
                                  </td></tr></tbody></table>
                                </td></tr></tbody></table>
                                <table class='table-space' height='1' style='height: 1px; font-size: 0px; line-height: 0; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='1' style='height: 1px; width: 450px; background-color: #ffffff;' width='450' bgcolor='#FFFFFF' align='left'>&nbsp;</td></tr></tbody></table>
                                <table class='table-space' height='36' style='height: 36px; font-size: 0px; line-height: 0; width: 450px; background-color: #e4e6e9;' width='450' bgcolor='#E4E6E9' cellspacing='0' cellpadding='0' border='0'><tbody><tr><td class='table-space-td' valign='middle' height='36' style='height: 36px; width: 450px; background-color: #e4e6e9;' width='450' bgcolor='#E4E6E9' align='left'>&nbsp;</td></tr></tbody></table></td></tr></table>
                                </td></tr>
                                 </table>
                                 </body>
                                 </html>";

            return template;
        }
    }
}
