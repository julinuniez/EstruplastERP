using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFasonCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    { 59, 100m, 999, 907 }
                });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(4990));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(4997));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5000));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5004));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5007));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5011));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5016));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5020));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 20,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5024));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 21,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5028));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5030));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 23,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5033));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 24,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5037));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 25,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5040));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 26,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5042));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 27,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5048));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5189));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5193));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5199));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5204));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5208));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5210));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5214));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5218));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5221));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5225));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5228));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5230));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5234));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5236));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5239));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5241));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5244));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5246));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5249));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5252));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5256));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5259));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5262));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5381));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5384));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5387));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5390));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 16, 8, 49, 387, DateTimeKind.Local).AddTicks(5050));
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

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(658));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(663));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(670));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(674));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(799));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(803));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(808));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(812));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 20,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(816));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 21,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(819));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(822));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 23,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(825));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 24,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(828));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 25,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(832));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 26,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(853));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 27,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(872));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1550));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1555));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1560));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1577));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1582));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1585));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1588));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1592));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1595));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1744));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1748));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1751));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1755));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1757));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1761));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1765));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1767));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1770));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1774));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1777));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1780));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1783));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1786));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1788));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1791));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1794));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(1798));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 19, 15, 51, 50, 538, DateTimeKind.Local).AddTicks(874));
        }
    }
}
