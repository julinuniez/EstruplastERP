using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class CargaProductosTerminados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7293));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7299));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7302));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7306));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7310));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7314));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7317));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7320));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7324));

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 50, true, 0m, null, "PT-PAI-B", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7509), null, 0m, "Lámina PAI Blanco Estándar", 0m, 0m, 0m, 500m },
                    { 51, true, 0m, null, "PT-PAI-N", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7512), null, 0m, "Lámina PAI Negro Estándar", 0m, 0m, 0m, 500m },
                    { 52, true, 0m, null, "PT-PAI-C", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7515), null, 0m, "Lámina PAI Color (Varios)", 0m, 0m, 0m, 200m },
                    { 53, true, 0m, null, "PT-PAI-BIC", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7518), null, 0m, "Lámina PAI Bicapa", 0m, 0m, 0m, 300m },
                    { 60, true, 0m, null, "PT-ABS-B", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7520), null, 0m, "Lámina ABS Blanco", 0m, 0m, 0m, 200m },
                    { 61, true, 0m, null, "PT-ABS-C", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7524), null, 0m, "Lámina ABS Negro / Color", 0m, 0m, 0m, 200m },
                    { 70, true, 0m, null, "PT-PP-N", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7527), null, 0m, "Lámina PP Natural/Blanco", 0m, 0m, 0m, 500m },
                    { 80, true, 0m, null, "PT-PEAD", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7530), null, 0m, "Lámina PEAD (Alta Densidad)", 0m, 0m, 0m, 500m },
                    { 81, true, 0m, null, "PT-PEBD", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7532), null, 0m, "Lámina PEBD (Baja Densidad)", 0m, 0m, 0m, 500m },
                    { 90, true, 0m, null, "PT-BIO", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7535), null, 0m, "Lámina Biolam (Compostable)", 0m, 0m, 0m, 100m },
                    { 95, true, 0m, null, "REV-PET", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7538), null, 0m, "Lámina PET Cristal (Reventa)", 0m, 0m, 0m, 1000m }
                });

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 200, 96m, 1, null, 50 },
                    { 201, 4m, 8, null, 50 },
                    { 202, 98m, 1, null, 51 },
                    { 203, 2m, 9, null, 51 },
                    { 204, 96m, 2, null, 60 },
                    { 205, 4m, 8, null, 60 },
                    { 206, 100m, 3, null, 70 },
                    { 207, 100m, 7, null, 90 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2954));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2958));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2962));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2965));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2968));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2971));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2974));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2978));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2982));
        }
    }
}
