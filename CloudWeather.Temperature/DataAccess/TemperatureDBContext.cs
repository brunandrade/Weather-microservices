using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temperature.DataAccess
{
    public class TemperatureDBContext : DbContext
    {
        public TemperatureDBContext()
        {

        }
        public TemperatureDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Temperature> Temperature { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SankeCaseIdentityTableNames(modelBuilder);
        }

        private static void SankeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>(b =>
            {
                b.ToTable("temperature");
            });
        }
    }
}
