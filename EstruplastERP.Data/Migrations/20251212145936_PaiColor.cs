using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class PaiColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoId", "ProductoTerminadoId" },
                values: new object[] { 215, 100m, 1, null, 52 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8252));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8257));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8260));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8264));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8267));

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
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8275));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8278));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 11, 59, 34, 794, DateTimeKind.Local).AddTicks(8281));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 215);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6917));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6921));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6925));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6928));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6931));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6936));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6939));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6943));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(6946));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7029));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7034));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7037));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7040));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7042));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7046));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7049));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7073));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7076));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7079));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 12, 10, 32, 5, 277, DateTimeKind.Local).AddTicks(7082));
        }
    }
}
