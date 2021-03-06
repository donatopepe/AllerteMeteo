using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MeteoAlert.Entities;
using System.Net;
using System.Net.Mail;

namespace MeteoAlert.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendEmailTextAsync(string email, string testosms, string content);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            //_logger.LogInformation(">>>>>>>>>>>>>>>>" + email + subject + message);
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                //_logger.LogInformation(">>>>>>>>>>>>>>>>" + _emailSettings.SmtpUsername + _emailSettings.SmtpPassword);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.SmtpUsername, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                //_logger.LogInformation(">>>>>>>>>>>>>>>>" + _emailSettings.SmtpUsername + _emailSettings.SenderName);
                mail.To.Add(new MailAddress(email));
                //_logger.LogInformation(">>>>>>>>>>>>>>>> creo mail " + _emailSettings.SmtpPort+ _emailSettings.SmtpServer);
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.SmtpPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.SmtpServer,
                    EnableSsl = _emailSettings.SmtpSSL.ToLower() == "false" ? false : true,
                    Credentials = credentials
                };
                //_logger.LogInformation(">>>>>>>>>>>>>>>>" + client.Port + client.EnableSsl);
                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            return Task.CompletedTask;
        }
        public Task SendEmailTextAsync(string email, string subject, string message)
        {
            //_logger.LogInformation(">>>>>>>>>>>>>>>>" + email + subject + message);
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                //_logger.LogInformation(">>>>>>>>>>>>>>>>" + _emailSettings.SmtpUsername + _emailSettings.SmtpPassword);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.SmtpUsername, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false
                };
                //_logger.LogInformation(">>>>>>>>>>>>>>>>" + _emailSettings.SmtpUsername + _emailSettings.SenderName);
                mail.To.Add(new MailAddress(email));
                //_logger.LogInformation(">>>>>>>>>>>>>>>> creo mail " + _emailSettings.SmtpPort+ _emailSettings.SmtpServer);
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.SmtpPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.SmtpServer,
                    EnableSsl = _emailSettings.SmtpSSL.ToLower() == "false" ? false : true,
                    Credentials = credentials
                };
                //_logger.LogInformation(">>>>>>>>>>>>>>>>" + client.Port + client.EnableSsl);
                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
