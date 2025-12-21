using CarRentalPortal01.Data;
using CarRentalPortal01.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CarRentalPortal01.Controllers
{
    public class CarsController : Controller
    {
        private readonly CarRentalDbContext _context;

        public CarsController(CarRentalDbContext context)
        {
            _context = context;
        }

        // ARAÇ LİSTESİ SAYFASI
        public IActionResult Index()
        {
            var allCars = _context.Vehicles
                .Where(v => v.IsAvailable)
                .OrderByDescending(v => v.Id)
                .ToList();

            return View(allCars);
        }
        // ARAÇ DETAY SAYFASI
        public IActionResult CarDetail(int id)
        {
            // 1. Seçilen Aracı Bul
            var car = _context.Vehicles.FirstOrDefault(x => x.Id == id);

            if (car == null) return NotFound();

            // 2. Varyantları (Diğer Renkleri) Bul
            // Kural: Grup Kodu aynı olsun AMA araç kendisi olmasın VE o araç da müsait olsun.
            var variants = new List<Vehicle>();

            if (!string.IsNullOrEmpty(car.VariantGroupId))
            {
                variants = _context.Vehicles
                    .Where(v => v.VariantGroupId == car.VariantGroupId && v.Id != id && v.IsAvailable)
                    .ToList();
            }

            // Varyantları ViewBag ile sayfaya taşıyoruz
            ViewBag.Variants = variants;

            return View(car);
        }
    }
}