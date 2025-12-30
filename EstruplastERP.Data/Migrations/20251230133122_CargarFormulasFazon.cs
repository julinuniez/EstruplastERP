using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class CargarFormulasFazon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 50, 100m, 990, 900 },
                    { 51, 100m, 990, 901 },
                    { 52, 98m, 990, 902 },
                    { 53, 2m, 22, 902 },
                    { 54, 98m, 990, 903 },
                    { 55, 2m, 22, 903 },
                    { 56, 100m, 990, 904 },
                    { 57, 100m, 990, 905 },
                    { 58, 100m, 990, 906 },
                    { 59, 100m, 990, 907 },
                    { 60, 100m, 991, 908 },
                    { 61, 100m, 992, 909 },
                    { 62, 100m, 992, 910 },
                    { 63, 100m, 993, 911 },
                    { 70, 100m, 602, 106 },
                    { 71, 100m, 602, 107 }
                });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8599));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8624));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8636));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8629));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8640));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8645));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8649));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8653));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8656));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8660));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8664));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8669));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8673));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8677));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8680));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8684));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8770));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8775));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8779));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8602));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8605));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8608));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8611));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8614));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8619));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8783));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8800));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8787));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8804));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8807));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8811));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8791));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8816));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8820));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8795));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8824));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8828));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8432));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8436));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8593));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8596));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8429));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2647));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2652));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2656));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2659));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 30, 31, 175, DateTimeKind.Local).AddTicks(2641));
        }
    }
}
