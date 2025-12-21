using Microsoft.AspNetCore.Mvc;

namespace CarRentalPortal01.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}