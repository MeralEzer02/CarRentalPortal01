using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalPortal01.Migrations
{
    /// <inheritdoc />
    public partial class mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Vehicle",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CategoryId",
                table: "Vehicle",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Category_CategoryId",
                table: "Vehicle",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Category_CategoryId",
                table: "Vehicle");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Vehicle_CategoryId",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Vehicle");
        }
    }
}
