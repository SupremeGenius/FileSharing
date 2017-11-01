using System.ComponentModel.DataAnnotations;

namespace FileSharing.Services.Dtos
{
    public class UserRegistrationDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "FIELD_MIN_MAX_LENGTH", MinimumLength = 6)]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "FIELD_MIN_MAX_LENGTH", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PASSWORD_NOT_MATCHING")]
        public string ConfirmPassword { get; set; }
    }
}
