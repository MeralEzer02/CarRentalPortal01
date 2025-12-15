using CarRentalPortal01.Models;
using CarRentalPortal01.Repositories;
using CarRentalPortal01.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace CarRentalPortal01.Controllers
{
    [Authorize(Roles = "1")]
    public class AdminController : Controller
    {
        private readonly IGenericRepository<Vehicle> _vehicleRepository;
        private readonly IGenericRepository<Rental> _rentalRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IToastNotification _toastNotification;
        private readonly Data.CarRentalDbContext _context;

        public AdminController(IGenericRepository<Vehicle> vehicleRepository,
                               IGenericRepository<Rental> rentalRepository,
                               IGenericRepository<User> userRepository,
                               IGenericRepository<Category> categoryRepository,
                               IToastNotification toastNotification,
                               Data.CarRentalDbContext context)
        {
            _vehicleRepository = vehicleRepository;
            _rentalRepository = rentalRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _toastNotification = toastNotification;
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll(c => c.VehicleCategories);
            var allRentals = _rentalRepository.GetAll(r => r.Vehicle);
            var topRentedGroup = allRentals.GroupBy(r => r.Vehicle)
                                           .OrderByDescending(g => g.Count())
                                           .FirstOrDefault();

            var topEarnerGroup = allRentals.GroupBy(r => r.Vehicle)
                                           .OrderByDescending(g => g.Sum(x => x.TotalPrice))
                                           .FirstOrDefault();

            var model = new DashboardViewModel
            {
                TotalVehicles = _vehicleRepository.GetAll().Count(),
                AvailableVehicles = _vehicleRepository.Find(v => v.IsAvailable).Count(),
                TotalUsers = _userRepository.GetAll().Count(),
                TotalRentals = _rentalRepository.GetAll().Count(),
                TotalEarnings = _rentalRepository.GetAll().Sum(r => r.TotalPrice),

                TotalCategories = _categoryRepository.GetAll().Count(),

                CategoryNames = categories.Select(c => c.Name).ToList(),
                CategoryVehicleCounts = categories.Select(c => c.VehicleCategories != null ? c.VehicleCategories.Count : 0).ToList(),

                MostPopularCar = topRentedGroup != null ? $"{topRentedGroup.Key.Brand} {topRentedGroup.Key.Model}" : "Veri Yok",
                TopEarnerCar = topEarnerGroup != null ? $"{topEarnerGroup.Key.Brand} {topEarnerGroup.Key.Model}" : "Veri Yok"
            };

            return View(model);
        }

        public IActionResult Profile()
        {
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = _userRepository.Find(x => x.Email == userEmail).FirstOrDefault();

            return View(user);
        }

        // --- LİSTELEME İŞLEMLERİ ---

        public IActionResult VehicleList()
        {
            var vehicles = _vehicleRepository.GetAll("VehicleCategories.Category");
            return View(vehicles);
        }

        public IActionResult RentalList()
        {
            var rentals = _rentalRepository.GetAll("Vehicle", "User");
            return View(rentals);
        }

        public IActionResult UserList()
        {
            var users = _userRepository.GetAll("Rentals.Vehicle");
            return View(users);
        }

        // --- ARAÇ EKLEME / GÜNCELLEME ---

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            VehicleUpsertViewModel vm = new VehicleUpsertViewModel
            {
                Vehicle = new Vehicle(),
                CategoryList = _categoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                return View(vm);
            }
            else
            {
                var vehicle = _vehicleRepository.GetAll("VehicleCategories").FirstOrDefault(u => u.Id == id);

                if (vehicle == null) return NotFound();

                vm.Vehicle = vehicle;

                if (vehicle.VehicleCategories != null)
                {
                    vm.SelectedCategoryIds = vehicle.VehicleCategories.Select(c => c.CategoryId).ToArray();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(VehicleUpsertViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Vehicle.Id == 0)
                {
                    _vehicleRepository.Add(vm.Vehicle);
                    _toastNotification.AddSuccessToastMessage("Araç eklendi.");
                }
                else
                {
                    _vehicleRepository.Update(vm.Vehicle);
                    _toastNotification.AddSuccessToastMessage("Araç güncellendi.");
                }
                _vehicleRepository.Save();

                var existingCategories = _context.VehicleCategories.Where(x => x.VehicleId == vm.Vehicle.Id).ToList();
                _context.VehicleCategories.RemoveRange(existingCategories);

                if (vm.SelectedCategoryIds != null)
                {
                    foreach (var catId in vm.SelectedCategoryIds)
                    {
                        var newLink = new VehicleCategory
                        {
                            VehicleId = vm.Vehicle.Id,
                            CategoryId = catId
                        };
                        _context.VehicleCategories.Add(newLink);
                    }
                }
                _context.SaveChanges();

                return RedirectToAction("VehicleList");
            }

            vm.CategoryList = _categoryRepository.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

            _toastNotification.AddErrorToastMessage("Bilgileri kontrol edin.");
            return View(vm);
        }

        public IActionResult Delete(int id)
        {
            var vehicle = _vehicleRepository.GetById(id);
            if (vehicle != null)
            {
                _vehicleRepository.Remove(vehicle);
                _vehicleRepository.Save();
                _toastNotification.AddWarningToastMessage("Araç başarıyla silindi.");
            }
            return RedirectToAction("VehicleList");
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var vehicle = _vehicleRepository.GetById(id);
            if (vehicle == null)
            {
                return Json(new { success = false, message = "Araç bulunamadı" });
            }

            vehicle.IsAvailable = !vehicle.IsAvailable;

            _vehicleRepository.Update(vehicle);
            _vehicleRepository.Save();

            return Json(new { success = true, isAvailable = vehicle.IsAvailable });
        }

        // --- KULLANICI İŞLEMLERİ ---

        [HttpGet]
        public IActionResult UserUpsert(int? id)
        {
            CarRentalPortal01.ViewModels.UserUpsertViewModel vm = new CarRentalPortal01.ViewModels.UserUpsertViewModel
            {
                User = new User()
            };

            if (id == null || id == 0)
            {
                return View(vm);
            }
            else
            {
                vm.User = _userRepository.GetById(id.Value);
                if (vm.User == null)
                {
                    return NotFound();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserUpsert(CarRentalPortal01.ViewModels.UserUpsertViewModel vm)
        {
            if (vm.User.UserId != 0 && string.IsNullOrEmpty(vm.User.PasswordHash))
            {
                ModelState.Remove("User.Password");
            }

            if (ModelState.IsValid)
            {
                if (vm.User.UserId == 0)
                {
                    _userRepository.Add(vm.User);
                    _toastNotification.AddSuccessToastMessage("Kullanıcı eklendi.");
                }
                else
                {
                    var existingUser = _userRepository.GetById(vm.User.UserId);
                    if (existingUser != null)
                    {
                        existingUser.Email = vm.User.Email;
                        existingUser.Role = vm.User.Role;
                        existingUser.UserName = vm.User.UserName;
                        existingUser.PhoneNumber = vm.User.PhoneNumber;

                        if (!string.IsNullOrEmpty(vm.User.PasswordHash))
                        {
                            existingUser.PasswordHash = vm.User.PasswordHash;
                        }

                        _userRepository.Update(existingUser);
                        _toastNotification.AddSuccessToastMessage("Kullanıcı güncellendi.");
                    }
                }

                _userRepository.Save();
                return RedirectToAction("UserList");
            }

            _toastNotification.AddErrorToastMessage("Lütfen bilgileri kontrol edin.");
            return View(vm);
        }

        public IActionResult DeleteUser(int id)
        {
            var user = _userRepository.GetById(id);
            if (user != null)
            {
                if (User.Identity.Name == user.Email)
                {
                    _toastNotification.AddErrorToastMessage("Kendi hesabınızı silemezsiniz!");
                    return RedirectToAction("UserList");
                }

                _userRepository.Remove(user);
                _userRepository.Save();
                _toastNotification.AddWarningToastMessage("Kullanıcı silindi.");
            }
            return RedirectToAction("UserList");
        }

        // --- KATEGORİ İŞLEMLERİ ---

        public IActionResult CategoryList()
        {
            var categories = _categoryRepository.GetAll(c => c.VehicleCategories);
            return View(categories);
        }

        [HttpGet]
        public IActionResult CategoryUpsert(int? id)
        {
            CategoryUpsertViewModel vm = new CategoryUpsertViewModel
            {
                Category = new Category(),
                VehicleList = _vehicleRepository.GetAll().Select(v => new SelectListItem
                {
                    Text = $"{v.Brand} {v.Model} ({v.LicensePlate})",
                    Value = v.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                return View(vm);
            }
            else
            {
                var category = _categoryRepository.GetAll("VehicleCategories").FirstOrDefault(c => c.Id == id);
                if (category == null) return NotFound();

                vm.Category = category;

                if (category.VehicleCategories != null)
                {
                    vm.SelectedVehicleIds = category.VehicleCategories.Select(vc => vc.VehicleId).ToArray();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryUpsert(CategoryUpsertViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Category.Id == 0)
                {
                    _categoryRepository.Add(vm.Category);
                    _toastNotification.AddSuccessToastMessage("Kategori eklendi.");
                }
                else
                {
                    _categoryRepository.Update(vm.Category);
                    _toastNotification.AddSuccessToastMessage("Kategori güncellendi.");
                }
                _categoryRepository.Save();

                var existingLinks = _context.VehicleCategories.Where(x => x.CategoryId == vm.Category.Id).ToList();
                _context.VehicleCategories.RemoveRange(existingLinks);

                if (vm.SelectedVehicleIds != null)
                {
                    foreach (var vehicleId in vm.SelectedVehicleIds)
                    {
                        _context.VehicleCategories.Add(new VehicleCategory
                        {
                            CategoryId = vm.Category.Id,
                            VehicleId = vehicleId
                        });
                    }
                }
                _context.SaveChanges();

                return RedirectToAction("CategoryList");
            }

            vm.VehicleList = _vehicleRepository.GetAll().Select(v => new SelectListItem
            {
                Text = $"{v.Brand} {v.Model}",
                Value = v.Id.ToString()
            });

            return View(vm);
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _categoryRepository.GetAll(c => c.VehicleCategories)
                                              .FirstOrDefault(c => c.Id == id);

            if (category != null)
            {
                if (category.VehicleCategories != null && category.VehicleCategories.Count > 0)
                {
                    _toastNotification.AddErrorToastMessage($"Bu kategori silinemez! İçinde {category.VehicleCategories.Count} adet araç kayıtlı.");
                    return RedirectToAction("CategoryList");
                }

                _categoryRepository.Remove(category);
                _categoryRepository.Save();
                _toastNotification.AddWarningToastMessage("Kategori başarıyla silindi.");
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Kategori bulunamadı.");
            }

            return RedirectToAction("CategoryList");
        }


        // --- KİRALAMA İŞLEMLERİ ---
        [HttpGet]
        public IActionResult RentalUpsert(int? id)
        {
            RentalUpsertViewModel vm = new RentalUpsertViewModel
            {
                Rental = new Rental(),

                UserList = _userRepository.GetAll().Select(u => new SelectListItem
                {
                    Text = $"{u.UserName} ({u.Email})",
                    Value = u.UserId.ToString()
                }),

                VehicleList = _vehicleRepository.GetAll().Select(v => new SelectListItem
                {
                    Text = $"{v.Brand} {v.Model} - {v.LicensePlate} ({v.DailyRentalRate:C0}/Gün)",
                    Value = v.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                vm.Rental.StartDate = DateTime.Now;
                vm.Rental.EndDate = DateTime.Now.AddDays(1);
                return View(vm);
            }
            else
            {
                vm.Rental = _rentalRepository.GetById(id.Value);
                if (vm.Rental == null) return NotFound();
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RentalUpsert(RentalUpsertViewModel vm)
        {
            ModelState.Remove("Rental.User");
            ModelState.Remove("Rental.Vehicle");
            ModelState.Remove("Rental.TotalPrice");

            if (vm.Rental.UserId == 0 || vm.Rental.VehicleId == 0)
            {
                _toastNotification.AddErrorToastMessage("Lütfen Kullanıcı ve Araç seçiniz.");
            }

            if (vm.Rental.StartDate >= vm.Rental.EndDate)
            {
                ModelState.AddModelError("Rental.EndDate", "Bitiş tarihi başlangıçtan büyük olmalı.");
            }

            var conflictingRental = _rentalRepository.Find(r =>
                r.VehicleId == vm.Rental.VehicleId &&      
                r.Id != vm.Rental.Id &&                    
                (vm.Rental.StartDate < r.EndDate && vm.Rental.EndDate > r.StartDate)
            ).FirstOrDefault();

            if (conflictingRental != null)
            {
                var conflictMsg = $"Bu araç seçilen tarihlerde dolu! ({conflictingRental.StartDate:dd.MM.yyyy} - {conflictingRental.EndDate:dd.MM.yyyy} arasında kirada)";

                ModelState.AddModelError("", conflictMsg);
                _toastNotification.AddErrorToastMessage("Araç bu tarihlerde müsait değil!");
            }


            if (ModelState.IsValid)
            {
                var selectedVehicle = _vehicleRepository.GetById(vm.Rental.VehicleId);
                if (selectedVehicle != null)
                {
                    var days = (vm.Rental.EndDate - vm.Rental.StartDate).TotalDays;
                    if (days < 1) days = 1;
                    vm.Rental.TotalPrice = (decimal)days * selectedVehicle.DailyRentalRate;
                }

                if (vm.Rental.Id == 0)
                {
                    _rentalRepository.Add(vm.Rental);
                    _toastNotification.AddSuccessToastMessage("Kiralama oluşturuldu.");
                }
                else
                {
                    _rentalRepository.Update(vm.Rental);
                    _toastNotification.AddSuccessToastMessage("Kiralama güncellendi.");
                }

                _rentalRepository.Save();
                return RedirectToAction("RentalList");
            }

            vm.UserList = _userRepository.GetAll().Select(u => new SelectListItem
            {
                Text = $"{u.UserName} ({u.Email})",
                Value = u.UserId.ToString()
            });

            vm.VehicleList = _vehicleRepository.GetAll().Select(v => new SelectListItem
            {
                Text = $"{v.Brand} {v.Model} - {v.LicensePlate} ({v.DailyRentalRate:C0}/Gün)",
                Value = v.Id.ToString()
            });

            return View(vm);
        }

        // --- TAKVİM (SCHEDULER) İŞLEMLERİ ---
        public IActionResult Scheduler()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCalendarEvents()
        {
            var rentals = _rentalRepository.GetAll("Vehicle", "User");

            var events = rentals.Select(r => new
            {
                id = r.Id,

                title = $"{r.Vehicle?.Brand} {r.Vehicle?.Model} - {(r.User != null ? (!string.IsNullOrEmpty(r.User.UserName) ? r.User.UserName : r.User.Email) : "Bilinmeyen Kullanıcı")}",

                start = r.StartDate.ToString("yyyy-MM-dd"),

                end = r.EndDate.AddDays(1).ToString("yyyy-MM-dd"),

                color = "#4e73df",
                textColor = "#ffffff"
            });

            return Json(events);
        }

        // --- RAPORLAMA İŞLEMLERİ ---
        public IActionResult Reports()
        {
            var rentals = _rentalRepository.GetAll(r => r.Vehicle);
            var allVehicles = _vehicleRepository.GetAll();

            var model = new ReportsViewModel
            {
                VehicleReports = new List<VehicleReportDto>()
            };

            foreach (var vehicle in allVehicles)
            {
                var vehicleRentals = rentals.Where(r => r.VehicleId == vehicle.Id).ToList();

                model.VehicleReports.Add(new VehicleReportDto
                {
                    VehicleName = $"{vehicle.Brand} {vehicle.Model}",
                    Plate = vehicle.LicensePlate,
                    RentalCount = vehicleRentals.Count(),
                    TotalIncome = vehicleRentals.Sum(r => r.TotalPrice)
                });
            }

            var topRented = model.VehicleReports.OrderByDescending(x => x.RentalCount).FirstOrDefault();
            if (topRented != null)
            {
                model.MostRentedVehicle = allVehicles.FirstOrDefault(v => v.LicensePlate == topRented.Plate);
                model.MostRentedCount = topRented.RentalCount;
            }

            var topEarner = model.VehicleReports.OrderByDescending(x => x.TotalIncome).FirstOrDefault();
            if (topEarner != null)
            {
                model.TopEarnerVehicle = allVehicles.FirstOrDefault(v => v.LicensePlate == topEarner.Plate);
                model.TopEarnerAmount = topEarner.TotalIncome;
            }

            return View(model);
        }


        // --- STOK DURUMU / ENVANTER RAPORU ---

        public IActionResult Inventory()
        {
            var allVehicles = _vehicleRepository.GetAll();

            var inventory = allVehicles
                .GroupBy(v => new { v.Brand, v.Model, v.Color, v.FuelType, v.GearType })
                .Select(g => new InventoryItemViewModel
                {
                    Brand = g.Key.Brand,
                    Model = g.Key.Model,
                    Color = g.Key.Color ?? "Belirtilmemiş",
                    FuelType = g.Key.FuelType ?? "-",
                    GearType = g.Key.GearType ?? "-",

                    TotalCount = g.Count(),

                    AvailableCount = g.Count(v => v.IsAvailable),

                    RentedCount = g.Count(v => !v.IsAvailable)
                })
                .OrderByDescending(x => x.TotalCount)
                .ToList();

            return View(inventory);
        }
    }
}