using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal01.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı giriniz.")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email gereklidir.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola gereklidir.")]
        public string Password { get; set; } = string.Empty;
    }
}
