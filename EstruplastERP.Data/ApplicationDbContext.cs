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

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Poliestireno Alto Impacto (AI/PAI)", CodigoSku = "MP-PAI", EsMateriaPrima = true, PesoEspecifico = 1.05m, StockActual = 1000, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 2, Nombre = "ABS", CodigoSku = "MP-ABS", EsMateriaPrima = true, PesoEspecifico = 1.05M, StockActual = 1000, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 3, Nombre = "Polipropileno (PP)", CodigoSku = "MP-PP", EsMateriaPrima = true, PesoEspecifico = 0.91m, StockActual = 1000, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 4, Nombre = "Polietileno Alta Densidad (PEAD)", CodigoSku = "MP-PEAD", EsMateriaPrima = true, PesoEspecifico = 0.96m, StockActual = 1000, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 5, Nombre = "Polietileno Baja Densidad (PEBD)", CodigoSku = "MP-PEBD", EsMateriaPrima = true, PesoEspecifico = 0.92m, StockActual = 1000, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 6, Nombre = "Bioplástico", CodigoSku = "MP-BIO", EsMateriaPrima = true, PesoEspecifico = 1.25m, StockActual = 0, StockMinimo = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 7, Nombre = "Tuti Fino", CodigoSku = "MP-TUTI-FINO", EsMateriaPrima = true, PesoEspecifico = 1.05m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 8, Nombre = "Tuti Grueso", CodigoSku = "MP-TUTI-GRUESO", EsMateriaPrima = true, PesoEspecifico = 1.05m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 20, Nombre = "Masterbatch Blanco", CodigoSku = "MP-MB-BLA", EsMateriaPrima = true, PesoEspecifico = 1.80m, StockActual = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 21, Nombre = "Masterbatch Negro", CodigoSku = "MP-MB-NEG", EsMateriaPrima = true, PesoEspecifico = 1.20m, StockActual = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 22, Nombre = "Masterbatch Color (Varios)", CodigoSku = "MP-MB-COL", EsMateriaPrima = true, PesoEspecifico = 1.20m, StockActual = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 23, Nombre = "Carga Mineral", CodigoSku = "MP-CARGA", EsMateriaPrima = true, PesoEspecifico = 1.80m, StockActual = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 24, Nombre = "Aditivo UV", CodigoSku = "MP-UV", EsMateriaPrima = true, PesoEspecifico = 0.95m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 25, Nombre = "Aditivo Caucho", CodigoSku = "MP-CAUCHO", EsMateriaPrima = true, PesoEspecifico = 0.94m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 26, Nombre = "Estearato", CodigoSku = "MP-ESTEARATO", EsMateriaPrima = true, PesoEspecifico = 0.35m, StockActual = 50, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 27, Nombre = "Brillo", CodigoSku = "MP-BRILLO", EsMateriaPrima = true, PesoEspecifico = 1.00m, StockActual = 50, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 999, Nombre = "MATERIAL DE CLIENTE (FAZÓN)", CodigoSku = "MP-FAZON-GEN", EsMateriaPrima = true, PesoEspecifico = 1.00m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now }
            );

            // -----------------------------------------------------------------------------
            // 2. PRODUCTOS TERMINADOS (PROPIOS) - CON LÍMITES
            // -----------------------------------------------------------------------------
            modelBuilder.Entity<Producto>().HasData(
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
                new Producto { Id = 500, Nombre = "BIOPLASTICO", CodigoSku = "BIO-LAM", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.25m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now }
            );

            // -----------------------------------------------------------------------------
            // 3. SECCIÓN: FAZÓN (SERVICIO) - CON LÍMITES
            // -----------------------------------------------------------------------------
            modelBuilder.Entity<Producto>().HasData(
                // FINOS (0.40 - 0.90)
                new Producto { Id = 900, Nombre = "LAMINADO A FAZON - A.I. FINO", CodigoSku = "FAZ-AI-FIN", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 902, Nombre = "LAMINADO A FAZON - A.I. FINO COLOR", CodigoSku = "FAZ-AI-FIN-COL", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 906, Nombre = "LAMINADO A FAZON - A.I. TUTTI FINO", CodigoSku = "FAZ-AI-TUT-FIN", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 909, Nombre = "LAMINADO A FAZON - PEAD/PP/BIO FINO", CodigoSku = "FAZ-POLI-FIN", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.95m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },

                // GRUESOS (0.90 - Sin Limite)
                new Producto { Id = 901, Nombre = "LAMINADO A FAZON - A.I. GRUESO", CodigoSku = "FAZ-AI-GRU", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 903, Nombre = "LAMINADO A FAZON - A.I. GRUESO COLOR", CodigoSku = "FAZ-AI-GRU-COL", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 904, Nombre = "LAMINADO A FAZON - A.I. BICAPA", CodigoSku = "FAZ-AI-BIC", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 905, Nombre = "LAMINADO A FAZON - A.I. TRICAPA", CodigoSku = "FAZ-AI-TRI", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 907, Nombre = "LAMINADO A FAZON - A.I. TUTTI GRUESO", CodigoSku = "FAZ-AI-TUT-GRU", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 908, Nombre = "LAMINADO A FAZON - ABS GRUESO", CodigoSku = "FAZ-ABS-GRU", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 910, Nombre = "LAMINADO A FAZON - PEAD/PP/BIO GRUESO", CodigoSku = "FAZ-POLI-GRU", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.95m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 911, Nombre = "LAMINADO A FAZON - PEAD BICAPA", CodigoSku = "FAZ-PEAD-BIC", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.96m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now }
            );

            // ... (Se mantienen las formulas tal cual las tenías) ...
            modelBuilder.Entity<Formula>().HasData(
                // ... (Copiar tus formulas anteriores aquí) ...
                new Formula { Id = 1, ProductoTerminadoId = 100, MateriaPrimaId = 1, Cantidad = 96 },
                new Formula { Id = 2, ProductoTerminadoId = 100, MateriaPrimaId = 20, Cantidad = 4 },
                new Formula { Id = 3, ProductoTerminadoId = 102, MateriaPrimaId = 1, Cantidad = 98 },
                new Formula { Id = 4, ProductoTerminadoId = 102, MateriaPrimaId = 22, Cantidad = 2 },
                new Formula { Id = 5, ProductoTerminadoId = 200, MateriaPrimaId = 2, Cantidad = 98 },
                new Formula { Id = 6, ProductoTerminadoId = 200, MateriaPrimaId = 20, Cantidad = 2 },
                new Formula { Id = 7, ProductoTerminadoId = 300, MateriaPrimaId = 3, Cantidad = 100 },
                new Formula { Id = 8, ProductoTerminadoId = 400, MateriaPrimaId = 4, Cantidad = 100 },
                new Formula { Id = 9, ProductoTerminadoId = 106, MateriaPrimaId = 7, Cantidad = 100 },
                // Formulas Fazon (50 en adelante)
                new Formula { Id = 50, ProductoTerminadoId = 900, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 51, ProductoTerminadoId = 901, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 52, ProductoTerminadoId = 902, MateriaPrimaId = 999, Cantidad = 98 },
                new Formula { Id = 53, ProductoTerminadoId = 902, MateriaPrimaId = 22, Cantidad = 2 },
                new Formula { Id = 54, ProductoTerminadoId = 903, MateriaPrimaId = 999, Cantidad = 98 },
                new Formula { Id = 55, ProductoTerminadoId = 903, MateriaPrimaId = 22, Cantidad = 2 },
                new Formula { Id = 56, ProductoTerminadoId = 904, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 57, ProductoTerminadoId = 905, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 58, ProductoTerminadoId = 906, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 59, ProductoTerminadoId = 907, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 60, ProductoTerminadoId = 908, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 61, ProductoTerminadoId = 909, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 62, ProductoTerminadoId = 910, MateriaPrimaId = 999, Cantidad = 100 },
                new Formula { Id = 63, ProductoTerminadoId = 911, MateriaPrimaId = 999, Cantidad = 100 }
            );
        }
    }
}