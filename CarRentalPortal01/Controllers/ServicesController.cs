using Microsoft.AspNetCore.Mvc;

namespace CarRentalPortal01.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}