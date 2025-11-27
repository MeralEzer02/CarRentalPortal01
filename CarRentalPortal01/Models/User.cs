using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal01.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public int Role { get; set; } = 0; // 0: User, 1: Admin
    }
}
