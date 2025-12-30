using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregadaFamiliaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FamiliaId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9073) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9097) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9109) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9101) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9113) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9117) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9121) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9105) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9125) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9128) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9132) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9136) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9203) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9207) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9210) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9214) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9218) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9221) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9225) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9230) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9076) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9079) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9082) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9085) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9088) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9092) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9234) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9252) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9238) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9259) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9262) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9266) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9242) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9271) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9274) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9246) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9278) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9283) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9061) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9064) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9067) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9070) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "FamiliaId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 30, 13, 0, 44, 45, DateTimeKind.Local).AddTicks(9055) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FamiliaId",
                table: "Productos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1257));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1287));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1300));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1292));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1305));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1310));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1314));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1296));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1369));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1373));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1377));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1381));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1386));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1390));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1393));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1397));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1401));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1404));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1408));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1412));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1261));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1265));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1270));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1274));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1277));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1281));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1417));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1438));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1421));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1441));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1445));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1449));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1427));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1452));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1456));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1433));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1460));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1465));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1245));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1248));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1251));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1254));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1240));
        }
    }
}
