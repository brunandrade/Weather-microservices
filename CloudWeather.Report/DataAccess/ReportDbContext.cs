using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Reports.DataAccess
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext()
        {

        }
        public ReportDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Report> Report { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SankeCaseIdentityTableNames(modelBuilder);
        }

        private static void SankeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>(b =>
            {
                b.ToTable("report");
            });
        }
    }
}
