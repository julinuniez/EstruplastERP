using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjustePrecisionProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "RemitoDetalles",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<bool>(
                name: "EsGenerico",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2348) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2354) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2357) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2505) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2516) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2519) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2523) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2526) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "EsGenerico", "FechaCreacion", "StockActual", "StockMinimo" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2530), 100m, 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "EsGenerico", "FechaCreacion", "StockActual", "StockMinimo" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2533), 100m, 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "EsGenerico", "FechaCreacion", "StockActual", "StockMinimo" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2536), 100m, 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "EsGenerico", "FechaCreacion", "StockActual", "StockMinimo" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2539), 100m, 0m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2542) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2545) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2548) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2551) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "EsGenerico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2554) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2556), "Material PAI Blanco (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2561), "Material PAI Negro (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2563), "Material PAI Color (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2566), "Material PAI Bicapa (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2569), "Material ABS Blanco (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2572), "Material ABS Negro/Color (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2574), "Material PP (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2577), "Material PEAD (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2580), "Material PEBD (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "EsGenerico", "FechaCreacion", "Nombre" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2583), "Material Biolam (A Medida)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "EsGenerico", "FechaCreacion", "PesoEspecifico" },
                values: new object[] { true, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2585), 0m });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 100, true, 0m, null, "STD-PAI-1000", null, null, false, false, true, 0m, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2588), null, 0m, "Lámina PAI Blanco 1000x1000 (Estándar)", 0m, 0m, 0m, 0m },
                    { 101, true, 0m, null, "STD-PAI-1220", null, null, false, false, true, 0m, new DateTime(2025, 12, 17, 9, 2, 28, 583, DateTimeKind.Local).AddTicks(2591), null, 0m, "Lámina PAI Negro 1220x2440 (Estándar)", 0m, 0m, 0m, 0m }
                });

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 300, 96m, 1, null, 100 },
                    { 301, 4m, 8, null, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 300);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 301);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DropColumn(
                name: "EsGenerico",
                table: "Productos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "RemitoDetalles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9500));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9505));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9508));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9512));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9517));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9522));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9624));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9627));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "FechaCreacion", "StockActual", "StockMinimo" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9650), 0m, 50m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "FechaCreacion", "StockActual", "StockMinimo" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9653), 0m, 50m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "FechaCreacion", "StockActual", "StockMinimo" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9656), 0m, 50m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "FechaCreacion", "StockActual", "StockMinimo" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9658), 0m, 50m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9634));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9636));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9639));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9642));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9647));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9661), "Material PAI Blanco" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9664), "Material PAI Negro" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9667), "Material PAI Color" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9669), "Material PAI Bicapa" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9672), "Material ABS Blanco" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9675), "Material ABS Negro/Color" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9678), "Material PP (Polipropileno)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9681), "Material PEAD (Alta Densidad)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9683), "Material PEBD (Baja Densidad)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9686), "Material Biolam (Compostable)" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9689), 1.38m });
        }
    }
}
