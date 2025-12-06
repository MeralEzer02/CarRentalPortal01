namespace CarRentalPortal01.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int TotalUsers { get; set; } 
        public int TotalRentals { get; set; }
        public decimal TotalEarnings { get; set; }
        public int TotalCategories { get; set; }
    }
}