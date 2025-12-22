using CarRentalPortal01.Models;
using CarRentalPortal01.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // FİLTRELEME PARAMETRELERİ
        public IActionResult Index(
            string search,
            int? categoryId,
            string brand,
            string color,
            string fuelType,
            string gearType,
            int? minYear,
            int? maxYear,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1)
        {
            // 1. TEMEL SORGUNUN BAŞLANGICI
            var query = _context.Vehicles
                .Include(v => v.VehicleCategories)
                .ThenInclude(vc => vc.Category)
                .Where(v => v.IsAvailable)
                .AsQueryable();

            // 2. FİLTRELERİ ADIM ADIM UYGULA

            // Arama (Genel)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Brand.Contains(search) || x.Model.Contains(search));
            }

            // Kategori (Araba / Motosiklet vb.)
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.VehicleCategories.Any(vc => vc.CategoryId == categoryId));
            }

            // Marka
            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(x => x.Brand == brand);
            }

            // Renk
            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(x => x.Color == color);
            }

            // Yakıt
            if (!string.IsNullOrEmpty(fuelType))
            {
                query = query.Where(x => x.FuelType == fuelType);
            }

            // Vites
            if (!string.IsNullOrEmpty(gearType))
            {
                query = query.Where(x => x.GearType == gearType);
            }

            // Yıl Aralığı
            if (minYear.HasValue) query = query.Where(x => x.Year >= minYear);
            if (maxYear.HasValue) query = query.Where(x => x.Year <= maxYear);

            // Fiyat Aralığı
            if (minPrice.HasValue) query = query.Where(x => x.DailyRentalRate >= minPrice);
            if (maxPrice.HasValue) query = query.Where(x => x.DailyRentalRate <= maxPrice);


            // 3. SİDEBAR (FİLTRE KUTULARI) İÇİN VERİLERİ HAZIRLA
            // Veritabanındaki mevcut seçenekleri çekiyoruz ki boş seçenek göstermeyelim.
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Vehicles.Where(x => x.IsAvailable).Select(x => x.Brand).Distinct().ToList();
            ViewBag.Colors = _context.Vehicles.Where(x => x.IsAvailable && x.Color != null).Select(x => x.Color).Distinct().ToList();

            // Seçili değerleri View'a geri gönder (Form sıfırlanmasın diye)
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentBrand = brand;
            ViewBag.CurrentColor = color;
            ViewBag.CurrentFuel = fuelType;
            ViewBag.CurrentGear = gearType;
            ViewBag.MinYear = minYear;
            ViewBag.MaxYear = maxYear;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;


            // 4. SAYFALAMA (PAGINATION)
            int pageSize = 9; // Grid yapısında 9 tane iyidir
            int totalCars = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalCars / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedCars = query
                .OrderByDescending(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(pagedCars);
        }
        // --- DETAYLAR BUTONU İÇİN ---
        public IActionResult Details(int id)
        {
            var car = _context.Vehicles.Find(id);
            if (car == null) return NotFound();

            if (!string.IsNullOrEmpty(car.VariantGroupId))
            {
                var variants = _context.Vehicles
                    .Where(v => v.VariantGroupId == car.VariantGroupId && v.Id != id)
                    .ToList();

                ViewBag.Variants = variants;
            }
            else
            {
                ViewBag.Variants = new List<Vehicle>();
            }

            return View("CarDetail", car);
        }

        // --- KİRALA BUTONU İÇİN ---
        [HttpGet]
        public IActionResult Rent(int id)
        {
            // 1. Kullanıcı Giriş Yapmış mı?
            if (!User.Identity.IsAuthenticated)
            {
                // Giriş yapmamışsa Login sayfasına at, dönüşte buraya gelsin
                return RedirectToAction("Login", "Auth", new { returnUrl = $"/Cars/Rent/{id}" });
            }

            // 2. Aracı Bul
            var car = _context.Vehicles.Find(id);
            if (car == null) return NotFound();

            // 3. Kiralama Sayfasına Git (View Modeline Aracı Gönder)
            // Eğer Rent.cshtml diye bir sayfan yoksa, oluşturman gerekecek.
            // Şimdilik test için Detay sayfasına yönlendirelim veya View döndürelim:
            return View(car);
        }

        // --- KİRALAMA İŞLEMİNİ TAMAMLAMA (POST) ---
        [HttpPost]
        public IActionResult Rent(int vehicleId, DateTime startDate, DateTime endDate)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");

            // Kullanıcının Email'ini Cookie'den al
            var userEmail = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user != null)
            {
                var vehicle = _context.Vehicles.Find(vehicleId);

                // Toplam Fiyat Hesapla
                var days = (endDate - startDate).Days;
                if (days <= 0) days = 1;
                var totalPrice = days * vehicle.DailyRentalRate;

                var rental = new Rental
                {
                    VehicleId = vehicleId,
                    UserId = user.UserId,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalPrice = totalPrice,
                };

                _context.Rentals.Add(rental);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home"); // Başarılıysa anasayfaya
            }

            return View(); // Hata varsa sayfada kal
        }
    }
}