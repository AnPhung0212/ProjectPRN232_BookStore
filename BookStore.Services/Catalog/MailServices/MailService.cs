using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static BookStore.BusinessObject.Config.MailSetting;

namespace BookStore.Services.Catalog.MailServices
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings?.Value ?? throw new ArgumentNullException(nameof(mailSettings));
        }

        public Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            // 1. Kiểm tra tham số
            ValidateEmailParameters(toEmail, subject, body);

            // 2. Thực hiện gửi email bất đồng bộ
            return SendEmailInternalAsync(toEmail, subject, body, isHtml);
        }

        // Method kiểm tra tham số
        private static void ValidateEmailParameters(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email cannot be null or empty", nameof(toEmail));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject cannot be null or empty", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Email body cannot be null or empty", nameof(body));
        }

        // Method bất đồng bộ thực hiện gửi email
        private async Task SendEmailInternalAsync(string toEmail, string subject, string body, bool isHtml)
        {
            using var smtp = new SmtpClient
            {
                Host = _mailSettings.SmtpServer,
                Port = _mailSettings.Port,
                EnableSsl = _mailSettings.EnableSSL,
                Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.Password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(toEmail);

            await smtp.SendMailAsync(mailMessage);
        }
    }
}
