using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFreonFazon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910);

            // 🚨 ELIMINADO EL ADDCOLUMN DE 'COLOR' PARA EVITAR EL ERROR DE DUPLICACIÓN

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3808), 1.1m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3813));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3826));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3817));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3830));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3833));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3837));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3822));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3841));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3845));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3849));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3853));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3858));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3861));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3865));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3948));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3953));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3956));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3961));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3966));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3979));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3983));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3986));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3991));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3975));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3995));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4000));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4003));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3789), 1.1m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3794), 1.1m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3797));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3801));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3785));

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "ClienteId", "CodigoSku", "Color", "EsFazon", "EsGenerico", "EsMateriaPrima", "EsPremezcla", "EsProductoTerminado", "EsScrap", "EspesorMaximo", "EspesorMinimo", "FamiliaId", "FechaCreacion", "Nombre", "PesoEspecifico", "PrecioCosto", "Rubro", "StockActual", "StockMinimo", "TipoMaterial" },
                values: new object[,]
                {
                    { 912, true, null, "FAZ-FREON-FIN", null, true, true, false, false, true, false, 0.90m, 0.40m, 60, new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4007), "LAMINADO A FAZON - RESISTENTE FREON FINO", 1.05m, 0m, "SERVICIO FAZON", 0m, 0m, null },
                    { 913, true, null, "FAZ-FREON-GRU", null, true, true, false, false, true, false, 0m, 0.90m, 60, new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4011), "LAMINADO A FAZON - RESISTENTE FREON GRUESO", 1.05m, 0m, "SERVICIO FAZON", 0m, 0m, null },
                    { 914, true, null, "FAZ-FREON-COL", "A Elección", true, true, false, false, true, false, 0m, 0.90m, 60, new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(4016), "LAMINADO A FAZON - RESISTENTE FREON COLOR", 1.05m, 0m, "SERVICIO FAZON", 0m, 0m, null },
                    { 994, true, null, "MP-FAZ-FREON", null, false, false, true, false, false, false, null, null, 60, new DateTime(2026, 3, 16, 11, 45, 54, 600, DateTimeKind.Local).AddTicks(3805), "MP FAZÓN FREON (BASE)", 1.1m, 0m, "SERVICIO FAZON", 0m, 0m, null }
                });

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 64, 100m, 994, 912 },
                    { 65, 100m, 994, 913 },
                    { 66, 100m, 994, 914 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Formulas",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 912);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 913);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 914);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 994);

            // 🚨 COMO BORRAMOS EL ADDCOLUMN ARRIBA, EL DROPCOLUMN ACÁ ABAJO ESTÁ BIEN QUE QUEDE 
            // POR SI ALGUNA VEZ QUERÉS DESHACER ESTO
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Ordenes");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1492), 1.20m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1499));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1512));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1504));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1516));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1520));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1524));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1508));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1529));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1533));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1705));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1710));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1714));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1718));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1722));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1726));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1731));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1735));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1739));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1748));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1768));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1752));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1772));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1776));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1780));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1760));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1785));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1789));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1797));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 990,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1475), 1.05m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 991,
                columns: new[] { "FechaCreacion", "PesoEspecifico" },
                values: new object[] { new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1481), 1.05m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 992,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1485));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 993,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1488));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1470));

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "ClienteId", "CodigoSku", "Color", "EsFazon", "EsGenerico", "EsMateriaPrima", "EsPremezcla", "EsProductoTerminado", "EsScrap", "EspesorMaximo", "EspesorMinimo", "FamiliaId", "FechaCreacion", "Nombre", "PesoEspecifico", "PrecioCosto", "Rubro", "StockActual", "StockMinimo", "TipoMaterial" },
                values: new object[,]
                {
                    { 500, true, null, "BIO-LAM", null, false, true, false, false, true, false, 0m, 0.90m, null, new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1743), "BIOPLASTICO", 1.25m, 0m, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 909, true, null, "FAZ-POLI-FIN", null, true, true, false, false, true, false, 0.90m, 0.40m, null, new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1764), "LAMINADO A FAZON - PEAD/PP/BIO FINO", 0.95m, 0m, "SERVICIO FAZON", 0m, 0m, null },
                    { 910, true, null, "FAZ-POLI-GRU", null, true, true, false, false, true, false, 0m, 0.90m, null, new DateTime(2026, 3, 6, 10, 59, 54, 423, DateTimeKind.Local).AddTicks(1793), "LAMINADO A FAZON - PEAD/PP/BIO GRUESO", 0.95m, 0m, "SERVICIO FAZON", 0m, 0m, null }
                });
        }
    }
}