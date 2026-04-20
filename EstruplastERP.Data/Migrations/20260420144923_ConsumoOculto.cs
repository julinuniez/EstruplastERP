using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConsumoOculto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsCritico",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VisibleEnHoja",
                table: "ConsumosOrdenes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7428) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7434) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7438) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7450) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7455) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7459) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7443) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7463) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7467) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7471) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7474) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7479) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7483) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7487) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7491) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7495) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7581) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7585) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7590) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7602) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7594) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7605) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7609) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7617) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7598) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7621) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7626) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7630) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7634) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7639) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7643) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7409) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7413) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7416) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7419) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7422) });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "EsCritico", "FechaCreacion" },
                values: new object[] { false, new DateTime(2026, 4, 20, 11, 49, 21, 359, DateTimeKind.Local).AddTicks(7403) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsCritico",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "VisibleEnHoja",
                table: "ConsumosOrdenes");

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
        }
    }
}
