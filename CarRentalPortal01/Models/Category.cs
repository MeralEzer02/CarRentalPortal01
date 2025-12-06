using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal01.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı girmek zorunludur!")]
        public string Name { get; set; } = string.Empty;

        public ICollection<VehicleCategory>? VehicleCategories { get; set; }
    }
}
