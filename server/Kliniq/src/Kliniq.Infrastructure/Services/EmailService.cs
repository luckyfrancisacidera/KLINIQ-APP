using Kliniq.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Kliniq.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _configuration["SmtpSettings:FromName"],
                _configuration["SmtpSettings:FromEmail"]!));

            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _configuration["SmtpSettings:Host"]!,
                int.Parse(_configuration["SmtpSettings:Port"]!),
                SecureSocketOptions.StartTls,
                cancellationToken);

            await client.AuthenticateAsync(
                _configuration["SmtpSettings:Username"]!,
                _configuration["SmtpSettings:Password"]!,
                cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
