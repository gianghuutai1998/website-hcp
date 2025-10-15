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

        public IActionResult Index()
        {
            var posts = _context.Posts.OrderByDescending(p => p.CreatedAt).ToList();
            return View(posts);
        }

        public IActionResult Details(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();
            return View(post);
        }

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
