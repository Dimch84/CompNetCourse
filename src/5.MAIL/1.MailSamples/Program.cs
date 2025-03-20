using System;
using System.Configuration;
using System.Net.Mail;

namespace MailSamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string smtpUsername = appSettings["SmtpUser"]; // "st007122@spbu.ru"
            var smtpPassword = appSettings["SmtpPwd"];

            SmtpSend(recipients: new string[1] { "dmitry.shalymov@gmail.com" }, from: "st007122@spbu.ru",
                subject: "Test subject", 
                message: "Hi There! How are you doing?",
                smtpServer: "mail.spbu.ru", smtpPort: 25,
                smtpUsername, smtpPassword);
        }

        public static void SmtpSend(string[] recipients, string from, string subject, string message, string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
        {
            try
            {
                foreach (string recipient in recipients)
                {
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.To.Add(recipient);
                    mailMsg.From = new MailAddress(from);
                    mailMsg.Subject = subject;
                    mailMsg.Body = message;

                    SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                    smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = false;
                    
                    if (!string.IsNullOrEmpty(smtpUsername))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                    }

                    smtpClient.Send(mailMsg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex);
            }
        }
    }
}
