using Microsoft.AspNetCore.Identity;

namespace CarRentalPortal01.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public int Role { get; set; }
        public decimal Salary { get; set; }
    }
}