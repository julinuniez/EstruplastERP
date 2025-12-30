using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class CargarProductosFazon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2662));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2689));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2702));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2694));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2706));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2711));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2715));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2697));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2838));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2842));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2846));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2850));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2855));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2859));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2862));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2866));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2871));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2875));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2880));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2883));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2665));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2669));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2672));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2675));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2679));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2684));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2888));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2905));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2892));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2909));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2913));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2916));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2896));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2920));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2924));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2900));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2928));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2932));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2641), "MATERIAL DE CLIENTE (GENÉRICO)" });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "ClienteId", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsFazon", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "EspesorMaximo", "EspesorMinimo", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "ProductoPadreId", "Rubro", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 990, true, 0m, null, null, "MP-FAZ-AI", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2647), null, 0m, "MP FAZÓN ALTO IMPACTO (BASE)", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 991, true, 0m, null, null, "MP-FAZ-ABS", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2652), null, 0m, "MP FAZÓN ABS (BASE)", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 992, true, 0m, null, null, "MP-FAZ-PP", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2656), null, 0m, "MP FAZÓN POLIPROPILENO (BASE)", 0.91m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 993, true, 0m, null, null, "MP-FAZ-PE", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2659), null, 0m, "MP FAZÓN PEAD/PEBD (BASE)", 0.96m, 0m, null, "SERVICIO FAZON", 0m, 0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993);

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 50, 100m, 999, 900 },
                    { 51, 100m, 999, 901 },
                    { 52, 98m, 999, 902 },
                    { 53, 2m, 22, 902 },
                    { 54, 98m, 999, 903 },
                    { 55, 2m, 22, 903 },
                    { 56, 100m, 999, 904 },
                    { 57, 100m, 999, 905 },
                    { 58, 100m, 999, 906 },
                    { 59, 100m, 999, 907 },
                    { 60, 100m, 999, 908 },
                    { 61, 100m, 999, 909 },
                    { 62, 100m, 999, 910 },
                    { 63, 100m, 999, 911 },
                    { 70, 100m, 602, 106 },
                    { 71, 100m, 602, 107 }
                });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6692));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6717));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6729));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6721));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6738));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6742));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6725));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6746));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6749));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6754));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6758));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6763));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6767));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6771));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6775));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6863));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6867));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6872));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6876));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6695));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6699));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6702));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6704));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6709));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6711));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6880));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6897));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6884));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6901));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6905));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6910));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6888));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6914));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6918));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6892));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6922));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6926));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "FechaCreacion", "Nombre" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6687), "MATERIAL DE CLIENTE (FAZÓN GENÉRICO)" });
        }
    }
}
