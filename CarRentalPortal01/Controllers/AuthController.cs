using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using CarRentalPortal01.Data;
using CarRentalPortal01.Models;
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

            var userToCheck = _context.Users.FirstOrDefault(x => x.Email == email);

            if (userToCheck == null)
            {
                _toastNotification.AddInfoToastMessage("Böyle bir üyelik bulunamadı. Sizi kayıt sayfasına yönlendiriyoruz.");
                return RedirectToAction("Register");
            }

            if (userToCheck.PasswordHash != password)
            {
                _toastNotification.AddErrorToastMessage("Girdiğiniz şifre hatalı!");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userToCheck.Email),
                new Claim(ClaimTypes.Role, userToCheck.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            _toastNotification.AddSuccessToastMessage($"Hoş geldiniz, {userToCheck.UserName}");

            if (userToCheck.Role == 2) return RedirectToAction("Index", "Admin");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // --- KAYIT OL (REGISTER) ---

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string UserName, string Email, string PasswordHash, string PhoneNumber)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(PasswordHash))
            {
                _toastNotification.AddWarningToastMessage("Lütfen Ad Soyad, Email ve Şifre giriniz.");
                return View();
            }

            var checkUser = _context.Users.FirstOrDefault(x => x.Email == Email);
            if (checkUser != null)
            {
                _toastNotification.AddErrorToastMessage("Bu email zaten kayıtlı!");
                return View();
            }

            User newUser = new User();
            newUser.UserName = UserName;
            newUser.Email = Email;
            newUser.PasswordHash = PasswordHash;
            newUser.PhoneNumber = string.IsNullOrEmpty(PhoneNumber) ? "-" : PhoneNumber;

            newUser.Role = 0;
            newUser.Salary = 0;
            newUser.DriverLicenseImage = "-";

            _context.Users.Add(newUser);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Kayıt Başarılı! Giriş yapabilirsiniz.");

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