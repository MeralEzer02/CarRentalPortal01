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

        // EKSİK OLAN KISIM BURASI: Context'i alan olarak tanımla
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
            var model = new DashboardViewModel
            {
                TotalVehicles = _vehicleRepository.GetAll().Count(),
                AvailableVehicles = _vehicleRepository.Find(v => v.IsAvailable).Count(),
                TotalUsers = _userRepository.GetAll().Count(),
                TotalRentals = _rentalRepository.GetAll().Count(),
                TotalEarnings = _rentalRepository.GetAll().Sum(r => r.TotalPrice),
                TotalCategories = _categoryRepository.GetAll().Count()
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

        public IActionResult VehicleList()
        {
            var vehicles = _vehicleRepository.GetAll("VehicleCategories.Category");

            return View(vehicles);
        }

        public IActionResult RentalList()
        {
            var rentals = _rentalRepository.GetAll(r => r.Vehicle);

            return View(rentals);
        }

        public IActionResult UserList()
        {
            var users = _userRepository.GetAll();
            return View(users);
        }

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
            if (ModelState.IsValid)
            {
                if (vm.User.UserId == 0)
                {
                    _userRepository.Add(vm.User);
                    _toastNotification.AddSuccessToastMessage("Kullanıcı eklendi.");
                }
                else
                {
                    _userRepository.Update(vm.User);
                    _toastNotification.AddSuccessToastMessage("Kullanıcı güncellendi.");
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
            Category category = new Category();
            if (id == null || id == 0) return View(category);

            category = _categoryRepository.GetById(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryUpsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _categoryRepository.Add(category);
                    _toastNotification.AddSuccessToastMessage("Kategori eklendi.");
                }
                else
                {
                    _categoryRepository.Update(category);
                    _toastNotification.AddSuccessToastMessage("Kategori güncellendi.");
                }
                _categoryRepository.Save();
                return RedirectToAction("CategoryList");
            }
            return View(category);
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
    }
}