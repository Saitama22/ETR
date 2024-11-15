using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using ETR.Handlers.Interface;
using ETR.Helpers;
using ETR.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ETR.Handlers {
	public class JwtAuthHandler : IJwtAuthHandler {
		private UserManager<IdentityUser> _userManager;
		private IConfiguration _configuration;

		public JwtAuthHandler(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager) {
			_userManager = userManager;
			_configuration = configuration;
		}

		public async Task<string> GenerateTokenAsync(LoginDTO model) {
			var user = await _userManager.FindByNameAsync(model.Username);
			if (user == null)
				return null;
			var token = GenerateJwtToken(user);
			return token;
		}

		public async Task<bool> TryRegisterAsync(RegisterDTO model) {
			var user = new IdentityUser { UserName = model.Email, Email = model.Email };

			if (await _userManager.FindByEmailAsync(user.Email) != null)
				return false;

			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return false;

			return true;
		}

		public async Task<bool> ConfirmEmail(string userId, string token) {
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return false;

			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (!result.Succeeded) {
				return false;
			}
			return true;
		}

		public async Task<bool> ChangePassword(PasswordChangeDTO passwordChangeDTO, string userName) {
			var user = await _userManager.FindByNameAsync(userName);
			
			if (user == null) 
				return false;	
			await _userManager.ChangePasswordAsync(user, passwordChangeDTO.OldPassword, passwordChangeDTO.Password);
			return true;
		}

	 
		private string GenerateJwtToken(IdentityUser user) {
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.Name, user.UserName),
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
