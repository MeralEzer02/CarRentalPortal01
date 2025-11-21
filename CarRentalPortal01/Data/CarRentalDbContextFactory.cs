using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CarRentalPortal01.Data
{
    public class CarRentalDbContextFactory : IDesignTimeDbContextFactory<CarRentalDbContext>
    {
        public CarRentalDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("CarRentalDbContext");

            var builder = new DbContextOptionsBuilder<CarRentalDbContext>();
            builder.UseSqlServer(connectionString);

            return new CarRentalDbContext(builder.Options);
        }
    }
}