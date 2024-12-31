using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "User name is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Current password is required.")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Current password should be at least 8 characters long.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "New password should be at least 8 characters long.")]
        public string NewPassword { get; set; }
    }
}
