using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalPortal01.Migrations
{
    /// <inheritdoc />
    public partial class mig12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- SADECE ARAÇ TABLOSUNA EKLENECEK YENİ SÜTUNLAR KALSIN ---

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GearType",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Kilometer",
                table: "Vehicle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantGroupId",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rental_Users_UserId",
                table: "Rental");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "GearType",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Kilometer",
                table: "Vehicle");     

            migrationBuilder.DropColumn(
                name: "VariantGroupId",
                table: "Vehicle");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Rental",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Rental",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Rental",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "Rental",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_Users_UserId",
                table: "Rental",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
