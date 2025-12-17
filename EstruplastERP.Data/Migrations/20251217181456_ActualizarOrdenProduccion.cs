using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarOrdenProduccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ordenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    Turno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KilosEstimados = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ordenes_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumosOrdenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenProduccionId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    CantidadKilos = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumosOrdenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumosOrdenes_Ordenes_OrdenProduccionId",
                        column: x => x.OrdenProduccionId,
                        principalTable: "Ordenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2212));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2219));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2224));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2228));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2231));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2235));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2238));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2242));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2245));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2248));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2252));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2255));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2258));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2261));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2265));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2268));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2271));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2274));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2282));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2286));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2289));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2292));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2295));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2297));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2301));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2394));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2397));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2399));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2402));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2405));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2409));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2414));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2418));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2422));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2427));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2431));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2440));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2444));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2448));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2452));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2455));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2459));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2464));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2468));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2472));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2476));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2480));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2483));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2487));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 15, 14, 52, 983, DateTimeKind.Local).AddTicks(2491));

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosOrdenes_OrdenProduccionId",
                table: "ConsumosOrdenes",
                column: "OrdenProduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_ProductoId",
                table: "Ordenes",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumosOrdenes");

            migrationBuilder.DropTable(
                name: "Ordenes");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7186));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7191));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7197));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7201));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7204));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7207));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7211));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7215));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7219));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 11,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7221));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 12,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7224));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 13,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7227));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 14,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7230));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 30,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7233));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 31,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7235));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 32,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7240));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 33,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7242));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 34,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7245));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 50,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7322));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 51,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7325));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 52,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7328));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 53,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7331));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 54,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7333));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 60,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7336));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 61,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7339));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 70,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7341));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 80,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7345));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 81,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7347));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 90,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7350));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 99,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7352));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7356));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7362));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7366));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7370));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7374));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7378));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 110,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7383));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 111,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7387));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 112,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7391));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 113,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7395));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 120,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7399));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 121,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7403));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 122,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7407));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 123,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7411));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 130,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7414));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 131,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7418));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 132,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7423));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 140,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7427));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 141,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7431));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 142,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 17, 11, 23, 44, 556, DateTimeKind.Local).AddTicks(7490));
        }
    }
}
