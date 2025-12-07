using CarRentalPortal01.Models;

namespace CarRentalPortal01.ViewModels
{
    public class ReportsViewModel
    {
        public Vehicle? MostRentedVehicle { get; set; }
        public int MostRentedCount { get; set; }

        public Vehicle? TopEarnerVehicle { get; set; }
        public decimal TopEarnerAmount { get; set; }

        public List<VehicleReportDto> VehicleReports { get; set; }
    }

    public class VehicleReportDto
    {
        public string VehicleName { get; set; } = string.Empty;
        public string Plate { get; set; } = string.Empty;
        public int RentalCount { get; set; }
        public decimal TotalIncome { get; set; }
    }
}