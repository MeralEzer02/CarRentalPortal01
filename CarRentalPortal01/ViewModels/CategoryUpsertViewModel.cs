using CarRentalPortal01.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRentalPortal01.ViewModels
{
    public class CategoryUpsertViewModel
    {
        public Category Category { get; set; }
        public IEnumerable<SelectListItem>? VehicleList { get; set; }
        public int[]? SelectedVehicleIds { get; set; }
    }
}