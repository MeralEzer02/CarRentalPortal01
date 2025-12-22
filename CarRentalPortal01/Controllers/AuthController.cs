using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using CarRentalPortal01.Data;
using CarRentalPortal01.Models; // User sınıfı buradan geliyor
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

        // --- GİRİŞ YAP (LOGIN) ---

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Zaten giriş yapmışsa Anasayfaya at
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _toastNotification.AddWarningToastMessage("Lütfen email ve şifrenizi girin.");
                return View();
            }

            // Kullanıcıyı Bul
            var user = _context.Users.FirstOrDefault(x => x.Email == email && x.PasswordHash == password);

            if (user != null)
            {
                // Kimlik Bilgilerini Oluştur (Cookie İçine Yazılacaklar)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                _toastNotification.AddSuccessToastMessage($"Hoş geldiniz, {user.UserName}");

                // YÖNLENDİRME MANTIĞI
                // 1. Admin ise Panele
                if (user.Role == 2)
                {
                    return RedirectToAction("Index", "Admin");
                }

                // 2. Eğer 'Kirala' butonundan geldiyse kaldığı yere dön
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // 3. Normal müşteri ise Anasayfaya
                return RedirectToAction("Index", "Home");
            }

            _toastNotification.AddErrorToastMessage("Email veya şifre hatalı!");
            return View();
        }

        // --- KAYIT OL (REGISTER) ---

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User p)
        {
            // Email kontrolü
            var checkUser = _context.Users.FirstOrDefault(x => x.Email == p.Email);
            if (checkUser != null)
            {
                _toastNotification.AddErrorToastMessage("Bu email zaten kayıtlı!");
                return View(p);
            }

            // Boş alan kontrolü
            if (string.IsNullOrEmpty(p.UserName) || string.IsNullOrEmpty(p.Email) || string.IsNullOrEmpty(p.PasswordHash))
            {
                _toastNotification.AddWarningToastMessage("Lütfen zorunlu alanları doldurun.");
                return View(p);
            }

            // Varsayılan değerler
            p.Role = 0; // Müşteri
            p.Salary = 0;
            p.DriverLicenseImage = "-";
            if (string.IsNullOrEmpty(p.PhoneNumber)) p.PhoneNumber = "-";

            _context.Users.Add(p);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Kayıt başarılı! Giriş yapabilirsiniz.");
            return RedirectToAction("Login");
        }

        // --- ÇIKIŞ YAP (LOGOUT) ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}