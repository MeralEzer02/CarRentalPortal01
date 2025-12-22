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

        // ARAÇ LİSTESİ
        public IActionResult Index(string search, string gearType, string fuelType, int page = 1)
        {
            var carsQuery = _context.Vehicles.Where(v => v.IsAvailable).AsQueryable();

            // 2. ARAMA FİLTRESİ
            if (!string.IsNullOrEmpty(search))
            {
                carsQuery = carsQuery.Where(x => x.Brand.Contains(search) || x.Model.Contains(search));
            }

            // 3. VİTES TİPİ FİLTRESİ
            if (!string.IsNullOrEmpty(gearType))
            {
                carsQuery = carsQuery.Where(x => x.GearType == gearType);
            }

            // 4. YAKIT TİPİ FİLTRESİ
            if (!string.IsNullOrEmpty(fuelType))
            {
                carsQuery = carsQuery.Where(x => x.FuelType == fuelType);
            }

            // 5. SAYFALAMA AYARLARI
            int pageSize = 6;
            int totalCars = carsQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalCars / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedCars = carsQuery
                .OrderByDescending(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.GearType = gearType;
            ViewBag.FuelType = fuelType;

            return View(pagedCars);
        }

        // ARAÇ DETAY SAYFASI
        public IActionResult CarDetail(int id)
        {
            // 1. Seçilen Aracı Bul
            var car = _context.Vehicles.FirstOrDefault(x => x.Id == id);

            if (car == null) return NotFound();

            var variants = new List<Vehicle>();

            if (!string.IsNullOrEmpty(car.VariantGroupId))
            {
                variants = _context.Vehicles
                    .Where(v => v.VariantGroupId == car.VariantGroupId && v.Id != id && v.IsAvailable)
                    .ToList();
            }

            ViewBag.Variants = variants;

            return View(car);
        }
    }
}