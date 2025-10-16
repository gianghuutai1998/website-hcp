using Microsoft.AspNetCore.Mvc;
using hcp.Data;
using hcp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;
using hcp.Helpers;

namespace hcp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/login")]
        public IActionResult Login() {
            if (User?.Identity?.IsAuthenticated == true) {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || !PasswordHelper.VerifyPassword(password, user.Password ?? string.Empty)) {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu ";
                return View();
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "Auth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
