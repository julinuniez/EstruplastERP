using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarOrdenProduccionAMovimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdenProduccionId",
                table: "Movimientos",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1646));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1653));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1666));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1657));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1669));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1673));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1677));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1661));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1681));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1684));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1785));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1789));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1793));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1797));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1801));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1805));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1809));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1813));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1816));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1821));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1832));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1825));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1836));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1840));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1844));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1829));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1848));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1851));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1855));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1861));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1865));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1870));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1628));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1631));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1634));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1638));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1643));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 9, 46, 19, 388, DateTimeKind.Local).AddTicks(1623));

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_OrdenProduccionId",
                table: "Movimientos",
                column: "OrdenProduccionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Ordenes_OrdenProduccionId",
                table: "Movimientos",
                column: "OrdenProduccionId",
                principalTable: "Ordenes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Ordenes_OrdenProduccionId",
                table: "Movimientos");

            migrationBuilder.DropIndex(
                name: "IX_Movimientos_OrdenProduccionId",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "OrdenProduccionId",
                table: "Movimientos");
        }
    }
}
