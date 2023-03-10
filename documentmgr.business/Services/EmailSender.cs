using documentmgr.dto;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace documentmgr.business.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage,
            List<(string fileName, byte[] fileBytes, string contentType)> attachments);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailSender(IOptions<EmailSettings> emailSettings, IHttpContextAccessor httpContextAccessor)
        {
            _emailSettings = emailSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        private SecureSocketOptions getSecurityType(int securitySetting)
        {
            return securitySetting == 0 ? SecureSocketOptions.None
                 : securitySetting == 1 ? SecureSocketOptions.SslOnConnect
                 : SecureSocketOptions.StartTls;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage,
            List<(string fileName, byte[] fileBytes, string contentType)> attachments)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            mimeMessage.To.Add(MailboxAddress.Parse(email));

            mimeMessage.Subject = subject;
            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage,
            };

            if (mimeMessage != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    builder.Attachments.Add(attachment.fileName, attachment.fileBytes, ContentType.Parse(attachment.contentType));
                }
            }

            mimeMessage.Body = builder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_emailSettings.MailServer, _emailSettings.MailPort, getSecurityType(_emailSettings.Security));

                // Note: only needed if the SMTP server requires authentication
                if (_emailSettings.AuthenticateCredentials)
                    await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}