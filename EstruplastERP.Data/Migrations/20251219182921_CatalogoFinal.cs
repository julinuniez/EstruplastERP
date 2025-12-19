using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogoFinal : Migration
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
                    RazonSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cuit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactoNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
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
                    EsFazon = table.Column<bool>(type: "bit", nullable: false),
                    Largo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Ancho = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Espesor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EspesorMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EspesorMaximo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
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
                    LoteProveedor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                columns: new[] { "Id", "Activo", "Ancho", "ClienteId", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsFazon", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "Espesor", "EspesorMaximo", "EspesorMinimo", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "ProductoPadreId", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 1, true, 0m, null, null, "MP-PAI", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3658), null, 0m, "Poliestireno Alto Impacto (AI/PAI)", 1.05m, 0m, null, 1000m, 1000m },
                    { 2, true, 0m, null, null, "MP-ABS", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3662), null, 0m, "ABS", 1.05m, 0m, null, 1000m, 500m },
                    { 3, true, 0m, null, null, "MP-PP", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3666), null, 0m, "Polipropileno (PP)", 0.91m, 0m, null, 1000m, 1000m },
                    { 4, true, 0m, null, null, "MP-PEAD", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3669), null, 0m, "Polietileno Alta Densidad (PEAD)", 0.96m, 0m, null, 1000m, 1000m },
                    { 5, true, 0m, null, null, "MP-PEBD", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3672), null, 0m, "Polietileno Baja Densidad (PEBD)", 0.92m, 0m, null, 1000m, 1000m },
                    { 6, true, 0m, null, null, "MP-BIO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3676), null, 0m, "Bioplástico", 1.25m, 0m, null, 0m, 200m },
                    { 7, true, 0m, null, null, "MP-TUTI-FINO", null, null, false, false, true, false, 0m, 0.90m, 0.40m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3681), null, 0m, "Tuti Fino", 1.05m, 0m, null, 0m, 0m },
                    { 8, true, 0m, null, null, "MP-TUTI-GRUESO", null, null, false, false, true, false, 0m, 5.00m, 0.91m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3685), null, 0m, "Tuti Grueso", 1.05m, 0m, null, 0m, 0m },
                    { 20, true, 0m, null, null, "MP-MB-BLA", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3688), null, 0m, "Masterbatch Blanco", 1.80m, 0m, null, 200m, 0m },
                    { 21, true, 0m, null, null, "MP-MB-NEG", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3690), null, 0m, "Masterbatch Negro", 1.20m, 0m, null, 200m, 0m },
                    { 22, true, 0m, null, null, "MP-MB-COL", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3693), null, 0m, "Masterbatch Color (Varios)", 1.20m, 0m, null, 200m, 0m },
                    { 23, true, 0m, null, null, "MP-CARGA", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3697), null, 0m, "Carga Mineral", 1.80m, 0m, null, 1000m, 0m },
                    { 24, true, 0m, null, null, "MP-UV", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3700), null, 0m, "Aditivo UV", 0.95m, 0m, null, 100m, 0m },
                    { 25, true, 0m, null, null, "MP-CAUCHO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3703), null, 0m, "Aditivo Caucho", 0.94m, 0m, null, 100m, 0m },
                    { 26, true, 0m, null, null, "MP-ESTEARATO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3705), null, 0m, "Estearato", 0.35m, 0m, null, 50m, 0m },
                    { 27, true, 0m, null, null, "MP-BRILLO", null, null, false, false, true, false, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3708), null, 0m, "Brillo", 1.00m, 0m, null, 50m, 0m },
                    { 100, true, 0m, null, null, "AI-FINO", null, null, false, true, false, true, 0m, 0.90m, 0.40m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3779), null, 0m, "A.I. FINO (0.40 - 0.90 mm)", 1.05m, 0m, null, 0m, 0m },
                    { 101, true, 0m, null, null, "AI-GRUESO", null, null, false, true, false, true, 0m, null, 0.91m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3852), null, 0m, "A.I. GRUESO", 1.05m, 0m, null, 0m, 0m },
                    { 102, true, 0m, null, null, "AI-FINO-COL", "A Elección", null, false, true, false, true, 0m, 0.90m, 0.40m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3858), null, 0m, "A.I. FINO COLOR (0.40 - 0.90 mm)", 1.05m, 0m, null, 0m, 0m },
                    { 103, true, 0m, null, null, "AI-GRUESO-COL", "A Elección", null, false, true, false, true, 0m, null, 0.91m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3862), null, 0m, "A.I. GRUESO COLOR", 1.05m, 0m, null, 0m, 0m },
                    { 104, true, 0m, null, null, "AI-BICAPA", null, null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3865), null, 0m, "A.I. BICAPA", 1.05m, 0m, null, 0m, 0m },
                    { 105, true, 0m, null, null, "AI-TRICAPA", null, null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3868), null, 0m, "A.I. TRICAPA", 1.05m, 0m, null, 0m, 0m },
                    { 106, true, 0m, null, null, "AI-TUTTI-FINO", null, null, false, true, false, true, 0m, 0.90m, 0.40m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3872), null, 0m, "A.I. TUTTI FINO (0.40 - 0.90 mm)", 1.05m, 0m, null, 0m, 0m },
                    { 107, true, 0m, null, null, "AI-TUTTI-GRUESO", null, null, false, true, false, true, 0m, null, 0.91m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3876), null, 0m, "A.I. TUTTI GRUESO", 1.05m, 0m, null, 0m, 0m },
                    { 108, true, 0m, null, null, "AI-FREON", null, null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3878), null, 0m, "A.I. RESISTENTE AL FREON", 1.05m, 0m, null, 0m, 0m },
                    { 109, true, 0m, null, null, "AI-FREON-COL", "A Elección", null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3881), null, 0m, "A.I. RESISTENTE AL FREON COLOR", 1.05m, 0m, null, 0m, 0m },
                    { 200, true, 0m, null, null, "ABS-BLA", "Blanco", null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3884), null, 0m, "ABS BLANCO", 1.05m, 0m, null, 0m, 0m },
                    { 201, true, 0m, null, null, "ABS-COL", "A Elección", null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3887), null, 0m, "ABS COLOR", 1.05m, 0m, null, 0m, 0m },
                    { 202, true, 0m, null, null, "ABS-GRUESO", null, null, false, true, false, true, 0m, null, 1.00m, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3890), null, 0m, "ABS GRUESO", 1.05m, 0m, null, 0m, 0m },
                    { 300, true, 0m, null, null, "PP-STD", null, null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3894), null, 0m, "PP (POLIPROPILENO)", 0.91m, 0m, null, 0m, 0m },
                    { 301, true, 0m, null, null, "PP-COL", "A Elección", null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3897), null, 0m, "PP (POLIPROPILENO) COLOR", 0.91m, 0m, null, 0m, 0m },
                    { 400, true, 0m, null, null, "PE-MIX", null, null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3899), null, 0m, "PEAD / PEBD", 0.94m, 0m, null, 0m, 0m },
                    { 401, true, 0m, null, null, "PEBD-GOF", null, null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3902), null, 0m, "PEBD GOFRADO", 0.92m, 0m, null, 0m, 0m },
                    { 402, true, 0m, null, null, "PEAD-BIC", null, null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3905), null, 0m, "PEAD BICAPA", 0.96m, 0m, null, 0m, 0m },
                    { 500, true, 0m, null, null, "BIO-LAM", null, null, false, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3908), null, 0m, "BIOPLASTICO", 1.25m, 0m, null, 0m, 0m },
                    { 900, true, 0m, null, null, "FAZON-AI-FIN", null, null, true, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3912), null, 0m, "LAMINADO A FAZON - A.I. FINO", 1.05m, 0m, null, 0m, 0m },
                    { 901, true, 0m, null, null, "FAZON-AI-GRU", null, null, true, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3914), null, 0m, "LAMINADO A FAZON - A.I. GRUESO", 1.05m, 0m, null, 0m, 0m },
                    { 902, true, 0m, null, null, "FAZON-AI-BIC", null, null, true, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3917), null, 0m, "LAMINADO A FAZON - A.I. BICAPA", 1.05m, 0m, null, 0m, 0m },
                    { 903, true, 0m, null, null, "FAZON-AI-TRI", null, null, true, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3920), null, 0m, "LAMINADO A FAZON - A.I. TRICAPA", 1.05m, 0m, null, 0m, 0m },
                    { 904, true, 0m, null, null, "FAZON-ABS", null, null, true, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3923), null, 0m, "LAMINADO A FAZON - ABS GRUESO", 1.05m, 0m, null, 0m, 0m },
                    { 905, true, 0m, null, null, "FAZON-POLI-FIN", null, null, true, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3926), null, 0m, "LAMINADO A FAZON - PEAD/PP/BIO FINO", 0.95m, 0m, null, 0m, 0m },
                    { 906, true, 0m, null, null, "FAZON-POLI-GRU", null, null, true, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3929), null, 0m, "LAMINADO A FAZON - PEAD/PP/BIO GRUESO", 0.95m, 0m, null, 0m, 0m },
                    { 907, true, 0m, null, null, "FAZON-PEAD-BIC", null, null, true, true, false, true, 0m, null, null, new DateTime(2025, 12, 19, 15, 29, 17, 124, DateTimeKind.Local).AddTicks(3932), null, 0m, "LAMINADO A FAZON - PEAD BICAPA", 0.96m, 0m, null, 0m, 0m }
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
                    { 9, 100m, 7, 106 }
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
