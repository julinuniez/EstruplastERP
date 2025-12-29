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
        public DbSet<OrdenProduccion> Ordenes { get; set; }
        public DbSet<ConsumoOrden> ConsumosOrdenes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =============================================================================
            // CONFIGURACIONES DE PRECISIÓN
            // =============================================================================
            modelBuilder.Entity<Producto>().Property(p => p.PesoEspecifico).HasPrecision(18, 4);
            modelBuilder.Entity<Producto>().Property(p => p.PrecioCosto).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.StockActual).HasPrecision(18, 3);
            modelBuilder.Entity<Producto>().Property(p => p.StockMinimo).HasPrecision(18, 3);
            modelBuilder.Entity<Producto>().Property(p => p.Largo).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.Ancho).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.Espesor).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.EspesorMinimo).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.EspesorMaximo).HasPrecision(18, 2);

            modelBuilder.Entity<Formula>().Property(f => f.Cantidad).HasPrecision(18, 4);
            modelBuilder.Entity<Movimiento>().Property(m => m.Cantidad).HasPrecision(18, 3);
            modelBuilder.Entity<Produccion>().Property(p => p.Cantidad).HasPrecision(18, 2);
            modelBuilder.Entity<Produccion>().Property(p => p.Kilos).HasPrecision(18, 3);
            modelBuilder.Entity<RemitoDetalle>().Property(r => r.Cantidad).HasPrecision(18, 3);
            modelBuilder.Entity<RemitoDetalle>().Property(r => r.PrecioUnitarioSnapshot).HasPrecision(18, 2);
            modelBuilder.Entity<Movimiento>().Property(m => m.PrecioUnitario).HasPrecision(18, 2);
            modelBuilder.Entity<Movimiento>().Property(m => m.PrecioTotal).HasPrecision(18, 2);

            // =============================================================================
            // RELACIONES
            // =============================================================================
            modelBuilder.Entity<Formula>()
                .HasOne(f => f.ProductoTerminado).WithMany(p => p.Formulas).HasForeignKey(f => f.ProductoTerminadoId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Formula>()
                .HasOne(f => f.MateriaPrima).WithMany().HasForeignKey(f => f.MateriaPrimaId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.ProductoPadre).WithMany().HasForeignKey(p => p.ProductoPadreId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Cliente).WithMany().HasForeignKey(p => p.ClienteId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsumoOrden>(entity =>
            {
                entity.HasOne(c => c.MateriaPrima).WithMany().HasForeignKey(c => c.MateriaPrimaId).OnDelete(DeleteBehavior.NoAction);
            });

            // =============================================================================
            // SEEDING (DATOS INICIALES)
            // =============================================================================
            modelBuilder.Entity<Producto>().HasData(

                // 1. MATERIAS PRIMAS ESENCIALES (NO BORRAR)
                // Necesarias para que funcionen las fórmulas de Fazón
                new Producto { Id = 999, Nombre = "MATERIAL DE CLIENTE (FAZÓN GENÉRICO)", CodigoSku = "MP-FAZON-GEN", EsMateriaPrima = true, PesoEspecifico = 1.00m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 22, Nombre = "Masterbatch Color (Varios)", CodigoSku = "MP-MB-COL", EsMateriaPrima = true, PesoEspecifico = 1.20m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },

                // 5. MATERIAS PRIMAS RECUPERADAS (SCRAP Y TUTTI) - NUEVO
                // =============================================================================
                // --- ALTO IMPACTO (PAI) ---
                new Producto { Id = 600, Nombre = "SCRAP A.I. BLANCO", CodigoSku = "REC-AI-BLA", EsMateriaPrima = true, PesoEspecifico = 1.05m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 601, Nombre = "SCRAP A.I. NEGRO", CodigoSku = "REC-AI-NEG", EsMateriaPrima = true, PesoEspecifico = 1.05m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 602, Nombre = "A.I. TUTTI (MEZCLA)", CodigoSku = "REC-AI-TUT", EsMateriaPrima = true, PesoEspecifico = 1.05m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },

                // --- POLIPROPILENO (PP) ---
                new Producto { Id = 603, Nombre = "SCRAP PP", CodigoSku = "REC-PP", EsMateriaPrima = true, PesoEspecifico = 0.91m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },

                // --- POLIETILENO ALTA DENSIDAD (PEAD) ---
                new Producto { Id = 604, Nombre = "SCRAP PEAD", CodigoSku = "REC-PEAD", EsMateriaPrima = true, PesoEspecifico = 0.96m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },

                // --- ABS ---
                new Producto { Id = 605, Nombre = "SCRAP ABS", CodigoSku = "REC-ABS", EsMateriaPrima = true, PesoEspecifico = 1.05m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },


                // 2. PRODUCTOS TERMINADOS (PROPIOS)
                // A.I. FINO (0.40 - 0.90)
                new Producto { Id = 100, Nombre = "A.I. FINO (0.40 - 0.90 mm)", CodigoSku = "AI-FINO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 102, Nombre = "A.I. FINO COLOR", CodigoSku = "AI-FINO-COL", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Color = "A Elección", Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 106, Nombre = "A.I. TUTTI FINO", CodigoSku = "AI-TUTTI-FINO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },

                // A.I. GRUESO (0.90 - Sin Limite)
                new Producto { Id = 101, Nombre = "A.I. GRUESO", CodigoSku = "AI-GRUESO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 103, Nombre = "A.I. GRUESO COLOR", CodigoSku = "AI-GRUESO-COL", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Color = "A Elección", Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 104, Nombre = "A.I. BICAPA", CodigoSku = "AI-BICAPA", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 105, Nombre = "A.I. TRICAPA", CodigoSku = "AI-TRICAPA", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 107, Nombre = "A.I. TUTTI GRUESO", CodigoSku = "AI-TUTTI-GRUESO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 108, Nombre = "A.I. RESISTENTE AL FREON", CodigoSku = "AI-FREON", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 109, Nombre = "A.I. RESISTENTE AL FREON COLOR", CodigoSku = "AI-FREON-COL", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Color = "A Elección", Activo = true, FechaCreacion = DateTime.Now },

                // ABS (Asumimos 0.90+ excepto el Grueso explicito de 1.00)
                new Producto { Id = 200, Nombre = "ABS BLANCO", CodigoSku = "ABS-BLA", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Color = "Blanco", Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 201, Nombre = "ABS COLOR", CodigoSku = "ABS-COL", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Color = "A Elección", Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 202, Nombre = "ABS GRUESO (Min 1mm)", CodigoSku = "ABS-GRUESO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 1.00m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },

                // PP (0.90+)
                new Producto { Id = 300, Nombre = "PP (POLIPROPILENO)", CodigoSku = "PP-STD", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.91m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 301, Nombre = "PP COLOR", CodigoSku = "PP-COL", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.91m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Color = "A Elección", Activo = true, FechaCreacion = DateTime.Now },

                // PEAD / PEBD / BIO (0.90+)
                new Producto { Id = 400, Nombre = "PEAD / PEBD", CodigoSku = "PE-MIX", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.94m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 401, Nombre = "PEBD GOFRADO", CodigoSku = "PEBD-GOF", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.92m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 402, Nombre = "PEAD BICAPA", CodigoSku = "PEAD-BIC", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.96m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 500, Nombre = "BIOPLASTICO", CodigoSku = "BIO-LAM", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.25m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },

                // 3. FAZÓN (SERVICIO)
                new Producto { Id = 900, Nombre = "LAMINADO A FAZON - A.I. FINO", CodigoSku = "FAZ-AI-FIN", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 902, Nombre = "LAMINADO A FAZON - A.I. FINO COLOR", CodigoSku = "FAZ-AI-FIN-COL", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 906, Nombre = "LAMINADO A FAZON - A.I. TUTTI FINO", CodigoSku = "FAZ-AI-TUT-FIN", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 909, Nombre = "LAMINADO A FAZON - PEAD/PP/BIO FINO", CodigoSku = "FAZ-POLI-FIN", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.95m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 901, Nombre = "LAMINADO A FAZON - A.I. GRUESO", CodigoSku = "FAZ-AI-GRU", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 903, Nombre = "LAMINADO A FAZON - A.I. GRUESO COLOR", CodigoSku = "FAZ-AI-GRU-COL", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 904, Nombre = "LAMINADO A FAZON - A.I. BICAPA", CodigoSku = "FAZ-AI-BIC", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 905, Nombre = "LAMINADO A FAZON - A.I. TRICAPA", CodigoSku = "FAZ-AI-TRI", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 907, Nombre = "LAMINADO A FAZON - A.I. TUTTI GRUESO", CodigoSku = "FAZ-AI-TUT-GRU", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 908, Nombre = "LAMINADO A FAZON - ABS GRUESO", CodigoSku = "FAZ-ABS-GRU", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 910, Nombre = "LAMINADO A FAZON - PEAD/PP/BIO GRUESO", CodigoSku = "FAZ-POLI-GRU", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.95m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 911, Nombre = "LAMINADO A FAZON - PEAD BICAPA", CodigoSku = "FAZ-PEAD-BIC", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.96m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now }
            );

            // =============================================================================
            // 4. RESTAURACIÓN DE RECETAS (FÓRMULAS)
            // =============================================================================
            modelBuilder.Entity<Formula>().HasData(

                // --- RECETAS FAZÓN (Usan ID 999) ---
                new Formula { Id = 50, ProductoTerminadoId = 900, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 51, ProductoTerminadoId = 901, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 52, ProductoTerminadoId = 902, MateriaPrimaId = 999, Cantidad = 98 },
                new Formula { Id = 53, ProductoTerminadoId = 902, MateriaPrimaId = 22, Cantidad = 2 },  // Color
                new Formula { Id = 54, ProductoTerminadoId = 903, MateriaPrimaId = 999, Cantidad = 98 },
                new Formula { Id = 55, ProductoTerminadoId = 903, MateriaPrimaId = 22, Cantidad = 2 },  // Color
                new Formula { Id = 56, ProductoTerminadoId = 904, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 57, ProductoTerminadoId = 905, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 58, ProductoTerminadoId = 906, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 59, ProductoTerminadoId = 907, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 60, ProductoTerminadoId = 908, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 61, ProductoTerminadoId = 909, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 62, ProductoTerminadoId = 910, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 63, ProductoTerminadoId = 911, MateriaPrimaId = 999, Cantidad = 100 },

                // --- RECETAS PROPIAS "TUTTI" (Usan ID 602 - A.I. TUTTI MEZCLA) ---
                // Para A.I. TUTTI FINO (106) y GRUESO (107)
                new Formula { Id = 70, ProductoTerminadoId = 106, MateriaPrimaId = 602, Cantidad = 100 },
                new Formula { Id = 71, ProductoTerminadoId = 107, MateriaPrimaId = 602, Cantidad = 100 }
            );

        }
    }
}