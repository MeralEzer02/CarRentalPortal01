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
            return View();
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
    }
}