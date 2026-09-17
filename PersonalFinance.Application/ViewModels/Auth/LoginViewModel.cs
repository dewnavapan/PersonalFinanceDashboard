using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกอีเมล")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}