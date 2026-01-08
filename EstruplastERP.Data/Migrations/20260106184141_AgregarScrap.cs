using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarScrap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsScrap",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5800) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5829) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5842) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5833) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5846) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5850) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5853) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5838) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5857) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5863) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5867) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5940) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5944) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5948) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5954) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5959) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5963) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5967) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5970) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5974) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5804) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5807) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5811) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5814) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5817) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5820) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5979) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5998) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5985) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6001) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6005) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6011) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5990) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6015) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6019) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5994) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6023) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6027) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5782) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5787) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5790) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5797) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "EsScrap", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5778) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsScrap",
                table: "Productos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2439));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2466));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2479));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2471));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2483));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2486));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2490));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2475));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2494));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2497));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2589));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2593));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2597));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2600));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2604));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2609));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2613));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2617));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2620));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2624));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2443));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2447));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2450));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2453));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2456));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2459));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2629));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2645));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2634));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2649));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2653));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2658));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2638));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2662));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2667));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2642));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2671));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2675));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2425));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2428));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2432));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2436));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 13, 13, 14, 511, DateTimeKind.Local).AddTicks(2421));
        }
    }
}
