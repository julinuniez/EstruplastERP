using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class LimpiezaCamposMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoteProveedor",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "PrecioTotal",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "PrecioUnitario",
                table: "Movimientos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9182));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9189));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9291));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9194));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9296));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9300));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9304));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9198));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9308));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9312));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9316));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9319));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9323));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9327));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9332));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9335));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9340));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9344));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9348));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9352));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9365));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9357));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9369));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9372));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9376));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9361));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9381));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9385));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9393));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9397));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9401));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9162));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9166));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9170));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9173));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9179));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 49, 37, 641, DateTimeKind.Local).AddTicks(9157));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoteProveedor",
                table: "Movimientos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioTotal",
                table: "Movimientos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioUnitario",
                table: "Movimientos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(810));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(925));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(937));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(930));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(941));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(945));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(949));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(934));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(952));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(956));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(960));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(963));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(967));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(974));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(977));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(981));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(985));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(989));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(992));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(998));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1009));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1001));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1013));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1016));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1021));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1005));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1025));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1028));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1032));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1036));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1040));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1044));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(791));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(795));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(798));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(801));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(805));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(787));
        }
    }
}
