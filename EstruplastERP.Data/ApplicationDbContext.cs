using Microsoft.EntityFrameworkCore;
using EstruplastERP.Core;

namespace EstruplastERP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Formula> Formulas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produccion> Producciones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Remito> Remitos { get; set; }
        public DbSet<RemitoDetalle> RemitoDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones de precisión
            modelBuilder.Entity<Producto>().Property(p => p.PesoEspecifico).HasPrecision(18, 4);
            modelBuilder.Entity<Producto>().Property(p => p.PrecioCosto).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.StockActual).HasPrecision(18, 3);
            modelBuilder.Entity<Producto>().Property(p => p.StockMinimo).HasPrecision(18, 3);

            modelBuilder.Entity<Formula>()
                .HasOne(f => f.ProductoTerminado).WithMany().HasForeignKey(f => f.ProductoTerminadoId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Formula>()
                .HasOne(f => f.MateriaPrima).WithMany().HasForeignKey(f => f.MateriaPrimaId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>().HasData(
                // =================================================================================
                // 1. MATERIAS PRIMAS (EsGenerico = false, nadie edita sus medidas)
                // =================================================================================
                new Producto
                {
                    Id = 1,
                    Nombre = "Poliestireno Alto Impacto (AI/PAI)",
                    CodigoSku = "MP-PAI",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 1.1m,
                    StockActual = 1000,
                    StockMinimo = 1000,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 2,
                    Nombre = "ABS (Acrilonitrilo Butadieno Estireno)",
                    CodigoSku = "MP-ABS",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 1.1M,
                    StockActual = 1000,
                    StockMinimo = 500,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 3,
                    Nombre = "Polipropileno (PP)",
                    CodigoSku = "MP-PP",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 0.91m,
                    StockActual = 1000,
                    StockMinimo = 1000,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 4,
                    Nombre = "Polietileno Alta Densidad (PEAD)",
                    CodigoSku = "MP-PEAD",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 0.95m,
                    StockActual = 1000,
                    StockMinimo = 1000,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 5,
                    Nombre = "Polietileno Baja Densidad (PEBD)",
                    CodigoSku = "MP-PEBD",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 0.92m,
                    StockActual = 1000,
                    StockMinimo = 1000,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 7,
                    Nombre = "Bioplástico Compostable (Biolam)",
                    CodigoSku = "MP-BIO",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 1.25m,
                    StockActual = 0,
                    StockMinimo = 200,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 8,
                    Nombre = "Masterbatch Blanco (Titanio)",
                    CodigoSku = "MP-MST-BLA",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 1.80m,
                    StockActual = 500,
                    StockMinimo = 100,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 9,
                    Nombre = "Masterbatch Negro",
                    CodigoSku = "MP-MST-NEG",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 1.20m,
                    StockActual = 500,
                    StockMinimo = 100,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 10,
                    Nombre = "Aditivo Genérico",
                    CodigoSku = "MP-ADITIVO-GEN",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = false, // 🔒
                    PesoEspecifico = 0.95m,
                    StockActual = 0,
                    StockMinimo = 50,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // --- COLORES ---
                new Producto { Id = 11, Nombre = "Masterbatch Rojo", CodigoSku = "MP-MST-ROJ", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 12, Nombre = "Masterbatch Azul", CodigoSku = "MP-MST-AZU", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 13, Nombre = "Masterbatch Verde", CodigoSku = "MP-MST-VER", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 14, Nombre = "Masterbatch Amarillo", CodigoSku = "MP-MST-AMA", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },

                // --- ADITIVOS DE PRODUCCIÓN ---
                new Producto { Id = 30, Nombre = "Aditivo Brillo", CodigoSku = "MP-BRILLO", EsMateriaPrima = true, EsGenerico = false, StockActual = 500, PesoEspecifico = 0.92m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 31, Nombre = "Estearato de Zinc", CodigoSku = "MP-ESTEARATO", EsMateriaPrima = true, EsGenerico = false, StockActual = 200, PesoEspecifico = 0.35m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 32, Nombre = "Carga Mineral (Carbonato)", CodigoSku = "MP-CARGA", EsMateriaPrima = true, EsGenerico = false, StockActual = 2000, PesoEspecifico = 1.80m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 33, Nombre = "Aditivo UV", CodigoSku = "MP-UV", EsMateriaPrima = true, EsGenerico = false, StockActual = 200, PesoEspecifico = 0.95m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 34, Nombre = "Aditivo Caucho", CodigoSku = "MP-CAUCHO", EsMateriaPrima = true, EsGenerico = false, StockActual = 200, PesoEspecifico = 0.94m, Activo = true, FechaCreacion = DateTime.Now },

                // =================================================================================
                // 2. PRODUCTOS GENÉRICOS (A Medida) -> EsGenerico = true
                // =================================================================================
                // Estos son los que el operario elige para fabricar "a pedido". Se habilita el input de medidas.

                // LÍNEA PAI
                new Producto
                {
                    Id = 50,
                    Nombre = "Material PAI Blanco (A Medida)",
                    CodigoSku = "MAT-PAI-B",
                    EsProductoTerminado = true,
                    EsMateriaPrima = false,
                    EsGenerico = true, // ✅ ABIERTO A EDICIÓN
                    PesoEspecifico = 0,
                    StockMinimo = 500,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 51,
                    Nombre = "Material PAI Negro (A Medida)",
                    CodigoSku = "MAT-PAI-N",
                    EsProductoTerminado = true,
                    EsMateriaPrima = false,
                    EsGenerico = true, // ✅ ABIERTO A EDICIÓN
                    PesoEspecifico = 0,
                    StockMinimo = 500,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 52,
                    Nombre = "Material PAI Color (A Medida)",
                    CodigoSku = "MAT-PAI-C",
                    EsProductoTerminado = true,
                    EsMateriaPrima = false,
                    EsGenerico = true, // ✅ ABIERTO A EDICIÓN
                    PesoEspecifico = 0,
                    StockMinimo = 200,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 53,
                    Nombre = "Material PAI Bicapa (A Medida)",
                    CodigoSku = "MAT-PAI-BIC",
                    EsProductoTerminado = true,
                    EsMateriaPrima = false,
                    EsGenerico = true, // ✅ ABIERTO A EDICIÓN
                    PesoEspecifico = 0,
                    StockMinimo = 500,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // LÍNEA ABS
                new Producto { Id = 60, Nombre = "Material ABS Blanco (A Medida)", CodigoSku = "MAT-ABS-B", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 300, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 61, Nombre = "Material ABS Negro/Color (A Medida)", CodigoSku = "MAT-ABS-C", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 300, Activo = true, FechaCreacion = DateTime.Now },

                // LÍNEA PP
                new Producto { Id = 70, Nombre = "Material PP (A Medida)", CodigoSku = "MAT-PP", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },

                // LÍNEA PEAD/PEBD
                new Producto { Id = 80, Nombre = "Material PEAD (A Medida)", CodigoSku = "MAT-PEAD", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 81, Nombre = "Material PEBD (A Medida)", CodigoSku = "MAT-PEBD", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },

                // BIOLAM & REVENTA
                new Producto { Id = 90, Nombre = "Material Biolam (A Medida)", CodigoSku = "MAT-BIO", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 99, Nombre = "Lámina PET (Reventa)", CodigoSku = "REV-PET", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },


                // =================================================================================
                // 3. PRODUCTOS ESTÁNDAR (De Stock) -> EsGenerico = false
                // =================================================================================
                // Estos son ejemplos de lo que sí stockeas fijo. El input de medidas se bloqueará.

                new Producto
                {
                    Id = 100,
                    Nombre = "Lámina PAI Blanco 1000x1000 (Estándar)",
                    CodigoSku = "STD-PAI-1000",
                    EsProductoTerminado = true,
                    EsMateriaPrima = false,
                    EsGenerico = false, // 🔒 FIJO
                    PesoEspecifico = 0,
                    StockActual = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 101,
                    Nombre = "Lámina PAI Negro 1220x2440 (Estándar)",
                    CodigoSku = "STD-PAI-1220",
                    EsProductoTerminado = true,
                    EsMateriaPrima = false,
                    EsGenerico = false, // 🔒 FIJO
                    PesoEspecifico = 0,
                    StockActual = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                }
            );

            // =================================================================================
            // RECETAS TÍPICAS (Esto no cambia)
            // =================================================================================
            modelBuilder.Entity<Formula>().HasData(
                // Las recetas se asocian a los ID Base (50, 51, etc.)
                new Formula { Id = 200, ProductoTerminadoId = 50, MateriaPrimaId = 1, Cantidad = 96 }, // PAI Blanco
                new Formula { Id = 201, ProductoTerminadoId = 50, MateriaPrimaId = 8, Cantidad = 4 },
                new Formula { Id = 202, ProductoTerminadoId = 51, MateriaPrimaId = 1, Cantidad = 98 }, // PAI Negro
                new Formula { Id = 203, ProductoTerminadoId = 51, MateriaPrimaId = 9, Cantidad = 2 },
                new Formula { Id = 204, ProductoTerminadoId = 70, MateriaPrimaId = 3, Cantidad = 100 }, // PP Natural
                new Formula { Id = 215, ProductoTerminadoId = 52, MateriaPrimaId = 1, Cantidad = 100 }, // PAI Color (100% mat + color en producción)

                // Si quisieras que el Estándar (100) tenga receta automática:
                // Como es PAI Blanco, usa la misma lógica que el 50
                new Formula { Id = 300, ProductoTerminadoId = 100, MateriaPrimaId = 1, Cantidad = 96 },
                new Formula { Id = 301, ProductoTerminadoId = 100, MateriaPrimaId = 8, Cantidad = 4 }
            );
        }
    }
}