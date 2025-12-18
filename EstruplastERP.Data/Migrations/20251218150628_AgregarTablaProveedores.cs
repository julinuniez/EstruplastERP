using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contacto",
                table: "Proveedores");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Proveedores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RazonSocial",
                table: "Proveedores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Proveedores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Cuit",
                table: "Proveedores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Proveedores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ContactoNombre",
                table: "Proveedores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Proveedores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Proveedores",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(105));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(111));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(114));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(118));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(121));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(125));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(128));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(132));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(135));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(138));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(140));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(144));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(146));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(149));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(152));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(156));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(159));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(162));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(165));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(168));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(171));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(174));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(248));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(250));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(253));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(256));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(258));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(261));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(263));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(266));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(269));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(274));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(278));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(282));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(287));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(291));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(295));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(299));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(304));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(308));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(313));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(317));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(320));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(324));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(328));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(332));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(336));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(340));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(344));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 12, 6, 27, 24, DateTimeKind.Local).AddTicks(348));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "ContactoNombre",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Proveedores");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Proveedores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RazonSocial",
                table: "Proveedores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Proveedores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cuit",
                table: "Proveedores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contacto",
                table: "Proveedores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9575));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9580));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9583));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9587));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9590));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9594));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9597));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9601));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9604));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9607));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9610));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9613));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9615));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9618));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9622));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9627));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9630));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9633));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9636));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9713));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9716));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9719));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9722));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9725));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9727));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9730));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9734));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9737));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9739));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9742));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9745));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9750));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9754));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9759));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9764));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9767));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9771));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9775));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9779));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9784));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9788));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9792));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9796));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9799));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9803));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9807));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9812));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9816));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9820));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 18, 11, 56, 8, 152, DateTimeKind.Local).AddTicks(9824));
        }
    }
}
