using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arquetipo.Repository.Migrations
{
    public partial class CreatingPuntoVentaPatioIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Patio_NumeroPuntoVenta",
                table: "Patio",
                column: "NumeroPuntoVenta",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patio_NumeroPuntoVenta",
                table: "Patio");
        }
    }
}
