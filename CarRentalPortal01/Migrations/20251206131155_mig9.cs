using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalPortal01.Migrations
{
    /// <inheritdoc />
    public partial class mig9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Category_CategoryId",
                table: "Vehicle");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Vehicle",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Category_CategoryId",
                table: "Vehicle",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Category_CategoryId",
                table: "Vehicle");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Vehicle",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Category_CategoryId",
                table: "Vehicle",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
