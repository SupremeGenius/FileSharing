using System.ComponentModel.DataAnnotations;

namespace FileSharingWeb.ViewModels.Public
{
	public class Register
	{
		[StringLength(50, ErrorMessage = "FIELD_MAX_LENGTH")]
		public string Username { get; set; }
        
		public string FirstName { get; set; }
        
		public string LastName { get; set;}
        
		[StringLength(100, ErrorMessage = "FIELD_MIN_MAX_LENGTH", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "PASSWORD_NOT_MATCHING")]
		public string ConfirmPassword { get; set; }
	}
}
