using CarRentalPortal01.Models;
using CarRentalPortal01.Repositories;
using CarRentalPortal01.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalPortal01.Controllers
{
    public class AdminController : Controller
    {
        private readonly IGenericRepository<Vehicle> _vehicleRepository;
        public AdminController(IGenericRepository<Vehicle> vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VehicleList()
        {
            var vehicles = _vehicleRepository.GetAll();
            return View(vehicles);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            VehicleUpsertViewModel vm = new VehicleUpsertViewModel
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
        public IActionResult Upsert(VehicleUpsertViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Vehicle.Id == 0)
                {
                    _vehicleRepository.Add(vm.Vehicle);
                }
                else
                {
                    _vehicleRepository.Update(vm.Vehicle);
                }

                _vehicleRepository.Save();
                return RedirectToAction("VehicleList");
            }
            return View(vm);
        }

        public IActionResult Delete(int id)
        {
            var vehicle = _vehicleRepository.GetById(id);
            if (vehicle != null)
            {
                _vehicleRepository.Remove(vehicle);
                _vehicleRepository.Save();
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
