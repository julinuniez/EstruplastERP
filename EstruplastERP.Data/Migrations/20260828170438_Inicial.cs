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
                    EsFazon = table.Column<bool>(type: "bit", nullable: false),
                    LimiteKilosPallet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HojasCarga",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoLote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDeclaracion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HojasCarga", x => x.Id);
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
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
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
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoSku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Rubro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsFazon = table.Column<bool>(type: "bit", nullable: false),
                    EsScrap = table.Column<bool>(type: "bit", nullable: false),
                    EspesorMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EspesorMaximo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PesoEspecifico = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 3, nullable: false),
                    PrecioCosto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_Productos_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
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
                name: "ConsumosHojasCarga",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HojaCargaId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    CantidadRealKg = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumosHojasCarga", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumosHojasCarga_HojasCarga_HojaCargaId",
                        column: x => x.HojaCargaId,
                        principalTable: "HojasCarga",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumosHojasCarga_Productos_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Formulas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoTerminadoId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ExtrusoraDestino = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formulas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formulas_Productos_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Formulas_Productos_ProductoTerminadoId",
                        column: x => x.ProductoTerminadoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ordenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroPedidoCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotaPedido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Largo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ancho = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Espesor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KilosEstimados = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Desperdicio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EsBobina = table.Column<bool>(type: "bit", nullable: false),
                    ConBrillo = table.Column<bool>(type: "bit", nullable: false),
                    LlevaFilm = table.Column<bool>(type: "bit", nullable: false),
                    EsGofrado = table.Column<bool>(type: "bit", nullable: false),
                    AditivoUV = table.Column<bool>(type: "bit", nullable: false),
                    TipoCorona = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsImpreso = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HojaCargaId = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Ordenes_HojasCarga_HojaCargaId",
                        column: x => x.HojaCargaId,
                        principalTable: "HojasCarga",
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
                    ExtrusoraDestino = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    Observacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    ProveedorId = table.Column<int>(type: "int", nullable: true),
                    NumeroRemito = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrdenProduccionId = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Movimientos_Ordenes_OrdenProduccionId",
                        column: x => x.OrdenProduccionId,
                        principalTable: "Ordenes",
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
                name: "PalletsProduccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenProduccionId = table.Column<int>(type: "int", nullable: false),
                    NumeroPallet = table.Column<int>(type: "int", nullable: false),
                    Kilos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PalletsProduccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PalletsProduccion_Ordenes_OrdenProduccionId",
                        column: x => x.OrdenProduccionId,
                        principalTable: "Ordenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "ClienteId", "CodigoSku", "EsFazon", "EsGenerico", "EsMateriaPrima", "EsProductoTerminado", "EsScrap", "EspesorMaximo", "EspesorMinimo", "FamiliaId", "FechaCreacion", "Nombre", "PesoEspecifico", "PrecioCosto", "ProveedorId", "Rubro", "StockActual", "StockMinimo", "TipoMaterial" },
                values: new object[,]
                {
                    { 22, true, null, "MP-MB-COL", false, false, true, false, false, null, null, 50, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(892), "Masterbatch Color (Varios)", 1.1m, 0m, null, "MATERIA PRIMA", 0m, 0m, null },
                    { 100, true, null, "AI-FINO", false, true, false, true, false, 0.90m, 0.40m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(900), "A.I. FINO (0.40 - 0.90 mm)", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 101, true, null, "AI-GRUESO", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(914), "A.I. GRUESO", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 102, true, null, "AI-FINO-COL", false, true, false, true, false, 0.90m, 0.40m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(906), "A.I. FINO COLOR", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 103, true, null, "AI-GRUESO-COL", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(918), "A.I. GRUESO COLOR", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 104, true, null, "AI-BICAPA", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(922), "A.I. BICAPA", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 105, true, null, "AI-TRICAPA", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(926), "A.I. TRICAPA", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 106, true, null, "AI-TUTTI-FINO", false, true, false, true, false, 0.90m, 0.40m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(910), "A.I. TUTTI FINO", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 107, true, null, "AI-TUTTI-GRUESO", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(944), "A.I. TUTTI GRUESO", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 108, true, null, "AI-FREON", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(969), "A.I. RESISTENTE AL FREON", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 109, true, null, "AI-FREON-COL", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(981), "A.I. RESISTENTE AL FREON COLOR", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 200, true, null, "ABS-BLA", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(985), "ABS BLANCO", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 201, true, null, "ABS-COL", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(990), "ABS COLOR", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 202, true, null, "ABS-GRUESO", false, true, false, true, false, 0m, 1.00m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1077), "ABS GRUESO (Min 1mm)", 1.05m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 300, true, null, "PP-STD", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1084), "PP (POLIPROPILENO)", 0.91m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 301, true, null, "PP-COL", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1092), "PP COLOR", 0.91m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 400, true, null, "PE-MIX", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1097), "PEAD / PEBD", 0.94m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 401, true, null, "PEBD-GOF", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1101), "PEBD GOFRADO", 0.92m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 402, true, null, "PEAD-BIC", false, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1105), "PEAD BICAPA", 0.96m, 0m, null, "PRODUCTO TERMINADO", 0m, 0m, null },
                    { 900, true, null, "FAZ-AI-FIN", true, true, false, true, false, 0.90m, 0.40m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1109), "FAZON - A.I. FINO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 901, true, null, "FAZ-AI-GRU", true, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1122), "FAZON - A.I. GRUESO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 902, true, null, "FAZ-AI-FIN-COL", true, true, false, true, false, 0.90m, 0.40m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1114), "FAZON - A.I. FINO COLOR", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 903, true, null, "FAZ-AI-GRU-COL", true, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1126), "FAZON - A.I. GRUESO COLOR", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 904, true, null, "FAZ-AI-BIC", true, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1130), "FAZON - A.I. BICAPA", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 905, true, null, "FAZ-AI-TRI", true, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1134), "FAZON - A.I. TRICAPA", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 906, true, null, "FAZ-AI-TUT-FIN", true, true, false, true, false, 0.90m, 0.40m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1118), "FAZON - A.I. TUTTI FINO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 907, true, null, "FAZ-AI-TUT-GRU", true, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1140), "FAZON - A.I. TUTTI GRUESO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 908, true, null, "FAZ-ABS-GRU", true, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1144), "FAZON - ABS GRUESO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 911, true, null, "FAZ-PEAD-BIC", true, true, false, true, false, 0m, 0.90m, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1148), "FAZON - PEAD BICAPA", 0.96m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 912, true, null, "FAZ-FREON-FIN", true, true, false, true, false, 0.90m, 0.40m, 60, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1152), "FAZON - RESISTENTE FREON FINO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 913, true, null, "FAZ-FREON-GRU", true, true, false, true, false, 0m, 0.90m, 60, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1158), "FAZON - RESISTENTE FREON GRUESO", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 914, true, null, "FAZ-FREON-COL", true, true, false, true, false, 0m, 0.90m, 60, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(1163), "FAZON - RESISTENTE FREON COLOR", 1.05m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 990, true, null, "MP-FAZ-AI", false, false, true, false, false, null, null, 10, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(873), "MP FAZÓN ALTO IMPACTO (BASE)", 1.1m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 991, true, null, "MP-FAZ-ABS", false, false, true, false, false, null, null, 20, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(877), "MP FAZÓN ABS (BASE)", 1.1m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 992, true, null, "MP-FAZ-PP", false, false, true, false, false, null, null, 30, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(881), "MP FAZÓN POLIPROPILENO (BASE)", 0.91m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 993, true, null, "MP-FAZ-PE", false, false, true, false, false, null, null, 40, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(885), "MP FAZÓN PEAD/PEBD (BASE)", 0.96m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 994, true, null, "MP-FAZ-FREON", false, false, true, false, false, null, null, 60, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(889), "MP FAZÓN FREON (BASE)", 1.1m, 0m, null, "SERVICIO FAZON", 0m, 0m, null },
                    { 999, true, null, "MP-FAZON-GEN", false, false, true, false, false, null, null, null, new DateTime(2026, 8, 28, 14, 4, 37, 671, DateTimeKind.Local).AddTicks(866), "MATERIAL DE CLIENTE (GENÉRICO)", 1.00m, 0m, null, "SERVICIO FAZON", 0m, 0m, null }
                });

            migrationBuilder.InsertData(
                table: "Formulas",
                columns: new[] { "Id", "Cantidad", "ExtrusoraDestino", "MateriaPrimaId", "ProductoTerminadoId" },
                values: new object[,]
                {
                    { 50, 100m, "UNICA", 990, 900 },
                    { 51, 100m, "UNICA", 990, 901 },
                    { 52, 98m, "UNICA", 990, 902 },
                    { 53, 2m, "UNICA", 22, 902 },
                    { 54, 98m, "UNICA", 990, 903 },
                    { 55, 2m, "UNICA", 22, 903 },
                    { 56, 100m, "UNICA", 990, 904 },
                    { 57, 100m, "UNICA", 990, 905 },
                    { 58, 100m, "UNICA", 990, 906 },
                    { 59, 100m, "UNICA", 990, 907 },
                    { 60, 100m, "UNICA", 991, 908 },
                    { 63, 100m, "UNICA", 993, 911 },
                    { 64, 100m, "UNICA", 994, 912 },
                    { 65, 100m, "UNICA", 994, 913 },
                    { 66, 100m, "UNICA", 994, 914 }
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
                name: "IX_ConsumosHojasCarga_HojaCargaId",
                table: "ConsumosHojasCarga",
                column: "HojaCargaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosHojasCarga_MateriaPrimaId",
                table: "ConsumosHojasCarga",
                column: "MateriaPrimaId");

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
                name: "IX_Movimientos_OrdenProduccionId",
                table: "Movimientos",
                column: "OrdenProduccionId");

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
                name: "IX_Ordenes_HojaCargaId",
                table: "Ordenes",
                column: "HojaCargaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_ProductoId",
                table: "Ordenes",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PalletsProduccion_OrdenProduccionId",
                table: "PalletsProduccion",
                column: "OrdenProduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_ClienteId",
                table: "Producciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_ProductoTerminadoId",
                table: "Producciones",
                column: "ProductoTerminadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ClienteId",
                table: "Productos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientesMaterialesFazon");

            migrationBuilder.DropTable(
                name: "ConsumosHojasCarga");

            migrationBuilder.DropTable(
                name: "ConsumosOrdenes");

            migrationBuilder.DropTable(
                name: "Formulas");

            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "PalletsProduccion");

            migrationBuilder.DropTable(
                name: "Producciones");

            migrationBuilder.DropTable(
                name: "RemitoDetalles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Ordenes");

            migrationBuilder.DropTable(
                name: "Remitos");

            migrationBuilder.DropTable(
                name: "HojasCarga");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Proveedores");
        }
    }
}
