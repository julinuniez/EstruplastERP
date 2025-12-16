using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarRemitosConDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ClienteNombre",
                table: "Remitos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Remitos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Observacion",
                table: "Remitos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "RemitoDetalles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "Detalle",
                table: "RemitoDetalles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioUnitarioSnapshot",
                table: "RemitoDetalles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9500));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9505));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9508));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9512));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9517));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9522));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9624));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9627));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9650));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9653));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9656));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9658));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9634));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9636));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9639));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9642));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9647));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9661));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9664));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9667));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9669));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9672));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9675));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9678));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9681));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9683));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9686));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 16, 14, 49, 40, 321, DateTimeKind.Local).AddTicks(9689));

            migrationBuilder.CreateIndex(
                name: "IX_Remitos_ClienteId",
                table: "Remitos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Remitos_Clientes_ClienteId",
                table: "Remitos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Remitos_Clientes_ClienteId",
                table: "Remitos");

            migrationBuilder.DropIndex(
                name: "IX_Remitos_ClienteId",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "Observacion",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "Detalle",
                table: "RemitoDetalles");

            migrationBuilder.DropColumn(
                name: "PrecioUnitarioSnapshot",
                table: "RemitoDetalles");

            migrationBuilder.AlterColumn<string>(
                name: "ClienteNombre",
                table: "Remitos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Cantidad",
                table: "RemitoDetalles",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8044));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8073));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8077));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8081));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8084));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8088));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8091));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8094));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8098));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8117));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8120));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8123));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8126));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8102));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8105));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8108));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8111));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8114));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8130));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8132));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8135));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8138));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8141));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8144));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8147));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8150));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8153));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8156));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 15, 16, 53, 34, 294, DateTimeKind.Local).AddTicks(8158));
        }
    }
}
