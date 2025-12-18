using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CodigoSku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CodigoBarras = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Largo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Ancho = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Espesor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PesoEspecifico = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ProductoPadreId = table.Column<int>(type: "int", nullable: true),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    PrecioCosto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    EsGenerico = table.Column<bool>(type: "bit", nullable: false),
                    EsMateriaPrima = table.Column<bool>(type: "bit", nullable: false),
                    EsProductoTerminado = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Productos_ProductoPadreId",
                        column: x => x.ProductoPadreId,
                        principalTable: "Productos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Remitos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    NumeroRemito = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remitos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Formulas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoTerminadoId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formulas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formulas_Productos_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Formulas_Productos_ProductoTerminadoId",
                        column: x => x.ProductoTerminadoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Turno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    ProveedorId = table.Column<int>(type: "int", nullable: true),
                    NumeroRemito = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimientos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movimientos_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movimientos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movimientos_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ordenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_Ordenes_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ordenes_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductoTerminadoId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", precision: 18, scale: 2, nullable: false),
                    Kilos = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    Lote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Turno = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producciones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Producciones_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producciones_Productos_ProductoTerminadoId",
                        column: x => x.ProductoTerminadoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RemitoDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemitoId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecioUnitarioSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemitoDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemitoDetalles_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemitoDetalles_Remitos_RemitoId",
                        column: x => x.RemitoId,
                        principalTable: "Remitos",
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
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_ConsumosOrdenes_Productos_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "Productos",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "ClienteId", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "ProductoPadreId", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 1, true, 0m, null, null, "MP-PAI", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3155), null, 0m, "Poliestireno Alto Impacto (AI/PAI)", 1.1m, 0m, null, 1000m, 1000m },
                    { 2, true, 0m, null, null, "MP-ABS", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3160), null, 0m, "ABS (Acrilonitrilo Butadieno Estireno)", 1.1m, 0m, null, 1000m, 500m },
                    { 3, true, 0m, null, null, "MP-PP", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3163), null, 0m, "Polipropileno (PP)", 0.91m, 0m, null, 1000m, 1000m },
                    { 4, true, 0m, null, null, "MP-PEAD", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3167), null, 0m, "Polietileno Alta Densidad (PEAD)", 0.95m, 0m, null, 1000m, 1000m },
                    { 5, true, 0m, null, null, "MP-PEBD", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3171), null, 0m, "Polietileno Baja Densidad (PEBD)", 0.92m, 0m, null, 1000m, 1000m },
                    { 7, true, 0m, null, null, "MP-BIO", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3174), null, 0m, "Bioplástico Compostable (Biolam)", 1.25m, 0m, null, 0m, 200m },
                    { 8, true, 0m, null, null, "MP-MST-BLA", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3182), null, 0m, "Masterbatch Blanco (Titanio)", 1.80m, 0m, null, 500m, 100m },
                    { 9, true, 0m, null, null, "MP-MST-NEG", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3187), null, 0m, "Masterbatch Negro", 1.20m, 0m, null, 500m, 100m },
                    { 10, true, 0m, null, null, "MP-ADITIVO-GEN", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3190), null, 0m, "Aditivo Genérico", 0.95m, 0m, null, 0m, 50m },
                    { 11, true, 0m, null, null, "MP-MST-ROJ", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3194), null, 0m, "Masterbatch Rojo", 1.20m, 0m, null, 100m, 0m },
                    { 12, true, 0m, null, null, "MP-MST-AZU", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3197), null, 0m, "Masterbatch Azul", 1.20m, 0m, null, 100m, 0m },
                    { 13, true, 0m, null, null, "MP-MST-VER", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3200), null, 0m, "Masterbatch Verde", 1.20m, 0m, null, 100m, 0m },
                    { 14, true, 0m, null, null, "MP-MST-AMA", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3279), null, 0m, "Masterbatch Amarillo", 1.20m, 0m, null, 100m, 0m },
                    { 30, true, 0m, null, null, "MP-BRILLO", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3283), null, 0m, "Aditivo Brillo", 0.92m, 0m, null, 500m, 0m },
                    { 31, true, 0m, null, null, "MP-ESTEARATO", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3286), null, 0m, "Estearato de Zinc", 0.35m, 0m, null, 200m, 0m },
                    { 32, true, 0m, null, null, "MP-CARGA", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3291), null, 0m, "Carga Mineral (Carbonato)", 1.80m, 0m, null, 2000m, 0m },
                    { 33, true, 0m, null, null, "MP-UV", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3294), null, 0m, "Aditivo UV", 0.95m, 0m, null, 200m, 0m },
                    { 34, true, 0m, null, null, "MP-CAUCHO", null, null, false, true, false, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3297), null, 0m, "Aditivo Caucho", 0.94m, 0m, null, 200m, 0m },
                    { 50, true, 0m, null, null, "MAT-PAI-B", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3300), null, 0m, "Material PAI Blanco", 0m, 0m, null, 0m, 500m },
                    { 51, true, 0m, null, null, "MAT-PAI-N", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3303), null, 0m, "Material PAI Negro", 0m, 0m, null, 0m, 500m },
                    { 52, true, 0m, null, null, "MAT-PAI-C", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3306), null, 0m, "Material PAI Color", 0m, 0m, null, 0m, 200m },
                    { 53, true, 0m, null, null, "MAT-PAI-BIC", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3309), null, 0m, "Material PAI Bicapa", 0m, 0m, null, 0m, 500m },
                    { 54, true, 0m, null, null, "MAT-PAI-TRI", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3312), null, 0m, "Material PAI Tricapa", 0m, 0m, null, 0m, 500m },
                    { 60, true, 0m, null, null, "MAT-ABS-B", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3315), null, 0m, "Material ABS Blanco", 0m, 0m, null, 0m, 300m },
                    { 61, true, 0m, null, null, "MAT-ABS-C", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3317), null, 0m, "Material ABS Negro/Color", 0m, 0m, null, 0m, 300m },
                    { 70, true, 0m, null, null, "MAT-PP", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3321), null, 0m, "Material PP", 0m, 0m, null, 0m, 1000m },
                    { 80, true, 0m, null, null, "MAT-PEAD", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3324), null, 0m, "Material PEAD", 0m, 0m, null, 0m, 1000m },
                    { 81, true, 0m, null, null, "MAT-PEBD", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3327), null, 0m, "Material PEBD", 0m, 0m, null, 0m, 1000m },
                    { 90, true, 0m, null, null, "MAT-BIO", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3330), null, 0m, "Material Biolam", 0m, 0m, null, 0m, 200m },
                    { 99, true, 0m, null, null, "REV-PET", null, null, true, false, true, 0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3332), null, 0m, "Lámina PET (Reventa)", 0m, 0m, null, 0m, 500m }
                });

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 200, 96m, 1, 50 },
                    { 201, 4m, 8, 50 },
                    { 202, 98m, 1, 51 },
                    { 203, 2m, 9, 51 },
                    { 204, 100m, 3, 70 },
                    { 215, 100m, 1, 52 }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Ancho", "ClienteId", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "ProductoPadreId", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 100, true, 1000m, null, null, "STD-PAI-1000-05", null, null, false, false, true, 0.5m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3336), null, 2000m, "Lámina PAI Blanco 1000x2000x0.5 mm", 1.05m, 0m, 50, 0m, 0m },
                    { 101, true, 1000m, null, null, "STD-PAI-1000-10", null, null, false, false, true, 1.0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3342), null, 2000m, "Lámina PAI Blanco 1000x2000x1.0 mm", 1.05m, 0m, 50, 0m, 0m },
                    { 102, true, 1000m, null, null, "STD-PAI-1000-15", null, null, false, false, true, 1.5m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3346), null, 2000m, "Lámina PAI Blanco 1000x2000x1.5 mm", 1.05m, 0m, 50, 0m, 0m },
                    { 103, true, 1220m, null, null, "STD-PAI-1220-05", null, null, false, false, true, 0.5m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3350), null, 2440m, "Lámina PAI Blanco 1220x2440x0.5 mm", 1.05m, 0m, 50, 0m, 0m },
                    { 104, true, 1220m, null, null, "STD-PAI-1220-10", null, null, false, false, true, 1.0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3356), null, 2440m, "Lámina PAI Blanco 1220x2440x1.0 mm", 1.05m, 0m, 50, 0m, 0m },
                    { 105, true, 1220m, null, null, "STD-PAI-1220-15", null, null, false, false, true, 1.5m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3360), null, 2440m, "Lámina PAI Blanco 1220x2440x1.5 mm", 1.05m, 0m, 50, 0m, 0m },
                    { 110, true, 1000m, null, null, "STD-BIC-1000-10", null, null, false, false, true, 1.0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3364), null, 2000m, "Lámina PAI Bicapa 1000x2000x1.0 mm", 1.05m, 0m, 53, 0m, 0m },
                    { 111, true, 1000m, null, null, "STD-BIC-1000-15", null, null, false, false, true, 1.5m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3368), null, 2000m, "Lámina PAI Bicapa 1000x2000x1.5 mm", 1.05m, 0m, 53, 0m, 0m },
                    { 112, true, 1220m, null, null, "STD-BIC-1220-10", null, null, false, false, true, 1.0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3372), null, 2440m, "Lámina PAI Bicapa 1220x2440x1.0 mm", 1.05m, 0m, 53, 0m, 0m },
                    { 113, true, 1220m, null, null, "STD-BIC-1220-15", null, null, false, false, true, 1.5m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3376), null, 2440m, "Lámina PAI Bicapa 1220x2440x1.5 mm", 1.05m, 0m, 53, 0m, 0m },
                    { 120, true, 1000m, null, null, "STD-TRI-1000-10", null, null, false, false, true, 1.0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3381), null, 2000m, "Lámina PAI Tricapa 1000x2000x1.0 mm", 1.05m, 0m, 54, 0m, 0m },
                    { 121, true, 1000m, null, null, "STD-TRI-1000-15", null, null, false, false, true, 1.5m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3385), null, 2000m, "Lámina PAI Tricapa 1000x2000x1.5 mm", 1.05m, 0m, 54, 0m, 0m },
                    { 122, true, 1220m, null, null, "STD-TRI-1220-10", null, null, false, false, true, 1.0m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3389), null, 2440m, "Lámina PAI Tricapa 1220x2440x1.0 mm", 1.05m, 0m, 54, 0m, 0m },
                    { 123, true, 1220m, null, null, "STD-TRI-1220-15", null, null, false, false, true, 1.5m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3458), null, 2440m, "Lámina PAI Tricapa 1220x2440x1.5 mm", 1.05m, 0m, 54, 0m, 0m },
                    { 130, true, 1000m, null, null, "STD-PET-050", null, null, false, false, true, 0.50m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3462), null, 2000m, "Lámina PET 1000x2000x0.50 mm", 1.38m, 0m, 99, 0m, 0m },
                    { 131, true, 1000m, null, null, "STD-PET-080", null, null, false, false, true, 0.80m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3467), null, 2000m, "Lámina PET 1000x2000x0.80 mm", 1.38m, 0m, 99, 0m, 0m },
                    { 132, true, 1000m, null, null, "STD-PET-100", null, null, false, false, true, 1.00m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3472), null, 2000m, "Lámina PET 1000x2000x1.00 mm", 1.38m, 0m, 99, 0m, 0m },
                    { 140, true, 1000m, null, null, "STD-BIO-050", null, null, false, false, true, 0.50m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3476), null, 2000m, "Lámina BIOLAM 1000x2000x0.50 mm", 1.25m, 0m, 90, 0m, 0m },
                    { 141, true, 1000m, null, null, "STD-BIO-070", null, null, false, false, true, 0.70m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3480), null, 2000m, "Lámina BIOLAM 1000x2000x0.70 mm", 1.25m, 0m, 90, 0m, 0m },
                    { 142, true, 1000m, null, null, "STD-BIO-100", null, null, false, false, true, 1.00m, new DateTime(2025, 12, 18, 11, 44, 4, 795, DateTimeKind.Local).AddTicks(3484), null, 2000m, "Lámina BIOLAM 1000x2000x1.00 mm", 1.25m, 0m, 90, 0m, 0m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosOrdenes_MateriaPrimaId",
                table: "ConsumosOrdenes",
                column: "MateriaPrimaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosOrdenes_OrdenProduccionId",
                table: "ConsumosOrdenes",
                column: "OrdenProduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_MateriaPrimaId",
                table: "Formulas",
                column: "MateriaPrimaId");

            migrationBuilder.CreateIndex(
                name: "IX_Formulas_ProductoTerminadoId",
                table: "Formulas",
                column: "ProductoTerminadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_ClienteId",
                table: "Movimientos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_EmpleadoId",
                table: "Movimientos",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_ProductoId",
                table: "Movimientos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_ProveedorId",
                table: "Movimientos",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_EmpleadoId",
                table: "Ordenes",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_ProductoId",
                table: "Ordenes",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_ClienteId",
                table: "Producciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_EmpleadoId",
                table: "Producciones",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_ProductoTerminadoId",
                table: "Producciones",
                column: "ProductoTerminadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ClienteId",
                table: "Productos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProductoPadreId",
                table: "Productos",
                column: "ProductoPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_RemitoDetalles_ProductoId",
                table: "RemitoDetalles",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RemitoDetalles_RemitoId",
                table: "RemitoDetalles",
                column: "RemitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Remitos_ClienteId",
                table: "Remitos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpleadoId",
                table: "Usuarios",
                column: "EmpleadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumosOrdenes");

            migrationBuilder.DropTable(
                name: "Formulas");

            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "Producciones");

            migrationBuilder.DropTable(
                name: "RemitoDetalles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Ordenes");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Remitos");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
