using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalPortal01.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenseImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "VehicleMaintenances",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(250,2)");

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseImage",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLicenseImage",
                table: "Users");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "VehicleMaintenances",
                type: "decimal(250,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
