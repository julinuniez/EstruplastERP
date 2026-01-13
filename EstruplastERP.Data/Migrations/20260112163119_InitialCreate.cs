using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EstruplastERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    Rubro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsFazon = table.Column<bool>(type: "bit", nullable: false),
                    EsScrap = table.Column<bool>(type: "bit", nullable: false),
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
                    FamiliaId = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_Ordenes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
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
                columns: new[] { "Id", "Activo", "Ancho", "ClienteId", "CodigoBarras", "CodigoSku", "Color", "Descripcion", "EsFazon", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "EsScrap", "Espesor", "EspesorMaximo", "EspesorMinimo", "FamiliaId", "FechaCreacion", "ImagenUrl", "Largo", "Nombre", "PesoEspecifico", "PrecioCosto", "ProductoPadreId", "Rubro", "StockActual", "StockMinimo" },
                values: new object[,]
                {
                    { 22, true, 0m, null, null, "MP-MB-COL", null, null, false, false, true, false, false, 0m, null, null, 50, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2276), null, 0m, "Masterbatch Color (Varios)", 1.20m, 0m, null, "MATERIA PRIMA", 0m, 0m },
                    { 100, true, 0m, null, null, "AI-FINO", null, null, false, true, false, true, false, 0m, 0.90m, 0.40m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2402), null, 0m, "A.I. FINO (0.40 - 0.90 mm)", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 101, true, 0m, null, null, "AI-GRUESO", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2442), null, 0m, "A.I. GRUESO", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 102, true, 0m, null, null, "AI-FINO-COL", "A Elección", null, false, true, false, true, false, 0m, 0.90m, 0.40m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2407), null, 0m, "A.I. FINO COLOR", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 103, true, 0m, null, null, "AI-GRUESO-COL", "A Elección", null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2446), null, 0m, "A.I. GRUESO COLOR", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 104, true, 0m, null, null, "AI-BICAPA", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2449), null, 0m, "A.I. BICAPA", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 105, true, 0m, null, null, "AI-TRICAPA", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2453), null, 0m, "A.I. TRICAPA", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 106, true, 0m, null, null, "AI-TUTTI-FINO", null, null, false, true, false, true, false, 0m, 0.90m, 0.40m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2423), null, 0m, "A.I. TUTTI FINO", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 107, true, 0m, null, null, "AI-TUTTI-GRUESO", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2464), null, 0m, "A.I. TUTTI GRUESO", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 108, true, 0m, null, null, "AI-FREON", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2469), null, 0m, "A.I. RESISTENTE AL FREON", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 109, true, 0m, null, null, "AI-FREON-COL", "A Elección", null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2473), null, 0m, "A.I. RESISTENTE AL FREON COLOR", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 200, true, 0m, null, null, "ABS-BLA", "Blanco", null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2477), null, 0m, "ABS BLANCO", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 201, true, 0m, null, null, "ABS-COL", "A Elección", null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2481), null, 0m, "ABS COLOR", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 202, true, 0m, null, null, "ABS-GRUESO", null, null, false, true, false, true, false, 0m, 0m, 1.00m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2487), null, 0m, "ABS GRUESO (Min 1mm)", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 300, true, 0m, null, null, "PP-STD", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2491), null, 0m, "PP (POLIPROPILENO)", 0.91m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 301, true, 0m, null, null, "PP-COL", "A Elección", null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2504), null, 0m, "PP COLOR", 0.91m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 400, true, 0m, null, null, "PE-MIX", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2508), null, 0m, "PEAD / PEBD", 0.94m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 401, true, 0m, null, null, "PEBD-GOF", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2511), null, 0m, "PEBD GOFRADO", 0.92m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 402, true, 0m, null, null, "PEAD-BIC", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2515), null, 0m, "PEAD BICAPA", 0.96m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 500, true, 0m, null, null, "BIO-LAM", null, null, false, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2520), null, 0m, "BIOPLASTICO", 1.25m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m },
                    { 600, true, 0m, null, null, "REC-AI-BLA", null, null, false, false, true, false, false, 0m, null, null, 10, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2280), null, 0m, "SCRAP A.I. BLANCO", 1.05m, 0m, null, "MATERIA PRIMA RECUPERADA", 0m, 0m },
                    { 601, true, 0m, null, null, "REC-AI-NEG", null, null, false, false, true, false, false, 0m, null, null, 10, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2286), null, 0m, "SCRAP A.I. NEGRO", 1.05m, 0m, null, "MATERIA PRIMA RECUPERADA", 0m, 0m },
                    { 602, true, 0m, null, null, "REC-AI-TUT", null, null, false, false, true, false, false, 0m, null, null, 10, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2289), null, 0m, "A.I. TUTTI (MEZCLA)", 1.05m, 0m, null, "MATERIA PRIMA RECUPERADA", 0m, 0m },
                    { 603, true, 0m, null, null, "REC-PP", null, null, false, false, true, false, false, 0m, null, null, 30, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2293), null, 0m, "SCRAP PP", 0.91m, 0m, null, "MATERIA PRIMA RECUPERADA", 0m, 0m },
                    { 604, true, 0m, null, null, "REC-PEAD", null, null, false, false, true, false, false, 0m, null, null, 40, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2391), null, 0m, "SCRAP PEAD", 0.96m, 0m, null, "MATERIA PRIMA RECUPERADA", 0m, 0m },
                    { 605, true, 0m, null, null, "REC-ABS", null, null, false, false, true, false, false, 0m, null, null, 20, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2395), null, 0m, "SCRAP ABS", 1.05m, 0m, null, "MATERIA PRIMA RECUPERADA", 0m, 0m },
                    { 900, true, 0m, null, null, "FAZ-AI-FIN", null, null, true, true, false, true, false, 0m, 0.90m, 0.40m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2524), null, 0m, "LAMINADO A FAZON - A.I. FINO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 901, true, 0m, null, null, "FAZ-AI-GRU", null, null, true, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2610), null, 0m, "LAMINADO A FAZON - A.I. GRUESO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 902, true, 0m, null, null, "FAZ-AI-FIN-COL", null, null, true, true, false, true, false, 0m, 0.90m, 0.40m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2529), null, 0m, "LAMINADO A FAZON - A.I. FINO COLOR", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 903, true, 0m, null, null, "FAZ-AI-GRU-COL", null, null, true, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2615), null, 0m, "LAMINADO A FAZON - A.I. GRUESO COLOR", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 904, true, 0m, null, null, "FAZ-AI-BIC", null, null, true, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2619), null, 0m, "LAMINADO A FAZON - A.I. BICAPA", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 905, true, 0m, null, null, "FAZ-AI-TRI", null, null, true, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2624), null, 0m, "LAMINADO A FAZON - A.I. TRICAPA", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 906, true, 0m, null, null, "FAZ-AI-TUT-FIN", null, null, true, true, false, true, false, 0m, 0.90m, 0.40m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2533), null, 0m, "LAMINADO A FAZON - A.I. TUTTI FINO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 907, true, 0m, null, null, "FAZ-AI-TUT-GRU", null, null, true, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2628), null, 0m, "LAMINADO A FAZON - A.I. TUTTI GRUESO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 908, true, 0m, null, null, "FAZ-ABS-GRU", null, null, true, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2632), null, 0m, "LAMINADO A FAZON - ABS GRUESO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 909, true, 0m, null, null, "FAZ-POLI-FIN", null, null, true, true, false, true, false, 0m, 0.90m, 0.40m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2537), null, 0m, "LAMINADO A FAZON - PEAD/PP/BIO FINO", 0.95m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 910, true, 0m, null, null, "FAZ-POLI-GRU", null, null, true, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2636), null, 0m, "LAMINADO A FAZON - PEAD/PP/BIO GRUESO", 0.95m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 911, true, 0m, null, null, "FAZ-PEAD-BIC", null, null, true, true, false, true, false, 0m, 0m, 0.90m, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2639), null, 0m, "LAMINADO A FAZON - PEAD BICAPA", 0.96m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 990, true, 0m, null, null, "MP-FAZ-AI", null, null, false, false, true, false, false, 0m, null, null, 10, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2263), null, 0m, "MP FAZÓN ALTO IMPACTO (BASE)", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 991, true, 0m, null, null, "MP-FAZ-ABS", null, null, false, false, true, false, false, 0m, null, null, 20, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2266), null, 0m, "MP FAZÓN ABS (BASE)", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 992, true, 0m, null, null, "MP-FAZ-PP", null, null, false, false, true, false, false, 0m, null, null, 30, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2269), null, 0m, "MP FAZÓN POLIPROPILENO (BASE)", 0.91m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 993, true, 0m, null, null, "MP-FAZ-PE", null, null, false, false, true, false, false, 0m, null, null, 40, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2273), null, 0m, "MP FAZÓN PEAD/PEBD (BASE)", 0.96m, 0m, null, "SERVICIO FAZON", 0m, 0m },
                    { 999, true, 0m, null, null, "MP-FAZON-GEN", null, null, false, false, true, false, false, 0m, null, null, null, new DateTime(2026, 1, 12, 13, 31, 15, 185, DateTimeKind.Local).AddTicks(2258), null, 0m, "MATERIAL DE CLIENTE (GENÉRICO)", 1.00m, 0m, null, "SERVICIO FAZON", 0m, 0m }
                });

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 50, 100m, 990, 900 },
                    { 51, 100m, 990, 901 },
                    { 52, 98m, 990, 902 },
                    { 53, 2m, 22, 902 },
                    { 54, 98m, 990, 903 },
                    { 55, 2m, 22, 903 },
                    { 56, 100m, 990, 904 },
                    { 57, 100m, 990, 905 },
                    { 58, 100m, 990, 906 },
                    { 59, 100m, 990, 907 },
                    { 60, 100m, 991, 908 },
                    { 61, 100m, 992, 909 },
                    { 62, 100m, 992, 910 },
                    { 63, 100m, 993, 911 },
                    { 70, 100m, 602, 106 },
                    { 71, 100m, 602, 107 }
                });

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
                name: "IX_Ordenes_ClienteId",
                table: "Ordenes",
                column: "ClienteId");

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
                name: "ClientesMaterialesFazon");

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
