using System.ComponentModel.DataAnnotations;

namespace FileStorageWeb.ViewModels.Public
{
	public class RegisterViewModel
	{
		[Required]
		[StringLength(50, ErrorMessage = "The {0} must be at max {1} characters long.")]
		[Display(Name = "Username")]
		public string UserName { get; set; }

		[Required]
		[Display(Name = "First name")]
		public string FirstName { get; set; }

		[Required]
		[Display(Name = "Last name")]
		public string LastName { get; set;}

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
