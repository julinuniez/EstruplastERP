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

        // Aquí registramos las tablas
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

            // Configuraciones de decimales para evitar warnings de SQL
            modelBuilder.Entity<Producto>().Property(p => p.PesoEspecifico).HasPrecision(18, 4);
            modelBuilder.Entity<Producto>().Property(p => p.PrecioCosto).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.StockActual).HasPrecision(18, 3);
            modelBuilder.Entity<Producto>().Property(p => p.StockMinimo).HasPrecision(18, 3);

            modelBuilder.Entity<Formula>()
                .HasOne(f => f.ProductoTerminado)
                .WithMany()
                .HasForeignKey(f => f.ProductoTerminadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Formula>()
                .HasOne(f => f.MateriaPrima)
                .WithMany()
                .HasForeignKey(f => f.MateriaPrimaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>().HasData(
                // 1. POLIESTIRENO (Alto Impacto / PAI)
                new Producto
                {
                    Id = 1,
                    Nombre = "Poliestireno Alto Impacto (AI/PAI)",
                    CodigoSku = "MP-PAI",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 1.1m,
                    StockActual = 1000, // Le puse stock para que puedas probar
                    StockMinimo = 1000,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // 2. ABS
                new Producto
                {
                    Id = 2,
                    Nombre = "ABS (Acrilonitrilo Butadieno Estireno)",
                    CodigoSku = "MP-ABS",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 1.1M,
                    StockActual = 1000,
                    StockMinimo = 500,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // 3. POLIPROPILENO
                new Producto
                {
                    Id = 3,
                    Nombre = "Polipropileno (PP)",
                    CodigoSku = "MP-PP",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 0.91m,
                    StockActual = 1000,
                    StockMinimo = 1000,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // 4. POLIETILENO ALTA DENSIDAD
                new Producto
                {
                    Id = 4,
                    Nombre = "Polietileno Alta Densidad (PEAD)",
                    CodigoSku = "MP-PEAD",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 0.95m,
                    StockActual = 1000,
                    StockMinimo = 1000,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // 5. POLIETILENO BAJA DENSIDAD
                new Producto
                {
                    Id = 5,
                    Nombre = "Polietileno Baja Densidad (PEBD)",
                    CodigoSku = "MP-PEBD",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 0.92m,
                    StockActual = 1000,
                    StockMinimo = 1000,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // 7. BIOPLÁSTICO (BIOLAM)
                new Producto
                {
                    Id = 7,
                    Nombre = "Bioplástico Compostable (Biolam)",
                    CodigoSku = "MP-BIO",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 1.25m,
                    StockActual = 0,
                    StockMinimo = 200,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // 8. MASTERBATCH BLANCO
                new Producto
                {
                    Id = 8,
                    Nombre = "Masterbatch Blanco (Titanio)",
                    CodigoSku = "MP-MST-BLA",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 1.80m,
                    StockActual = 500,
                    StockMinimo = 100,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // 9. MASTERBATCH NEGRO
                new Producto
                {
                    Id = 9,
                    Nombre = "Masterbatch Negro",
                    CodigoSku = "MP-MST-NEG",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 1.20m,
                    StockActual = 500,
                    StockMinimo = 100,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // 10. ADITIVOS (Genérico que tenías)
                new Producto
                {
                    Id = 10,
                    Nombre = "Aditivo Genérico",
                    CodigoSku = "MP-ADITIVO-GEN",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    PesoEspecifico = 0.95m,
                    StockActual = 0,
                    StockMinimo = 50,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // =================================================================
                //  NUEVOS ADITIVOS PARA LA LÓGICA DE PRODUCCIÓN (IDs 30-34)
                // =================================================================
                new Producto
                {
                    Id = 30,
                    Nombre = "Aditivo Brillo",
                    CodigoSku = "MP-BRILLO",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    StockActual = 500,
                    PesoEspecifico = 0.92m,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 31,
                    Nombre = "Estearato de Zinc",
                    CodigoSku = "MP-ESTEARATO",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    StockActual = 200, // Stock para cubrir las latas
                    PesoEspecifico = 0.35m,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 32,
                    Nombre = "Carga Mineral (Carbonato)",
                    CodigoSku = "MP-CARGA",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    StockActual = 2000,
                    PesoEspecifico = 1.80m,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 33,
                    Nombre = "Aditivo UV",
                    CodigoSku = "MP-UV",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    StockActual = 200,
                    PesoEspecifico = 0.95m,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 34,
                    Nombre = "Aditivo Caucho",
                    CodigoSku = "MP-CAUCHO",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    StockActual = 200,
                    PesoEspecifico = 0.94m,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                },

                // --- COLORES ESPECÍFICOS (Que estaban sueltos al final) ---
                new Producto { Id = 11, Nombre = "Masterbatch Rojo", CodigoSku = "MP-MST-ROJ", EsMateriaPrima = true, EsProductoTerminado = false, PesoEspecifico = 1.20m, StockMinimo = 50, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 12, Nombre = "Masterbatch Azul", CodigoSku = "MP-MST-AZU", EsMateriaPrima = true, EsProductoTerminado = false, PesoEspecifico = 1.20m, StockMinimo = 50, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 13, Nombre = "Masterbatch Verde", CodigoSku = "MP-MST-VER", EsMateriaPrima = true, EsProductoTerminado = false, PesoEspecifico = 1.20m, StockMinimo = 50, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 14, Nombre = "Masterbatch Amarillo", CodigoSku = "MP-MST-AMA", EsMateriaPrima = true, EsProductoTerminado = false, PesoEspecifico = 1.20m, StockMinimo = 50, Activo = true, FechaCreacion = DateTime.Now },

                // --- PRODUCTOS TERMINADOS ---
                // --- LÍNEA ALTO IMPACTO (PAI) ---
                new Producto { Id = 50, Nombre = "Material PAI Blanco", CodigoSku = "MAT-PAI-B", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 51, Nombre = "Material PAI Negro", CodigoSku = "MAT-PAI-N", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 52, Nombre = "Material PAI Color", CodigoSku = "MAT-PAI-C", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 53, Nombre = "Material PAI Bicapa", CodigoSku = "MAT-PAI-BIC", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },

                // --- LÍNEA ABS ---
                new Producto { Id = 60, Nombre = "Material ABS Blanco", CodigoSku = "MAT-ABS-B", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 300, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 61, Nombre = "Material ABS Negro/Color", CodigoSku = "MAT-ABS-C", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 300, Activo = true, FechaCreacion = DateTime.Now },

                // --- LÍNEA POLIPROPILENO (PP) ---
                new Producto { Id = 70, Nombre = "Material PP (Polipropileno)", CodigoSku = "MAT-PP", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },

                // --- LÍNEA POLIETILENO ---
                new Producto { Id = 80, Nombre = "Material PEAD (Alta Densidad)", CodigoSku = "MAT-PEAD", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 81, Nombre = "Material PEBD (Baja Densidad)", CodigoSku = "MAT-PEBD", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },

                // --- LÍNEA BIOPLÁSTICO ---
                new Producto { Id = 90, Nombre = "Material Biolam (Compostable)", CodigoSku = "MAT-BIO", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 0, StockMinimo = 200, Activo = true, FechaCreacion = DateTime.Now },

                // --- REVENTA (PET) ---
                new Producto { Id = 99, Nombre = "Lámina PET (Reventa)", CodigoSku = "REV-PET", EsProductoTerminado = true, EsMateriaPrima = false, PesoEspecifico = 1.38m, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now }
            );

            // =================================================================================
            // RECETAS TÍPICAS (Porcentajes)
            // =================================================================================
            modelBuilder.Entity<Formula>().HasData(
                // PAI BLANCO: 96% PAI (Id 1) + 4% MB Blanco (Id 8)
                new Formula { Id = 200, ProductoTerminadoId = 50, MateriaPrimaId = 1, Cantidad = 96 },
                new Formula { Id = 201, ProductoTerminadoId = 50, MateriaPrimaId = 8, Cantidad = 4 },

                // PAI NEGRO: 98% PAI (Id 1) + 2% MB Negro (Id 9)
                new Formula { Id = 202, ProductoTerminadoId = 51, MateriaPrimaId = 1, Cantidad = 98 },
                new Formula { Id = 203, ProductoTerminadoId = 51, MateriaPrimaId = 9, Cantidad = 2 },

                // PP NATURAL: 100% PP (Id 3)
                new Formula { Id = 204, ProductoTerminadoId = 70, MateriaPrimaId = 3, Cantidad = 100 },

                //PAI COLOR
                new Formula { Id = 215, ProductoTerminadoId = 52, MateriaPrimaId = 1, Cantidad = 100 }
            );
        }
    }
}