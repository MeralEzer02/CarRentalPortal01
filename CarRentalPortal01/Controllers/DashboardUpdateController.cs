using CarRentalPortal01.Data;
using CarRentalPortal01.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CarRentalPortal01.Controllers
{
    public class DashboardUpdateController : Controller
    {
        private readonly IHubContext<CarHub> _hubContext;
        private readonly CarRentalDbContext _context;

        public DashboardUpdateController(IHubContext<CarHub> hubContext, CarRentalDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        // Bu linke gidildiğinde Dashboard'daki herkesin ekranı güncellenecek
        // Link: /DashboardUpdate/TriggerUpdate
        [HttpGet]
        public async Task<IActionResult> TriggerUpdate()
        {
            // 1. Güncel Verileri Hesapla
            int totalCars = _context.Vehicles.Count();

            int activeRentals = 0;
            if (_context.Rentals.Any())
                activeRentals = _context.Rentals.Count(r => r.StartDate <= DateTime.Now && r.EndDate >= DateTime.Now);

            int carsInMaintenance = 0;
            if (_context.VehicleMaintenances.Any())
                carsInMaintenance = _context.VehicleMaintenances.Count(m => !m.IsCompleted);

            int availableCars = totalCars - (activeRentals + carsInMaintenance);
            if (availableCars < 0) availableCars = 0;

            decimal totalEarnings = 0;
            if (_context.Rentals.Any())
                totalEarnings = _context.Rentals.Sum(r => r.TotalPrice);

            // 2. SignalR ile Tüm Bağlı İstemcilere (Adminlere) Gönder
            await _hubContext.Clients.All.SendAsync("ReceiveDashboardUpdate",
                totalCars,
                availableCars,
                activeRentals,
                totalEarnings.ToString("N0") // Binlik ayracıyla gönderiyoruz
            );

            return Ok("Sinyal Gönderildi! Dashboard güncellendi.");
        }
    }
}