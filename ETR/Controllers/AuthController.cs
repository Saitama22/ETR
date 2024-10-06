using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using ETR.Consts;
using ETR.Helpers;
using ETR.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ETR.Controllers
{
    public class AuthController : Controller {
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IEmailSender _sendMail;
		public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IEmailSender sendMail) {
			_userManager = userManager;
			_configuration = configuration;
			_sendMail = sendMail;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginModel model) {
			var user = await _userManager.FindByNameAsync(model.Username);
			if (user != null) {
				var token = GenerateJwtToken(user);
				return Ok(new { token });
			}
			return BadRequest("Не удалось выполнить вход");
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterModel model) {
			if (!ModelState.IsValid)
				return BadRequest();

			var user = new IdentityUser { UserName = model.Email, Email = model.Email };
			if (await _userManager.FindByEmailAsync(user.Email) != null)
				return BadRequest("Данный пользователь уже существует");

			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded) 
				return BadRequest();

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.Action("Confirm", "Auth", new { userId = user.Id, code = token }, HttpContext.Request.Scheme);
			await _sendMail.SendEmailAsync(model.Email, "Подтверждите почту",
				$"Подтвержите почту по: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ссылке</a>");
			return Ok();
		}

		[HttpGet]
		[Route("confirm")]
		public async Task<IActionResult> ConfirmEmail(string userId, string token) {
			if (userId == null || token == null) 
				return BadRequest();

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) 
				return BadRequest();

			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (result.Succeeded) {
				return Ok();
			}

			return BadRequest();
		}

		private string GenerateJwtToken(IdentityUser user) {
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
