using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPlanner.API.Services.EmailConfirmation
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;

        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();

                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));

                var name = "asd";

                mimeMessage.To.Add(new MailboxAddress(name, email));

                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("html")

                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync("Angles92Test@gmail.com", "B3dub311-");

                    await client.SendAsync(mimeMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
