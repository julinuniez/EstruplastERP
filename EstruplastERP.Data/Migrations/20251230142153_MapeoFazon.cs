using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class MapeoFazon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientesMaterialesFazon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    MaterialGenericoId = table.Column<int>(type: "int", nullable: false),
                    MaterialRealId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesMaterialesFazon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientesMaterialesFazon_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientesMaterialesFazon_Productos_MaterialGenericoId",
                        column: x => x.MaterialGenericoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientesMaterialesFazon_Productos_MaterialRealId",
                        column: x => x.MaterialRealId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1257));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1287));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1300));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1292));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1305));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1310));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1314));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1296));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1369));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1373));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1377));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1381));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1386));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1390));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1393));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1397));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1401));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1404));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1408));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1412));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1261));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1265));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1270));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1274));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1277));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1281));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1417));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1438));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1421));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1441));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1445));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1449));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1427));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1452));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1456));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1433));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1460));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1465));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1245));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1248));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1251));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1254));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 11, 21, 52, 153, DateTimeKind.Local).AddTicks(1240));

            migrationBuilder.CreateIndex(
                name: "IX_ClientesMaterialesFazon_ClienteId",
                table: "ClientesMaterialesFazon",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesMaterialesFazon_MaterialGenericoId",
                table: "ClientesMaterialesFazon",
                column: "MaterialGenericoId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesMaterialesFazon_MaterialRealId",
                table: "ClientesMaterialesFazon",
                column: "MaterialRealId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientesMaterialesFazon");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8599));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8624));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8636));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8629));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8640));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8645));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8649));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8633));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8653));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8656));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8660));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8664));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8669));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8673));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8677));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8680));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8684));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8770));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8775));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8779));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8602));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8605));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8608));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8611));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8614));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8619));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8783));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8800));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8787));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8804));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8807));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8811));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8791));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8816));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8820));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8795));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8824));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8828));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8432));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8436));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8593));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8596));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 30, 10, 31, 21, 556, DateTimeKind.Local).AddTicks(8429));
        }
    }
}
