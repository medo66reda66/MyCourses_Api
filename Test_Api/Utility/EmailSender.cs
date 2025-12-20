using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Ecommers.Api.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client =  new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("medo66reda6677@gmail.com", "xhkb ynfu tmdy zuap")

                };
            return client.SendMailAsync(
                new MailMessage(from: "medo66reda6677@gmail.com",
                to: email,
                subject,
                htmlMessage)
                {IsBodyHtml= true });
        }
    }
}
