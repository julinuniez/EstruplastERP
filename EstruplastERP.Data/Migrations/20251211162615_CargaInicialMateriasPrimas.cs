using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class CargaInicialMateriasPrimas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 1, true, 0m, null, "MP-PAI", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7493), null, 0m, "Poliestireno Alto Impacto (AI/PAI)", 1.05m, 0m, 0m, 1000m },
                    { 2, true, 0m, null, "MP-ABS", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7497), null, 0m, "ABS (Acrilonitrilo Butadieno Estireno)", 1.05m, 0m, 0m, 500m },
                    { 3, true, 0m, null, "MP-PP", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7501), null, 0m, "Polipropileno (PP)", 0.91m, 0m, 0m, 1000m },
                    { 4, true, 0m, null, "MP-PEAD", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7504), null, 0m, "Polietileno Alta Densidad (PEAD)", 0.95m, 0m, 0m, 1000m },
                    { 5, true, 0m, null, "MP-PEBD", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7507), null, 0m, "Polietileno Baja Densidad (PEBD)", 0.92m, 0m, 0m, 1000m },
                    { 7, true, 0m, null, "MP-BIO", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7511), null, 0m, "Bioplástico Compostable (Biolam)", 1.25m, 0m, 0m, 200m },
                    { 8, true, 0m, null, "MP-MST-BLA", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7514), null, 0m, "Masterbatch Blanco (Titanio)", 1.80m, 0m, 0m, 100m },
                    { 9, true, 0m, null, "MP-MST-NEG", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7517), null, 0m, "Masterbatch Negro", 1.20m, 0m, 0m, 100m },
                    { 10, true, 0m, null, "MP-ADITIVO", null, null, true, false, 0m, new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7521), null, 0m, "Aditivo UV / Caucho", 0.95m, 0m, 0m, 50m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
