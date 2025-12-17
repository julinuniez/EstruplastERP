using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarClienteAProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7186) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7191) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7197) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7201) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7204) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7207) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7211) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7215) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7219) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7221) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7224) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7227) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7230) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7233) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7235) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7240) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7242) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7245) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7322) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7325) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7328) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7331) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7333) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7336) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7339) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7341) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7345) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7347) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7350) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7352) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7356) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7362) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7366) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7370) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7374) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7378) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7383) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7387) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7391) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7395) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7399) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7403) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7407) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7411) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7414) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7418) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7423) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7427) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7431) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "ClienteId", "FechaCreacion" },
                values: new object[] { null, new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7490) });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ClienteId",
                table: "Productos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Clientes_ClienteId",
                table: "Productos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Clientes_ClienteId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ClienteId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Productos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6282));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6288));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6291));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6297));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6300));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6304));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6308));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6311));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6315));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6318));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6321));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6324));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6429));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6433));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6436));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6439));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6442));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6446));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6449));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6452));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6455));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6459));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6462));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6464));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6467));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6470));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6473));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6475));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6479));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6482));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6486));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6491));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6495));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6500));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6504));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6509));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6512));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6516));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6520));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6524));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6528));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6532));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6536));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6540));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6544));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6600));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6605));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6612));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6616));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6620));
        }
    }
}
