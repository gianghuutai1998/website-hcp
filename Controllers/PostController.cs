using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using hcp.Data;
using hcp.Models;

namespace hcp.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        public PostController(AppDbContext context)
        {
            _context = context;
        }

        // Xem danh sách bài đăng
        public IActionResult Index()
        {
            var posts = _context.Posts.OrderByDescending(p => p.CreatedAt).ToList();
            return View(posts);
        }

        // Xem chi tiết
        public IActionResult Details(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // Chỉ admin mới có thể thêm / sửa / xóa
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Create(Post post)
        {
            if (!ModelState.IsValid) return View(post);

            _context.Posts.Add(post);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
