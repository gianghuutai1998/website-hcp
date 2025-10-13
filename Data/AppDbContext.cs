using Microsoft.EntityFrameworkCore;
using hcp.Models;

namespace hcp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ApiUser> ApiUsers { get; set; }
    }
}
