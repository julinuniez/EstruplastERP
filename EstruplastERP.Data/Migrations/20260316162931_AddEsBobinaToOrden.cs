using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEsBobinaToOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsBobina",
                table: "Ordenes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2297));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2394));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2386));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2398));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2402));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2406));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2390));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2410));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2414));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2418));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2423));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2427));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2431));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2434));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2438));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2442));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2447));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2451));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2455));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2467));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2459));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2472));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2476));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2480));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2463));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2484));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2487));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2491));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2501));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2505));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2510));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2279));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2282));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2286));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2290));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2294));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 13, 29, 30, 153, DateTimeKind.Local).AddTicks(2274));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsBobina",
                table: "Ordenes");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4103));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4112));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4125));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4116));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4130));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4134));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4141));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4121));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4157));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4175));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4311));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4316));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4320));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4326));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4330));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4341));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4345));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4349));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4354));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4360));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4373));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4365));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4378));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4382));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4388));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4369));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4392));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4396));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4400));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4404));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4409));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4415));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4083));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4087));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4091));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4095));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4099));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 12, 21, 17, 531, DateTimeKind.Local).AddTicks(4077));
        }
    }
}
