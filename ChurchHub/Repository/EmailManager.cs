using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ChurchHub.Repository
{
    public class MailManager
    {
        // Setters and Getters
        private string MailSender { get; set; }
        private string MailAppPassword { get; set; }

        // Constructor
        public MailManager()
        {
            MailSender = ConfigurationManager.AppSettings["MailSender"];
            MailAppPassword = ConfigurationManager.AppSettings["MailSenderAppPassword"];
        }

        // Method for Sending Email
        public bool SendEmail(string szRecipient, string subject, string szMsgBody, ref string errResponse)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    var smtp = new SmtpClient();
                    message.From = new MailAddress(MailSender);
                    message.To.Add(new MailAddress(szRecipient));
                    message.Subject = subject;
                    message.IsBodyHtml = true; // to make message body as html
                    message.Body = szMsgBody;

                    smtp.Port = 587;
                    smtp.Host = "smtp.gmail.com"; // for gmail host
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(MailSender, MailAppPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(message);

                    // Set success response
                    errResponse = "Message Sent";

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Set error response
                errResponse = ex.Message;

                return false;
            }
        }
    }
}
