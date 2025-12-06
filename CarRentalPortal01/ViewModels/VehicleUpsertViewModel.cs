using CarRentalPortal01.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRentalPortal01.ViewModels
{
    public class VehicleUpsertViewModel
    {
        public Vehicle Vehicle { get; set; }
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        public int[]? SelectedCategoryIds { get; set; }
    }
}
