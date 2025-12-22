using Microsoft.AspNetCore.Http;
using CarRentalPortal01.Models;

namespace CarRentalPortal01.ViewModels
{
    public class UserUpsertViewModel
    {
        public OldUser User { get; set; }
        public IFormFile? File { get; set; }
    }
}