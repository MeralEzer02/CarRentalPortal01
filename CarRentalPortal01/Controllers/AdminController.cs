using CarRentalPortal01.Models;
using CarRentalPortal01.Repositories;
using CarRentalPortal01.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NToastNotify;

namespace CarRentalPortal01.Controllers
{
    [Authorize(Roles = "1")]
    public class AdminController : Controller
    {
        private readonly IGenericRepository<Vehicle> _vehicleRepository;
        private readonly IGenericRepository<Rental> _rentalRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IToastNotification _toastNotification;
        public AdminController(IGenericRepository<Vehicle> vehicleRepository,
                               IGenericRepository<Rental> rentalRepository,
                               IGenericRepository<User> userRepository,
                               IToastNotification toastNotification)
        {
            _vehicleRepository = vehicleRepository;
            _rentalRepository = rentalRepository;
            _userRepository = userRepository;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalVehicles = _vehicleRepository.GetAll().Count(),
                AvailableVehicles = _vehicleRepository.Find(v => v.IsAvailable).Count(),
                TotalUsers = _userRepository.GetAll().Count(),
                TotalRentals = _rentalRepository.GetAll().Count(),
                TotalEarnings = _rentalRepository.GetAll().Sum(r => r.TotalPrice)
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
            var vehicles = _vehicleRepository.GetAll();
            return View(vehicles);
        }

        public IActionResult RentalList()
        {
            var rentals = _rentalRepository.GetAll();
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
            CarRentalPortal01.ViewModels.VehicleUpsertViewModel vm = new CarRentalPortal01.ViewModels.VehicleUpsertViewModel
            {
                Vehicle = new Vehicle()
            };

            if (id == null || id == 0)
            {
                return View(vm);
            }
            else
            {
                vm.Vehicle = _vehicleRepository.GetById(id.Value);
                if (vm.Vehicle == null)
                {
                    return NotFound();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CarRentalPortal01.ViewModels.VehicleUpsertViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Vehicle.Id == 0)
                {
                    _vehicleRepository.Add(vm.Vehicle);
                    _toastNotification.AddSuccessToastMessage("Araç başarıyla eklendi.");
                }
                else
                {
                    _vehicleRepository.Update(vm.Vehicle);
                    _toastNotification.AddSuccessToastMessage("Araç bilgileri güncellendi.");
                }

                _vehicleRepository.Save();
                return RedirectToAction("VehicleList");
            }

            _toastNotification.AddErrorToastMessage("Bir hata oluştu. Lütfen bilgileri kontrol edin.");
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
    }
}