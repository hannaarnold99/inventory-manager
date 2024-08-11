using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManager.Migrations
{
    /// <inheritdoc />
    public partial class AddingNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockSublocation_StockProduct_StockProductId",
                table: "StockSublocation");

            migrationBuilder.AlterColumn<int>(
                name: "StockProductId",
                table: "StockSublocation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StockSublocation_StockProduct_StockProductId",
                table: "StockSublocation",
                column: "StockProductId",
                principalTable: "StockProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockSublocation_StockProduct_StockProductId",
                table: "StockSublocation");

            migrationBuilder.AlterColumn<int>(
                name: "StockProductId",
                table: "StockSublocation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StockSublocation_StockProduct_StockProductId",
                table: "StockSublocation",
                column: "StockProductId",
                principalTable: "StockProduct",
                principalColumn: "Id");
        }
    }
}
