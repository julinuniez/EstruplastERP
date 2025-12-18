using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class RangoEspesores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EspesorMaximo",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EspesorMinimo",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2388) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2393) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2399) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2403) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2406) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2410) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2413) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2416) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2419) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2422) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2425) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2428) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2431) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2434) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2436) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2439) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2442) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2445) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2448) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2451) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2454) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2457) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2526) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2529) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2532) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2534) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2538) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2540) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2543) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2546) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2549) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2555) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2559) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2563) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2568) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2572) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2576) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2580) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2583) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2587) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2591) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2595) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2599) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2603) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2607) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2610) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2615) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2619) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2664) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "EspesorMaximo", "EspesorMinimo", "FechaCreacion" },
                values: new object[] { null, null, new DateTime(2025, 12, 18, 14, 28, 13, 961, DateTimeKind.Local).AddTicks(2668) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EspesorMaximo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EspesorMinimo",
                table: "Productos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(105));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(111));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(114));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(118));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(121));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(125));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(128));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(132));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(135));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(138));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(140));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(144));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(146));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(149));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(152));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(156));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(159));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(162));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(165));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(168));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(171));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(174));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(248));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(250));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(253));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(256));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(258));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(261));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(263));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(266));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(269));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(274));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(278));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(282));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(287));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(291));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(295));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(299));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(304));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(308));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(313));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(317));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(320));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(324));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(328));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(332));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(336));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(340));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(344));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(348));
        }
    }
}
