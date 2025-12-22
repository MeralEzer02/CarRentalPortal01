using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarRentalPortal01.Models;

namespace CarRentalPortal01.Data
{
    public class CarRentalDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options)
            : base(options)
        {
        }

        public DbSet<OldUser> OldUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<VehicleCategory> VehicleCategories { get; set; }
        public DbSet<VehicleMaintenance> VehicleMaintenances { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<Rental>().ToTable("Rental");

            modelBuilder.Entity<Rental>().Ignore("User");
            modelBuilder.Entity<Rental>().Ignore("OldUser");

            modelBuilder.Entity<Rental>()
                .Property(r => r.TotalPrice)
                .HasColumnType("money");

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.DailyRentalRate)
                .HasColumnType("money");

            modelBuilder.Entity<VehicleMaintenance>()
                .Property(p => p.Cost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<VehicleCategory>()
                .HasKey(vc => new { vc.VehicleId, vc.CategoryId });

            modelBuilder.Entity<VehicleCategory>()
                .HasOne(vc => vc.Vehicle)
                .WithMany(v => v.VehicleCategories)
                .HasForeignKey(vc => vc.VehicleId);

            modelBuilder.Entity<VehicleCategory>()
                .HasOne(vc => vc.Category)
                .WithMany(c => c.VehicleCategories)
                .HasForeignKey(vc => vc.CategoryId);

            modelBuilder.Entity<AppUser>().Property(u => u.Salary).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OldUser>().Property(u => u.Salary).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Expense>().Property(e => e.Amount).HasColumnType("decimal(18,2)");
        }
    }
}