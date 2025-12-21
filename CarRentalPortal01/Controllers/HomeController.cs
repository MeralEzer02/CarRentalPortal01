using Microsoft.AspNetCore.Mvc;

namespace CarRentalPortal01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}