using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductosRubros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rubro",
                table: "Productos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6692), "MATERIA PRIMA" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6717), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6729), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6721), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6733), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6738), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6742), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6725), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6746), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6749), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6754), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6758), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6763), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6767), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6771), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6775), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6863), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6867), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6872), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6876), "PRODUCTO TERMINADO" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6695), "MATERIA PRIMA RECUPERADA" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6699), "MATERIA PRIMA RECUPERADA" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6702), "MATERIA PRIMA RECUPERADA" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6704), "MATERIA PRIMA RECUPERADA" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6709), "MATERIA PRIMA RECUPERADA" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6711), "MATERIA PRIMA RECUPERADA" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6880), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6897), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6884), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6901), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6905), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6910), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6888), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6914), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6918), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6892), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6922), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6926), "SERVICIO FAZON" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "FechaCreacion", "Rubro" },
                values: new object[] { new DateTime(2025, 12, 30, 8, 41, 58, 160, DateTimeKind.Local).AddTicks(6687), "SERVICIO FAZON" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rubro",
                table: "Productos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 22,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8354));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 100,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8375));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 101,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8388));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 102,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8379));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 103,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8394));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 104,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8397));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 105,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8401));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 106,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8384));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 107,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8405));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 108,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8409));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 109,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8482));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 200,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8485));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 201,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8489));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 202,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8493));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 300,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8496));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 301,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8500));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 400,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8504));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 401,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8507));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 402,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8511));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 500,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8514));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 600,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8356));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 601,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8359));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 602,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8362));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 603,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8365));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 604,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8367));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 605,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8370));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 900,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8519));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 901,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8533));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 902,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8522));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 903,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8537));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 904,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8540));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 905,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8545));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 906,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8526));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 907,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8550));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 908,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8554));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 909,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8530));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 910,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8557));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 911,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8561));

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 999,
                column: "FechaCreacion",
                value: new DateTime(2025, 12, 29, 16, 50, 5, 216, DateTimeKind.Local).AddTicks(8349));
        }
    }
}
