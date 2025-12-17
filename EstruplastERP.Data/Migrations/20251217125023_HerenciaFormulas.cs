using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class HerenciaFormulas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formulas_Productos_ProductoId",
                table: "Formulas");

            migrationBuilder.DropIndex(
                name: "IX_Formulas_ProductoId",
                table: "Formulas");

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 300);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 301);

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "Formulas");

            migrationBuilder.AddColumn<int>(
                name: "ProductoPadreId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6282), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },  
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6288), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6291), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6297), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6300), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6304), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6308), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6311), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6315), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6318), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6321), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6324), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6429), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6433), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6436), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6439), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6442), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6446), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6449), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6452), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6455), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6459), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6464), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6467), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6470), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6473), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6475), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6479), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6482), null });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6486), 50 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6491), 50 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6495), 50 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6500), 50 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6504), 50 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6509), 50 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6512), 53 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6516), 53 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6520), 53 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6524), 53 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6528), 54 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6532), 54 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6536), 54 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6540), 54 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6544), 99 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6600), 99 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6605), 99 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6612), 90 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6616), 90 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "FechaCreacion", "ProductoPadreId" },
                values: new object[] { new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6620), 90 });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "ProductoPadreId", "StockActual", "StockMinimo" },
                values: new object[] { 54, true, 0m, null, "MAT-PAI-TRI", null, null, true, false, true, 0m, new DateTime(2025, 12, 17, 9, 50, 22, 219, DateTimeKind.Local).AddTicks(6462), null, 0m, "Material PAI Tricapa", 0m, 0m, null, 0m, 500m });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProductoPadreId",
                table: "Productos",
                column: "ProductoPadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Productos_ProductoPadreId",
                table: "Productos",
                column: "ProductoPadreId",
                principalTable: "Productos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Productos_ProductoPadreId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ProductoPadreId",
                table: "Productos");

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DropColumn(
                name: "ProductoPadreId",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "Formulas",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 200,
                column: "ProductoId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 201,
                column: "ProductoId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 202,
                column: "ProductoId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 203,
                column: "ProductoId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 204,
                column: "ProductoId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 215,
                column: "ProductoId",
                value: null);

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 300, 96m, 1, null, 100 },
                    { 301, 4m, 8, null, 100 }
                });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4496));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4502));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4505));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4508));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4512));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4517));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4521));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4524));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4527));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4531));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4534));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4537));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4540));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4543));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4546));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4549));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4552));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4555));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4558));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4562));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4565));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4568));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4570));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4573));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4575));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4578));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4581));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4662));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4665));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4668));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4678));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4681));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4685));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4689));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4693));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4696));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4700));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4705));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4709));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4713));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4717));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4720));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4724));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4729));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4733));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4737));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4740));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4744));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 9, 30, 8, 667, DateTimeKind.Local).AddTicks(4748));

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_ProductoId",
                table: "Formulas",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Formulas_Productos_ProductoId",
                table: "Formulas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }
    }
}
