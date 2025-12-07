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
        public List<string> CategoryNames { get; set; }
        public List<int> CategoryVehicleCounts { get; set; }
        public string MostPopularCar { get; set; } = "Veri Yok";
        public string TopEarnerCar { get; set; } = "Veri Yok"; 
    }
}