using Microsoft.EntityFrameworkCore;

namespace exchangeRateApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Currency>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd(); // Configure auto-increment

            base.OnModelCreating(modelBuilder);
        }
    }
}
