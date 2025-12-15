using CarRentalPortal01.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRentalPortal01.ViewModels
{
    public class RentalUpsertViewModel
    {
        public Rental Rental { get; set; }

        // Müşteri Seçimi İçin Liste
        public IEnumerable<SelectListItem>? UserList { get; set; }

        // Araç Seçimi İçin Liste
        public IEnumerable<SelectListItem>? VehicleList { get; set; }
    }
}