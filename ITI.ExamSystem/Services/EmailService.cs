using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace ITI.ExamSystem.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //smtpClient.UseDefaultCredentials = false;
            //var smtpClient = new SmtpClient(_config["Email:Smtp"])
            //{
            //    Port = int.Parse(_config["Email:Port"]),
            //    Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
            //    EnableSsl = true,
            //};

            var smtpClient = new SmtpClient(_config["Email:Smtp"]);

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = int.Parse(_config["Email:Port"]);
            smtpClient.Credentials = new NetworkCredential(
                _config["Email:Username"],
                _config["Email:Password"]
            );
            smtpClient.EnableSsl = true;


            var mail = new MailMessage
            {
                From = new MailAddress(_config["Email:Username"], "Exam System"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mail.To.Add(email);

            await smtpClient.SendMailAsync(mail);
        }
    }
}
