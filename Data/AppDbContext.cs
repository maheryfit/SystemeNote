using Microsoft.EntityFrameworkCore;
using SystemeNote.Models;

namespace SystemeNote.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Define your DbSets here, for example:
        public DbSet<Diplome> Diplomes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
    }
}