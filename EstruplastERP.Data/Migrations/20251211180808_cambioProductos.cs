using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class cambioProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                keyValue: 95);

            migrationBuilder.UpdateData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 204,
                columns: new[] { "Cantidad", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[] { 100m, 3, 70 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5029));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5033));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5036));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5040));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5044));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5142));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5145));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5148));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5151));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre" },
                values: new object[] { "MAT-PAI-B", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5226), "Material PAI Blanco" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre" },
                values: new object[] { "MAT-PAI-N", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5229), "Material PAI Negro" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre" },
                values: new object[] { "MAT-PAI-C", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5233), "Material PAI Color" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "MAT-PAI-BIC", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5236), "Material PAI Bicapa", 500m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "MAT-ABS-B", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5239), "Material ABS Blanco", 300m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "MAT-ABS-C", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5242), "Material ABS Negro/Color", 300m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "MAT-PP", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5245), "Material PP (Polipropileno)", 1000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "MAT-PEAD", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5247), "Material PEAD (Alta Densidad)", 1000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "MAT-PEBD", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5251), "Material PEBD (Baja Densidad)", 1000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "MAT-BIO", new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5254), "Material Biolam (Compostable)", 200m });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "StockActual", "StockMinimo" },
                values: new object[] { 99, true, 0m, null, "REV-PET", null, null, false, true, 0m, new DateTime(2025, 12, 11, 15, 8, 8, 21, DateTimeKind.Local).AddTicks(5257), null, 0m, "Lámina PET (Reventa)", 1.38m, 0m, 0m, 500m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.UpdateData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 204,
                columns: new[] { "Cantidad", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[] { 96m, 2, 60 });

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 205, 4m, 8, null, 60 },
                    { 206, 100m, 3, null, 70 },
                    { 207, 100m, 7, null, 90 }
                });

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

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre" },
                values: new object[] { "PT-PAI-B", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7509), "Lámina PAI Blanco Estándar" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre" },
                values: new object[] { "PT-PAI-N", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7512), "Lámina PAI Negro Estándar" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre" },
                values: new object[] { "PT-PAI-C", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7515), "Lámina PAI Color (Varios)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "PT-PAI-BIC", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7518), "Lámina PAI Bicapa", 300m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "PT-ABS-B", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7520), "Lámina ABS Blanco", 200m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "PT-ABS-C", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7524), "Lámina ABS Negro / Color", 200m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "PT-PP-N", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7527), "Lámina PP Natural/Blanco", 500m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "PT-PEAD", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7530), "Lámina PEAD (Alta Densidad)", 500m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "PT-PEBD", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7532), "Lámina PEBD (Baja Densidad)", 500m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "CodigoSku", "FechaCreacion", "Nombre", "StockMinimo" },
                values: new object[] { "PT-BIO", new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7535), "Lámina Biolam (Compostable)", 100m });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "StockActual", "StockMinimo" },
                values: new object[] { 95, true, 0m, null, "REV-PET", null, null, false, true, 0m, new DateTime(2025, 12, 11, 14, 57, 49, 703, DateTimeKind.Local).AddTicks(7538), null, 0m, "Lámina PET Cristal (Reventa)", 0m, 0m, 0m, 1000m });
        }
    }
}
