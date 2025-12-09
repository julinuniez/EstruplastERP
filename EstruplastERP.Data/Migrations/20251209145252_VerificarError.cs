using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class VerificarError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "Formulas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_ProductoId",
                table: "Formulas",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Productos_ProductoId",
                table: "Formulas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Productos_ProductoId",
                table: "Formulas");

            migrationBuilder.DropIndex(
                name: "IX_Formulas_ProductoId",
                table: "Formulas");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "Formulas");
        }
    }
}
