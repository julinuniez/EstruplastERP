using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProveedorProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProveedorId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2028), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2035), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2049), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2041), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2053), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2056), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2060), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2045), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2080), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2100), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2110), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2114), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2118), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2121), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2126), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2138), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2143), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2147), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2151), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2156), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2170), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2162), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2174), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2178), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2182), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2167), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2187), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2191), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2195), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2199), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2265), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2271), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(1922), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(1926), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(1929), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(1933), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(2025), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "FechaCreacion", "ProveedorId" },
                values: new object[] { new DateTime(2026, 4, 27, 12, 4, 30, 974, DateTimeKind.Local).AddTicks(1916), null });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ProveedorId",
                table: "Productos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5725));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5731));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5742));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5735));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5746));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5750));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5753));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5739));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5758));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5762));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5766));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5770));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5774));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5777));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5782));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5789));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5792));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5796));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5800));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5898));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5912));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5904));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5916));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5920));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5924));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5908));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5929));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5934));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5938));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5942));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5947));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5951));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5703));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5711));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5715));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5718));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5722));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 27, 10, 21, 3, 531, DateTimeKind.Local).AddTicks(5699));
        }
    }
}
