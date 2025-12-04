using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CodigoSku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CodigoBarras = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioCosto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsMateriaPrima = table.Column<bool>(type: "bit", nullable: false),
                    EsProductoTerminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
