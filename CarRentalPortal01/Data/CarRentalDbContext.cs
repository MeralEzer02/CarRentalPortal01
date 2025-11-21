using Microsoft.EntityFrameworkCore;
using CarRentalPortal01.Models;

namespace CarRentalPortal01.Data
{
    public class CarRentalDbContext : DbContext
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options)
            : base(options)
        {
        }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<Rental>().ToTable("Rental");

            modelBuilder.Entity<Rental>()
                .Property(r => r.TotalPrice)
                .HasColumnType("money");

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.DailyRentalRate)
                .HasColumnType("money");

            base.OnModelCreating(modelBuilder);
            // Additional model configuration can go here
        }
    }
}