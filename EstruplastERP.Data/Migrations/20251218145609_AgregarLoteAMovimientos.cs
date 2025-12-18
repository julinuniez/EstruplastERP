using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarLoteAMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoteProveedor",
                table: "Movimientos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9575));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9580));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9583));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9587));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9590));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9594));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9597));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9601));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9604));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9607));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9610));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9613));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9615));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9618));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9622));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9627));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9633));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9636));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9713));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9716));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9719));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9722));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9725));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9727));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9730));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9734));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9737));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9739));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9742));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9745));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9750));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9754));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9759));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9764));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9767));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9771));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9775));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9779));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9784));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9788));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9792));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9796));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9799));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9803));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9807));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9812));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9816));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9820));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9824));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoteProveedor",
                table: "Movimientos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3155));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3160));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3163));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3167));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3171));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3174));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3182));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3187));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3190));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3194));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3197));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3200));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3279));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3283));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3286));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3291));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3294));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3297));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3300));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3303));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3306));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3309));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3312));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3315));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3317));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3321));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3324));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3327));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3330));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3332));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3336));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3342));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3346));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3350));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3356));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3360));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3364));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3368));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3372));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3376));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3381));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3385));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3389));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3458));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3462));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3467));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3472));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3476));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3480));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3484));
        }
    }
}
