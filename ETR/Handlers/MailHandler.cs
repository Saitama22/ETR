using System.Text.Encodings.Web;
using System;
using ETR.Handlers.Interface;
using ETR.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using static System.Collections.Specialized.BitVector32;

namespace ETR.Handlers {
	public class MailHandler : IMailHandler {
		private UserManager<IdentityUser> _userManager;
		private IEmailSender _sendMail;

		public MailHandler(UserManager<IdentityUser> userManager, IEmailSender sendMail) {
			_userManager = userManager;
			_sendMail = sendMail;
		}

		public async Task SendMessageEmailConfirm(string email, string? confirmUrl) {
			var user = await _userManager.FindByEmailAsync(email);
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var uriBuilder = new UriBuilder(confirmUrl) {
				Query = $"userId={user.Id}&token={token}"
			};
			var confirmUrlWithQuery = uriBuilder.ToString();
			await _sendMail.SendEmailAsync(email, "Подтверждите почту",
				$"Подтвержите почту по: <a href='{HtmlEncoder.Default.Encode(confirmUrlWithQuery)}'>ссылке</a>");
		}
	}
}
