using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal01.Models

{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public decimal DailyRentalRate { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string Description { get; set; }
        public string ImageUrl { get; set; } 
    }
}
