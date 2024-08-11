using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManager.Migrations
{
    /// <inheritdoc />
    public partial class AddSublocationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockSublocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Sublocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SublocationQuantity = table.Column<int>(type: "int", nullable: false),
                    StockProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockSublocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockSublocation_StockProduct_StockProductId",
                        column: x => x.StockProductId,
                        principalTable: "StockProduct",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockSublocation_StockProductId",
                table: "StockSublocation",
                column: "StockProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockSublocation");
        }
    }
}
