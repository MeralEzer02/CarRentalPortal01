namespace CarRentalPortal01.Models
{
    public class VehicleCategory
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}