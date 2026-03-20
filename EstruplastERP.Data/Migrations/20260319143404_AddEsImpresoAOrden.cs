using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEsImpresoAOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoCorona",
                table: "Ordenes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "EsImpreso",
                table: "Ordenes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(810));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(925));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(937));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(930));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(941));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(945));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(949));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(934));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(952));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(956));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(960));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(963));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(967));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(974));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(977));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(981));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(985));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(989));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(992));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(998));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1009));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1001));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1013));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1016));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1021));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1005));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1025));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1028));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1032));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1036));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1040));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(1044));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(791));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(795));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(798));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(801));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(805));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 11, 34, 3, 401, DateTimeKind.Local).AddTicks(787));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsImpreso",
                table: "Ordenes");

            migrationBuilder.AlterColumn<string>(
                name: "TipoCorona",
                table: "Ordenes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(851));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(856));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(870));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(861));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(874));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(877));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(881));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(866));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(885));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(889));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(893));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(897));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1064));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1068));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1072));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1077));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1081));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1085));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1088));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1093));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1105));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1096));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1109));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1113));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1117));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1101));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1121));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1125));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1129));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1133));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1138));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(1142));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(834));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(838));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(841));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(844));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(848));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 10, 5, 29, 867, DateTimeKind.Local).AddTicks(828));
        }
    }
}
