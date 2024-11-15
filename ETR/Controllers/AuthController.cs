using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using ETR.Consts;
using ETR.Handlers;
using ETR.Handlers.Interface;
using ETR.Helpers;
using ETR.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ETR.Controllers
{
    public class AuthController : Controller {
		private readonly IJwtAuthHandler _authHandler;
		private readonly IMailHandler _mailHandler;

		public AuthController(IJwtAuthHandler authHandler, IMailHandler mailHandler) {
			_authHandler = authHandler;
			_mailHandler = mailHandler;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDTO model) {
			string token = await _authHandler.GenerateTokenAsync(model);
			if (string.IsNullOrEmpty(token)) {
				return Unauthorized();
			}
			return Ok(new { token });
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterDTO model) {
			if (!ModelState.IsValid)
				return BadRequest();
			if (!await _authHandler.TryRegisterAsync(model))
				return BadRequest();

			var confirmUrl = Url.Action("Confirm", "Auth", new { }, HttpContext.Request.Scheme);
			await _mailHandler.SendMessageEmailConfirm(model.Email, confirmUrl);			

			string token = await _authHandler.GenerateTokenAsync(new LoginDTO() {
				Password = model.Password,
				Username = model.Email
			});
			if (string.IsNullOrEmpty(token)) {
				return Unauthorized();
			}
			return Ok(new { token });
		}

		[HttpGet("confirm")]
		public async Task<IActionResult> ConfirmEmail(string userId, string token) {
			if (userId == null || token == null)
				return BadRequest();

			if (!await _authHandler.ConfirmEmail(userId, token))
				return BadRequest();
			return Ok();
		}

		[Authorize]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword(PasswordChangeDTO passwordChangeDTO) {
			if (!ModelState.IsValid)
				return BadRequest();	
			if (string.IsNullOrEmpty(User.Identity.Name))
				return BadRequest();
			if (!await _authHandler.ChangePassword(passwordChangeDTO, User.Identity.Name))
				return BadRequest();
			return Ok();				
		}
	}
}
