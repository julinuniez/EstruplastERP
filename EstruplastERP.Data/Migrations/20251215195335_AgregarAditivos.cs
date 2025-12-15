using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAditivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8044), 1000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8073), 1000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8077), 1000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8081), 1000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8084), 1000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8088));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8091), 500m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8094), 500m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre" },
                values: new object[] { "MP-ADITIVO-GEN", new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8098), "Aditivo Genérico" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8130));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8132));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8135));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8138));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8141));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8144));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8147));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8150));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8153));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8156));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8158));

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 11, true, 0m, null, "MP-MST-ROJ", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8117), null, 0m, "Masterbatch Rojo", 1.20m, 0m, 0m, 50m },
                    { 12, true, 0m, null, "MP-MST-AZU", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8120), null, 0m, "Masterbatch Azul", 1.20m, 0m, 0m, 50m },
                    { 13, true, 0m, null, "MP-MST-VER", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8123), null, 0m, "Masterbatch Verde", 1.20m, 0m, 0m, 50m },
                    { 14, true, 0m, null, "MP-MST-AMA", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8126), null, 0m, "Masterbatch Amarillo", 1.20m, 0m, 0m, 50m },
                    { 30, true, 0m, null, "MP-BRILLO", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8102), null, 0m, "Aditivo Brillo", 0.92m, 0m, 500m, 0m },
                    { 31, true, 0m, null, "MP-ESTEARATO", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8105), null, 0m, "Estearato de Zinc", 0.35m, 0m, 200m, 0m },
                    { 32, true, 0m, null, "MP-CARGA", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8108), null, 0m, "Carga Mineral (Carbonato)", 1.80m, 0m, 2000m, 0m },
                    { 33, true, 0m, null, "MP-UV", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8111), null, 0m, "Aditivo UV", 0.95m, 0m, 200m, 0m },
                    { 34, true, 0m, null, "MP-CAUCHO", null, null, true, false, 0m, new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8114), null, 0m, "Aditivo Caucho", 0.94m, 0m, 200m, 0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8252), 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8257), 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8260), 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8264), 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8267), 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8271));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8275), 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "FechaCreacion", "StockActual" },
                values: new object[] { new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8278), 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre" },
                values: new object[] { "MP-ADITIVO", new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8281), "Aditivo UV / Caucho" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8450));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8453));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8457));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8459));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8463));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8466));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8469));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8473));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8476));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8479));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8482));
        }
    }
}
