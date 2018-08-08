using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Base.Library
{
    public static class MailHelper
    {
        public static string AppEmailAddress;// = ConfigurationManager.AppSettings["ApplicationEmailAddress"];
        public static  string SMTPServer;// = ConfigurationManager.AppSettings["SMTPServer"];
        public static  string SMTPLogin;// = ConfigurationManager.AppSettings["SMTPLogin"];
        public static  string SMTPPassword;// = ConfigurationManager.AppSettings["SMTPPassword"];
        public static  int SMTPPort;// = Int32.Parse(ConfigurationManager.AppSettings["SMTPPort"]);

        private static void SendEmail(MailAddress fromAddress, MailAddress toAddress, string subject, string body, bool isHighPriority)
        {
            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            if (isHighPriority)
            {
                message.Priority = MailPriority.High;
            }
            using (var client = new SmtpClient(SMTPServer))
            {
                client.Port = SMTPPort;
                client.Credentials = new NetworkCredential(SMTPLogin, SMTPPassword);
                client.Send(message);
            }
        }

        public static void SendEmailsBcc(string fromAddress, IList<string> bccAddresses, string subject, string body, bool isHighPriority)
        {
            var message = new MailMessage()
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.From = new MailAddress(fromAddress ?? AppEmailAddress);
            foreach (string email in bccAddresses)
            {
                if (!String.IsNullOrWhiteSpace(email))
                    message.Bcc.Add(new MailAddress(email));
            }

            if (isHighPriority)
            {
                message.Priority = MailPriority.High;
            }
            using (var client = new SmtpClient(SMTPServer))
            {
                client.Port = SMTPPort;
                client.Credentials = new NetworkCredential(SMTPLogin, SMTPPassword);
                client.Send(message);
            }
        }

        //public static void SendEmailToRecipients(string subject, string body, bool isHighPriority)
        //{
        //    using (var penContext = new PatientEventNotificationContext())
        //    {
        //        foreach (var pen in penContext.Emails)
        //        {
        //            SendEmail(new MailAddress(AppEmailAddress), new MailAddress(pen.EmailAddress), subject, body, isHighPriority);
        //        }
        //    }
        //}

        public static void SendEmailToListOfEmails(List<string> emails, string subject, string body, bool isHighPriority)
        {
            var errors = new List<SmtpException>();
            foreach (var email in emails)
            {
                try
                {
                    SendEmail(new MailAddress(AppEmailAddress), new MailAddress(email), subject, body, isHighPriority);
                }
                catch (SmtpException e)
                {
                    errors.Add(e);
                }
            }
            if (errors.Count > 0)
            {
                throw new AggregateException("Failed to send email to " + errors.Count + " recipients.", errors);
            }
        }
    }
}
