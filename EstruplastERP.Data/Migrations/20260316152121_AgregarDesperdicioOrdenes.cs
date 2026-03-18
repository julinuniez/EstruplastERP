using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDesperdicioOrdenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Desperdicio",
                table: "Ordenes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desperdicio",
                table: "Ordenes");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3808));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3813));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3826));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3817));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3830));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3833));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3837));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3822));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3841));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3845));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3849));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3853));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3858));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3861));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3865));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3948));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3953));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3956));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3961));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3966));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3979));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3983));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3986));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3991));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3975));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3995));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4000));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4003));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4007));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4011));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4016));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3789));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3794));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3797));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3801));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3805));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3785));
        }
    }
}
