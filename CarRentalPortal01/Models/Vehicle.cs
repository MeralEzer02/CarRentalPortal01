namespace CarRentalPortal01.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public decimal DailyRentalRate { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string Description { get; set; }
        public string ImageUrl { get; set; } 
    }
}
