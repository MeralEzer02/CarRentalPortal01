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
        public IActionResult Index(string search, int page = 1)
        {
            var carsQuery = _context.Vehicles.Where(v => v.IsAvailable).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                carsQuery = carsQuery.Where(x => x.Brand.Contains(search) || x.Model.Contains(search));
            }

            int pageSize = 6; // Her sayfada kaç araç görünsün?
            int totalCars = carsQuery.Count(); // Toplam uygun araç sayısı
            int totalPages = (int)Math.Ceiling((double)totalCars / pageSize); // Toplam sayfa sayısı

            // Sayfa sınırlarını kontrol et
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

            return View(pagedCars);
        }        // ARAÇ DETAY SAYFASI
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