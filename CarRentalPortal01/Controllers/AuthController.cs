using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using CarRentalPortal01.Data;
using CarRentalPortal01.ViewModels;
using NToastNotify;

namespace CarRentalPortal01.Controllers
{
    public class AuthController : Controller
    {
        private readonly CarRentalDbContext _context;
        private readonly IToastNotification _toastNotification;
        public AuthController(CarRentalDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(x => x.Email == model.Email && x.PasswordHash == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        // new Claim(ClaimTypes.Role, user.Role.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    _toastNotification.AddSuccessToastMessage($"Hoş geldiniz, {user.Email}");
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Email veya şifre hatalı!");
                    ModelState.AddModelError("", "Email veya şifre hatalı!");
                }
            }
            else
            {
                _toastNotification.AddWarningToastMessage("Lütfen alanları kontrol edin.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _toastNotification.AddInfoToastMessage("Başarıyla çıkış yapıldı.");

            return RedirectToAction("Login");
        }
    }
}