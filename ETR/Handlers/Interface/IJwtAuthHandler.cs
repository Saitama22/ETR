using System.Security.Claims;
using ETR.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ETR.Handlers.Interface {
	public interface IJwtAuthHandler {
		Task<bool> ChangePassword(PasswordChangeDTO passwordChangeDTO, string userName);
		Task<bool> ConfirmEmail(string userId, string token);
		Task<string> GenerateTokenAsync(LoginDTO model);
		Task<bool> TryRegisterAsync(RegisterDTO model);
	}
}
