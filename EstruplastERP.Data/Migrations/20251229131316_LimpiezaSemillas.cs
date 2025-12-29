using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class LimpiezaSemillas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5159));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5302));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5165));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5306));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5309));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5312));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5168));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5317));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5321));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5327));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5331));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5334));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5338));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5342));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5346));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5350));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5353));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5357));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5360));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5365));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5380));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5369));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5384));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5388));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5391));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5373));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5395));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5399));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5376));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5484));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 10, 13, 15, 422, DateTimeKind.Local).AddTicks(5487));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2152));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2165));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2157));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2293));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2297));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2301));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2161));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2305));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2310));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2314));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2319));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2323));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2326));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2330));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2336));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2339));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2343));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2347));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2351));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2419));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2440));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2424));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2444));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2448));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2452));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2428));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2539));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2543));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2436));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2547));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2551));

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "ClienteId", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsFazon", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "EspesorMaximo", "EspesorMinimo", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "ProductoPadreId", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 1, true, 0m, null, null, "MP-PAI", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2008), null, 0m, "Poliestireno Alto Impacto (AI/PAI)", 1.05m, 0m, null, 1000m, 1000m },
                    { 2, true, 0m, null, null, "MP-ABS", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2013), null, 0m, "ABS", 1.05m, 0m, null, 1000m, 500m },
                    { 3, true, 0m, null, null, "MP-PP", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2017), null, 0m, "Polipropileno (PP)", 0.91m, 0m, null, 1000m, 1000m },
                    { 4, true, 0m, null, null, "MP-PEAD", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2020), null, 0m, "Polietileno Alta Densidad (PEAD)", 0.96m, 0m, null, 1000m, 1000m },
                    { 5, true, 0m, null, null, "MP-PEBD", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2024), null, 0m, "Polietileno Baja Densidad (PEBD)", 0.92m, 0m, null, 1000m, 1000m },
                    { 6, true, 0m, null, null, "MP-BIO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2027), null, 0m, "Bioplástico", 1.25m, 0m, null, 0m, 200m },
                    { 7, true, 0m, null, null, "MP-TUTI-FINO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2030), null, 0m, "Tuti Fino", 1.05m, 0m, null, 0m, 0m },
                    { 8, true, 0m, null, null, "MP-TUTI-GRUESO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2033), null, 0m, "Tuti Grueso", 1.05m, 0m, null, 0m, 0m },
                    { 20, true, 0m, null, null, "MP-MB-BLA", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2036), null, 0m, "Masterbatch Blanco", 1.80m, 0m, null, 200m, 0m },
                    { 21, true, 0m, null, null, "MP-MB-NEG", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2040), null, 0m, "Masterbatch Negro", 1.20m, 0m, null, 200m, 0m },
                    { 22, true, 0m, null, null, "MP-MB-COL", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2046), null, 0m, "Masterbatch Color (Varios)", 1.20m, 0m, null, 200m, 0m },
                    { 23, true, 0m, null, null, "MP-CARGA", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2049), null, 0m, "Carga Mineral", 1.80m, 0m, null, 1000m, 0m },
                    { 24, true, 0m, null, null, "MP-UV", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2053), null, 0m, "Aditivo UV", 0.95m, 0m, null, 100m, 0m },
                    { 25, true, 0m, null, null, "MP-CAUCHO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2056), null, 0m, "Aditivo Caucho", 0.94m, 0m, null, 100m, 0m },
                    { 26, true, 0m, null, null, "MP-ESTEARATO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2059), null, 0m, "Estearato", 0.35m, 0m, null, 50m, 0m },
                    { 27, true, 0m, null, null, "MP-BRILLO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2062), null, 0m, "Brillo", 1.00m, 0m, null, 50m, 0m },
                    { 999, true, 0m, null, null, "MP-FAZON-GEN", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 29, 9, 22, 37, 750, DateTimeKind.Local).AddTicks(2065), null, 0m, "MATERIAL DE CLIENTE (FAZÓN)", 1.00m, 0m, null, 0m, 0m }
                });

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 1, 96m, 1, 100 },
                    { 2, 4m, 20, 100 },
                    { 3, 98m, 1, 102 },
                    { 4, 2m, 22, 102 },
                    { 5, 98m, 2, 200 },
                    { 6, 2m, 20, 200 },
                    { 7, 100m, 3, 300 },
                    { 8, 100m, 4, 400 },
                    { 9, 100m, 7, 106 },
                    { 50, 100m, 999, 900 },
                    { 51, 100m, 999, 901 },
                    { 52, 98m, 999, 902 },
                    { 53, 2m, 22, 902 },
                    { 54, 98m, 999, 903 },
                    { 55, 2m, 22, 903 },
                    { 56, 100m, 999, 904 },
                    { 57, 100m, 999, 905 },
                    { 58, 100m, 999, 906 },
                    { 59, 100m, 999, 907 },
                    { 60, 100m, 999, 908 },
                    { 61, 100m, 999, 909 },
                    { 62, 100m, 999, 910 },
                    { 63, 100m, 999, 911 }
                });
        }
    }
}
