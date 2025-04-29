using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slider> Slides { get; set; }
    }
}
