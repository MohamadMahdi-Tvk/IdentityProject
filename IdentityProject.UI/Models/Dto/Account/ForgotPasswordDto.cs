using System.ComponentModel.DataAnnotations;

namespace IdentityProject.UI.Models.Dto.Account
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
