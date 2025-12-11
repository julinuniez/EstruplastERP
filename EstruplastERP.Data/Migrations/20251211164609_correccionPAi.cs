using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class correccionPAi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2954), 1.1m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2958), 1.1m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2962));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2965));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2968));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2971));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2974));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2978));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 46, 8, 470, DateTimeKind.Local).AddTicks(2982));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7493), 1.05m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7497), 1.05m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7501));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7504));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7507));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7511));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7514));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7517));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 11, 13, 26, 12, 307, DateTimeKind.Local).AddTicks(7521));
        }
    }
}
