using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAgrupacionPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "NumeroPedidoCliente",
                table: "Ordenes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6939), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6966), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6979), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6971), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6983), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6986), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6990), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6975), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6994), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6998), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7003), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7008), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7012), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7016), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7020), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7025), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7085), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7091), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7094), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7098), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6942), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6946), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6949), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6953), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6956), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6960), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7103), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7120), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7107), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7124), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7128), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7132), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7111), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7136), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7141), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7116), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7146), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(7150), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6792), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6796), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6930), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6933), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "FechaCreacion", "TipoMaterial" },
                values: new object[] { new DateTime(2026, 1, 23, 11, 21, 15, 906, DateTimeKind.Local).AddTicks(6787), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoMaterial",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Ancho",
                table: "Ordenes");

            migrationBuilder.DropColumn(
                name: "Espesor",
                table: "Ordenes");

            migrationBuilder.DropColumn(
                name: "Largo",
                table: "Ordenes");

            migrationBuilder.DropColumn(
                name: "NumeroPedidoCliente",
                table: "Ordenes");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5800));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5829));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5842));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5833));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5846));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5850));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5853));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5838));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5857));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5863));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5867));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5940));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5944));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5948));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5954));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5959));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5963));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5967));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5970));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5974));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5804));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5807));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5811));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5814));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5817));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5820));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5979));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5998));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5985));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6001));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6005));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6011));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5990));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6015));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6019));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5994));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6023));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(6027));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5782));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5787));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5790));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5797));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 6, 15, 41, 37, 822, DateTimeKind.Local).AddTicks(5778));
        }
    }
}
