using ETR.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ETR.Helpers {
	public class ETREmailSender : IEmailSender {
		private readonly EmailSettings _emailSettings;

		public ETREmailSender(IOptions<EmailSettings> emailSettings) {
			_emailSettings = emailSettings.Value;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage) {
			using (var client = new SmtpClient()) {
				await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);

				var mailMessage = new MimeMessage {
					From = { new MailboxAddress("Your App Name", _emailSettings.SmtpUser) },
					To = { new MailboxAddress("", email) },
					Subject = subject,
					Body = new TextPart("html") { Text = htmlMessage }
				};

				await client.SendAsync(mailMessage);
				await client.DisconnectAsync(true);
			}
		}
	}
}
