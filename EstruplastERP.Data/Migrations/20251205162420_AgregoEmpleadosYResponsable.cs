using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregoEmpleadosYResponsable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Ancho",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Productos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Espesor",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Largo",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EmpleadoId",
                table: "Movimientos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Puesto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_EmpleadoId",
                table: "Movimientos",
                column: "EmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Empleados_EmpleadoId",
                table: "Movimientos",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Empleados_EmpleadoId",
                table: "Movimientos");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropIndex(
                name: "IX_Movimientos_EmpleadoId",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "Ancho",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Espesor",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Largo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EmpleadoId",
                table: "Movimientos");
        }
    }
}
