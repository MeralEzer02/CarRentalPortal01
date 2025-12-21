using Microsoft.AspNetCore.Mvc;

namespace CarRentalPortal01.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}