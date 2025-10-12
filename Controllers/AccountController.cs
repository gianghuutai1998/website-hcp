using Microsoft.AspNetCore.Mvc;
using hcp.Data;
using hcp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;

namespace hcp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/login")]
        public IActionResult Login() => View();

        [HttpPost("/login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var hashedPassword = HashPassword(password);

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);
            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "Auth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Auth", principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Auth");
            return RedirectToAction("Index", "Home");
        }
    }
}
