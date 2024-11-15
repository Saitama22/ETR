using System.ComponentModel.DataAnnotations;

namespace ETR.Models.DTOs {
	public class PasswordChangeDTO {
		[Required]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
		public string ConfirmPassword { get; set; }
	}
}
