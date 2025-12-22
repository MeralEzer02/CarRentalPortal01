using Microsoft.EntityFrameworkCore;
using CarRentalPortal01.Models;

namespace CarRentalPortal01.Data
{
    // IdentityDbContext YOK! Sadece DbContext var.
    public class CarRentalDbContext : DbContext
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options)
            : base(options)
        {
        }

        // ESKİ TABLOLARIMIZI EKSİKSİZ GERİ GETİRDİK
        public DbSet<User> Users { get; set; } // Manuel User tablosu geri geldi!
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
            // Tablo isimlerini tekil yapma ayarları
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<Rental>().ToTable("Rental");

            // Para birimi (Money) ayarları
            modelBuilder.Entity<Rental>()
                .Property(r => r.TotalPrice)
                .HasColumnType("money");

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.DailyRentalRate)
                .HasColumnType("money");

            modelBuilder.Entity<VehicleMaintenance>()
                .Property(p => p.Cost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(18,2)");

            // Çoka-Çok İlişki (Araç - Kategori)
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

            base.OnModelCreating(modelBuilder);
        }
    }
}