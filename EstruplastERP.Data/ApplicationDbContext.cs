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
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produccion> Producciones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Remito> Remitos { get; set; }
        public DbSet<RemitoDetalle> RemitoDetalles { get; set; }
        public DbSet<OrdenProduccion> Ordenes { get; set; }
        public DbSet<ConsumoOrden> ConsumosOrdenes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<ClienteMaterialFazon> ClientesMaterialesFazon { get; set; }
        public DbSet<HojaCarga> HojasCarga { get; set; }
        public DbSet<ConsumoHojaCarga> ConsumosHojasCarga { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =============================================================================
            // 0. CONSTANTES DE FAMILIAS (Para mantener el orden lógico)
            // =============================================================================
            int FAM_AI = 10;   // Alto Impacto
            int FAM_ABS = 20;  // ABS
            int FAM_PP = 30;   // Polipropileno
            int FAM_PE = 40;   // Polietileno (PEAD/PEBD)
            int FAM_MB = 50;   // Masterbatch / Aditivos
            int FAM_FREON = 60;

            // =============================================================================
            // CONFIGURACIONES DE PRECISIÓN
            // =============================================================================
            modelBuilder.Entity<Producto>().Property(p => p.PesoEspecifico).HasPrecision(18, 4);
            modelBuilder.Entity<Producto>().Property(p => p.PrecioCosto).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.StockActual).HasPrecision(18, 3);
            modelBuilder.Entity<Producto>().Property(p => p.StockMinimo).HasPrecision(18, 3);
            modelBuilder.Entity<Producto>().Property(p => p.EspesorMinimo).HasPrecision(18, 2);
            modelBuilder.Entity<Producto>().Property(p => p.EspesorMaximo).HasPrecision(18, 2);

            modelBuilder.Entity<Formula>().Property(f => f.Cantidad).HasPrecision(18, 4);
            modelBuilder.Entity<Movimiento>().Property(m => m.Cantidad).HasPrecision(18, 3);
            modelBuilder.Entity<Produccion>().Property(p => p.Cantidad).HasPrecision(18, 2);
            modelBuilder.Entity<Produccion>().Property(p => p.Kilos).HasPrecision(18, 3);
            modelBuilder.Entity<RemitoDetalle>().Property(r => r.Cantidad).HasPrecision(18, 3);
            modelBuilder.Entity<RemitoDetalle>().Property(r => r.PrecioUnitarioSnapshot).HasPrecision(18, 2);

            // =============================================================================
            // RELACIONES
            // =============================================================================
            modelBuilder.Entity<Formula>()
                .HasOne(f => f.ProductoTerminado).WithMany(p => p.Formulas).HasForeignKey(f => f.ProductoTerminadoId).OnDelete(DeleteBehavior.Restrict);

            // 👇 SE DEJÓ SOLO UNA VERSIÓN (Cascade Delete) PARA PERMITIR LIMPIAR FÓRMULAS AL BORRAR MP
            modelBuilder.Entity<Formula>()
                .HasOne(f => f.MateriaPrima)
                .WithMany()
                .HasForeignKey(f => f.MateriaPrimaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Cliente).WithMany().HasForeignKey(p => p.ClienteId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsumoOrden>(entity =>
            {
                entity.HasOne(c => c.MateriaPrima).WithMany().HasForeignKey(c => c.MateriaPrimaId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ClienteMaterialFazon>()
                .HasOne(c => c.MaterialGenerico).WithMany().HasForeignKey(c => c.MaterialGenericoId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClienteMaterialFazon>()
                .HasOne(c => c.MaterialReal).WithMany().HasForeignKey(c => c.MaterialRealId).OnDelete(DeleteBehavior.Restrict);

            // =============================================================================
            // SEEDING (DATOS INICIALES CON FAMILIA ID)
            // =============================================================================
            modelBuilder.Entity<Producto>().HasData(

                // 1. MATERIAS PRIMAS GENÉRICAS (FAZÓN BASE)
                new Producto { Id = 999, Nombre = "MATERIAL DE CLIENTE (GENÉRICO)", CodigoSku = "MP-FAZON-GEN", Rubro = "SERVICIO FAZON", EsMateriaPrima = true, PesoEspecifico = 1.00m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = null },

                new Producto { Id = 990, Nombre = "MP FAZÓN ALTO IMPACTO (BASE)", CodigoSku = "MP-FAZ-AI", Rubro = "SERVICIO FAZON", EsMateriaPrima = true, PesoEspecifico = 1.1m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_AI },
                new Producto { Id = 991, Nombre = "MP FAZÓN ABS (BASE)", CodigoSku = "MP-FAZ-ABS", Rubro = "SERVICIO FAZON", EsMateriaPrima = true, PesoEspecifico = 1.1m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_ABS },
                new Producto { Id = 992, Nombre = "MP FAZÓN POLIPROPILENO (BASE)", CodigoSku = "MP-FAZ-PP", Rubro = "SERVICIO FAZON", EsMateriaPrima = true, PesoEspecifico = 0.91m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_PP },
                new Producto { Id = 993, Nombre = "MP FAZÓN PEAD/PEBD (BASE)", CodigoSku = "MP-FAZ-PE", Rubro = "SERVICIO FAZON", EsMateriaPrima = true, PesoEspecifico = 0.96m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_PE },
                new Producto { Id = 994, Nombre = "MP FAZÓN FREON (BASE)", CodigoSku = "MP-FAZ-FREON", Rubro = "SERVICIO FAZON", EsMateriaPrima = true, PesoEspecifico = 1.1m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_FREON },
                new Producto { Id = 22, Nombre = "Masterbatch Color (Varios)", CodigoSku = "MP-MB-COL", Rubro = "MATERIA PRIMA", EsMateriaPrima = true, PesoEspecifico = 1.1m, StockActual = 0, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_MB },

                // 2. PRODUCTOS TERMINADOS (PROPIOS)
                new Producto { Id = 100, Nombre = "A.I. FINO (0.40 - 0.90 mm)", CodigoSku = "AI-FINO", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 102, Nombre = "A.I. FINO COLOR", CodigoSku = "AI-FINO-COL", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 106, Nombre = "A.I. TUTTI FINO", CodigoSku = "AI-TUTTI-FINO", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },

                new Producto { Id = 101, Nombre = "A.I. GRUESO", CodigoSku = "AI-GRUESO", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 103, Nombre = "A.I. GRUESO COLOR", CodigoSku = "AI-GRUESO-COL", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 104, Nombre = "A.I. BICAPA", CodigoSku = "AI-BICAPA", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 105, Nombre = "A.I. TRICAPA", CodigoSku = "AI-TRICAPA", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 107, Nombre = "A.I. TUTTI GRUESO", CodigoSku = "AI-TUTTI-GRUESO", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 108, Nombre = "A.I. RESISTENTE AL FREON", CodigoSku = "AI-FREON", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 109, Nombre = "A.I. RESISTENTE AL FREON COLOR", CodigoSku = "AI-FREON-COL", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },

                new Producto { Id = 200, Nombre = "ABS BLANCO", CodigoSku = "ABS-BLA", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 201, Nombre = "ABS COLOR", CodigoSku = "ABS-COL", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 202, Nombre = "ABS GRUESO (Min 1mm)", CodigoSku = "ABS-GRUESO", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 1.00m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },

                new Producto { Id = 300, Nombre = "PP (POLIPROPILENO)", CodigoSku = "PP-STD", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.91m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 301, Nombre = "PP COLOR", CodigoSku = "PP-COL", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.91m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },

                new Producto { Id = 400, Nombre = "PEAD / PEBD", CodigoSku = "PE-MIX", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.94m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 401, Nombre = "PEBD GOFRADO", CodigoSku = "PEBD-GOF", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.92m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 402, Nombre = "PEAD BICAPA", CodigoSku = "PEAD-BIC", Rubro = "PRODUCTO TERMINADO", EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.96m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },

                new Producto { Id = 900, Nombre = "FAZON - A.I. FINO", CodigoSku = "FAZ-AI-FIN", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 902, Nombre = "FAZON - A.I. FINO COLOR", CodigoSku = "FAZ-AI-FIN-COL", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 906, Nombre = "FAZON - A.I. TUTTI FINO", CodigoSku = "FAZ-AI-TUT-FIN", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now },

                new Producto { Id = 901, Nombre = "FAZON - A.I. GRUESO", CodigoSku = "FAZ-AI-GRU", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 903, Nombre = "FAZON - A.I. GRUESO COLOR", CodigoSku = "FAZ-AI-GRU-COL", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 904, Nombre = "FAZON - A.I. BICAPA", CodigoSku = "FAZ-AI-BIC", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 905, Nombre = "FAZON - A.I. TRICAPA", CodigoSku = "FAZ-AI-TRI", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 907, Nombre = "FAZON - A.I. TUTTI GRUESO", CodigoSku = "FAZ-AI-TUT-GRU", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 908, Nombre = "FAZON - ABS GRUESO", CodigoSku = "FAZ-ABS-GRU", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },

                new Producto { Id = 911, Nombre = "FAZON - PEAD BICAPA", CodigoSku = "FAZ-PEAD-BIC", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 0.96m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 912, Nombre = "FAZON - RESISTENTE FREON FINO", CodigoSku = "FAZ-FREON-FIN", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_FREON },
                new Producto { Id = 913, Nombre = "FAZON - RESISTENTE FREON GRUESO", CodigoSku = "FAZ-FREON-GRU", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_FREON },
                new Producto { Id = 914, Nombre = "FAZON - RESISTENTE FREON COLOR", CodigoSku = "FAZ-FREON-COL", Rubro = "SERVICIO FAZON", EsFazon = true, EsProductoTerminado = true, EsGenerico = true, PesoEspecifico = 1.05m, EspesorMinimo = 0.90m, EspesorMaximo = 0m, Activo = true, FechaCreacion = DateTime.Now, FamiliaId = FAM_FREON }
            );

            // =============================================================================
            // 4. RECETAS (FÓRMULAS)
            // =============================================================================

            modelBuilder.Entity<Formula>().HasData(
                new Formula { Id = 50, ProductoTerminadoId = 900, MateriaPrimaId = 990, Cantidad = 100 },
                new Formula { Id = 51, ProductoTerminadoId = 901, MateriaPrimaId = 990, Cantidad = 100 },
                new Formula { Id = 52, ProductoTerminadoId = 902, MateriaPrimaId = 990, Cantidad = 98 },
                new Formula { Id = 53, ProductoTerminadoId = 902, MateriaPrimaId = 22, Cantidad = 2 },
                new Formula { Id = 54, ProductoTerminadoId = 903, MateriaPrimaId = 990, Cantidad = 98 },
                new Formula { Id = 55, ProductoTerminadoId = 903, MateriaPrimaId = 22, Cantidad = 2 },
                new Formula { Id = 56, ProductoTerminadoId = 904, MateriaPrimaId = 990, Cantidad = 100 },
                new Formula { Id = 57, ProductoTerminadoId = 905, MateriaPrimaId = 990, Cantidad = 100 },

                new Formula { Id = 58, ProductoTerminadoId = 906, MateriaPrimaId = 990, Cantidad = 100 },
                new Formula { Id = 59, ProductoTerminadoId = 907, MateriaPrimaId = 990, Cantidad = 100 },

                new Formula { Id = 60, ProductoTerminadoId = 908, MateriaPrimaId = 991, Cantidad = 100 },

                new Formula { Id = 63, ProductoTerminadoId = 911, MateriaPrimaId = 993, Cantidad = 100 },
                new Formula { Id = 64, ProductoTerminadoId = 912, MateriaPrimaId = 994, Cantidad = 100 },
                new Formula { Id = 65, ProductoTerminadoId = 913, MateriaPrimaId = 994, Cantidad = 100 },
                new Formula { Id = 66, ProductoTerminadoId = 914, MateriaPrimaId = 994, Cantidad = 100 }
            );
        }
    }
}