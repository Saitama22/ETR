using System.ComponentModel.DataAnnotations;

namespace ETR.Models.DTOs
{
    public class LoginDTO {
		[Required]
		[EmailAddress]
		public string Username { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
    }
}
