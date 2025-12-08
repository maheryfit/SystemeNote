using Microsoft.EntityFrameworkCore;

namespace SystemeNote.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Define your DbSets here, for example:
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}