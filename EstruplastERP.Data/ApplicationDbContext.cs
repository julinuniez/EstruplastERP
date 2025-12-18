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
            
            // NUEVO: Precisión para los rangos de validación técnica
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
                .HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsumoOrden>(entity =>
            {
                entity.HasOne(c => c.MateriaPrima)
                      .WithMany()
                      .HasForeignKey(c => c.MateriaPrimaId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // =============================================================================
            // SEED DATA
            // =============================================================================
            modelBuilder.Entity<Producto>().HasData(
                // 1. MATERIAS PRIMAS
                new Producto { Id = 1, Nombre = "Poliestireno Alto Impacto (AI/PAI)", CodigoSku = "MP-PAI", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 1.1m, StockActual = 1000, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 2, Nombre = "ABS (Acrilonitrilo Butadieno Estireno)", CodigoSku = "MP-ABS", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 1.1M, StockActual = 1000, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 3, Nombre = "Polipropileno (PP)", CodigoSku = "MP-PP", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 1m, StockActual = 1000, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 4, Nombre = "Polietileno Alta Densidad (PEAD)", CodigoSku = "MP-PEAD", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 1m, StockActual = 1000, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 5, Nombre = "Polietileno Baja Densidad (PEBD)", CodigoSku = "MP-PEBD", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 1m, StockActual = 1000, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                
                // NUEVOS: TUTI (RECUPERADO) SEGÚN LISTA DE PRECIOS
                // Fino: 0.40mm - 0.90mm
                new Producto { Id = 6, Nombre = "Tuti Fino (0.40 - 0.90)", CodigoSku = "MP-TUTI-FINO", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.1m, StockActual = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now, EspesorMinimo = 0.40m, EspesorMaximo = 0.90m },
                // Grueso: > 0.90mm
                new Producto { Id = 15, Nombre = "Tuti Grueso", CodigoSku = "MP-TUTI-GRUESO", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.1m, StockActual = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now, EspesorMinimo = 0.91m, EspesorMaximo = 5.00m },

                new Producto { Id = 7, Nombre = "Bioplástico Compostable (Biolam)", CodigoSku = "MP-BIO", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 1.25m, StockActual = 0, StockMinimo = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 8, Nombre = "Masterbatch Blanco (Titanio)", CodigoSku = "MP-MST-BLA", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 1.80m, StockActual = 500, StockMinimo = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 9, Nombre = "Masterbatch Negro", CodigoSku = "MP-MST-NEG", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 500, StockMinimo = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 10, Nombre = "Aditivo Genérico", CodigoSku = "MP-ADITIVO-GEN", EsMateriaPrima = true, EsProductoTerminado = false, EsGenerico = false, PesoEspecifico = 0.95m, StockActual = 0, StockMinimo = 50, Activo = true, FechaCreacion = DateTime.Now },

                // COLORES
                new Producto { Id = 11, Nombre = "Masterbatch Rojo", CodigoSku = "MP-MST-ROJ", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 12, Nombre = "Masterbatch Azul", CodigoSku = "MP-MST-AZU", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 13, Nombre = "Masterbatch Verde", CodigoSku = "MP-MST-VER", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 14, Nombre = "Masterbatch Amarillo", CodigoSku = "MP-MST-AMA", EsMateriaPrima = true, EsGenerico = false, PesoEspecifico = 1.20m, StockActual = 100, Activo = true, FechaCreacion = DateTime.Now },

                // ADITIVOS DE PRODUCCIÓN
                new Producto { Id = 30, Nombre = "Aditivo Brillo", CodigoSku = "MP-BRILLO", EsMateriaPrima = true, EsGenerico = false, StockActual = 500, PesoEspecifico = 1m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 31, Nombre = "Estearato de Zinc", CodigoSku = "MP-ESTEARATO", EsMateriaPrima = true, EsGenerico = false, StockActual = 200, PesoEspecifico = 0.35m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 32, Nombre = "Carga Mineral (Carbonato)", CodigoSku = "MP-CARGA", EsMateriaPrima = true, EsGenerico = false, StockActual = 2000, PesoEspecifico = 1.80m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 33, Nombre = "Aditivo UV", CodigoSku = "MP-UV", EsMateriaPrima = true, EsGenerico = false, StockActual = 200, PesoEspecifico = 0.95m, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 34, Nombre = "Aditivo Caucho", CodigoSku = "MP-CAUCHO", EsMateriaPrima = true, EsGenerico = false, StockActual = 200, PesoEspecifico = 0.94m, Activo = true, FechaCreacion = DateTime.Now },

                // 2. PRODUCTOS GENÉRICOS (PADRES)
                new Producto { Id = 50, Nombre = "Material PAI Blanco", CodigoSku = "MAT-PAI-B", EsProductoTerminado = true, EsMateriaPrima = false, EsGenerico = true, PesoEspecifico = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 51, Nombre = "Material PAI Negro", CodigoSku = "MAT-PAI-N", EsProductoTerminado = true, EsMateriaPrima = false, EsGenerico = true, PesoEspecifico = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 52, Nombre = "Material PAI Color", CodigoSku = "MAT-PAI-C", EsProductoTerminado = true, EsMateriaPrima = false, EsGenerico = true, PesoEspecifico = 0, StockMinimo = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 53, Nombre = "Material PAI Bicapa", CodigoSku = "MAT-PAI-BIC", EsProductoTerminado = true, EsMateriaPrima = false, EsGenerico = true, PesoEspecifico = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 54, Nombre = "Material PAI Tricapa", CodigoSku = "MAT-PAI-TRI", EsProductoTerminado = true, EsMateriaPrima = false, EsGenerico = true, PesoEspecifico = 0, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },

                new Producto { Id = 60, Nombre = "Material ABS Blanco", CodigoSku = "MAT-ABS-B", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 300, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 61, Nombre = "Material ABS Negro/Color", CodigoSku = "MAT-ABS-C", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 300, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 70, Nombre = "Material PP", CodigoSku = "MAT-PP", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 80, Nombre = "Material PEAD", CodigoSku = "MAT-PEAD", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 81, Nombre = "Material PEBD", CodigoSku = "MAT-PEBD", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 1000, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 90, Nombre = "Material Biolam", CodigoSku = "MAT-BIO", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 200, Activo = true, FechaCreacion = DateTime.Now },
                new Producto { Id = 99, Nombre = "Lámina PET (Reventa)", CodigoSku = "REV-PET", EsProductoTerminado = true, EsGenerico = true, StockMinimo = 500, Activo = true, FechaCreacion = DateTime.Now },

                // 3. PRODUCTOS ESTÁNDAR
                new Producto { Id = 100, ProductoPadreId = 50, Nombre = "Lámina PAI Blanco 1000x2000x0.5 mm", CodigoSku = "STD-PAI-1000-05", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 0.5m },
                new Producto { Id = 101, ProductoPadreId = 50, Nombre = "Lámina PAI Blanco 1000x2000x1.0 mm", CodigoSku = "STD-PAI-1000-10", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 1.0m },
                new Producto { Id = 102, ProductoPadreId = 50, Nombre = "Lámina PAI Blanco 1000x2000x1.5 mm", CodigoSku = "STD-PAI-1000-15", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 1.5m },

                new Producto { Id = 103, ProductoPadreId = 50, Nombre = "Lámina PAI Blanco 1220x2440x0.5 mm", CodigoSku = "STD-PAI-1220-05", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2440, Ancho = 1220, Espesor = 0.5m },
                new Producto { Id = 104, ProductoPadreId = 50, Nombre = "Lámina PAI Blanco 1220x2440x1.0 mm", CodigoSku = "STD-PAI-1220-10", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2440, Ancho = 1220, Espesor = 1.0m },
                new Producto { Id = 105, ProductoPadreId = 50, Nombre = "Lámina PAI Blanco 1220x2440x1.5 mm", CodigoSku = "STD-PAI-1220-15", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2440, Ancho = 1220, Espesor = 1.5m },

                // PAI BICAPA
                new Producto { Id = 110, ProductoPadreId = 53, Nombre = "Lámina PAI Bicapa 1000x2000x1.0 mm", CodigoSku = "STD-BIC-1000-10", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 1.0m },
                new Producto { Id = 111, ProductoPadreId = 53, Nombre = "Lámina PAI Bicapa 1000x2000x1.5 mm", CodigoSku = "STD-BIC-1000-15", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 1.5m },
                new Producto { Id = 112, ProductoPadreId = 53, Nombre = "Lámina PAI Bicapa 1220x2440x1.0 mm", CodigoSku = "STD-BIC-1220-10", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2440, Ancho = 1220, Espesor = 1.0m },
                new Producto { Id = 113, ProductoPadreId = 53, Nombre = "Lámina PAI Bicapa 1220x2440x1.5 mm", CodigoSku = "STD-BIC-1220-15", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2440, Ancho = 1220, Espesor = 1.5m },

                // PAI TRICAPA
                new Producto { Id = 120, ProductoPadreId = 54, Nombre = "Lámina PAI Tricapa 1000x2000x1.0 mm", CodigoSku = "STD-TRI-1000-10", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 1.0m },
                new Producto { Id = 121, ProductoPadreId = 54, Nombre = "Lámina PAI Tricapa 1000x2000x1.5 mm", CodigoSku = "STD-TRI-1000-15", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 1.5m },
                new Producto { Id = 122, ProductoPadreId = 54, Nombre = "Lámina PAI Tricapa 1220x2440x1.0 mm", CodigoSku = "STD-TRI-1220-10", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2440, Ancho = 1220, Espesor = 1.0m },
                new Producto { Id = 123, ProductoPadreId = 54, Nombre = "Lámina PAI Tricapa 1220x2440x1.5 mm", CodigoSku = "STD-TRI-1220-15", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.05m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2440, Ancho = 1220, Espesor = 1.5m },

                // PET
                new Producto { Id = 130, ProductoPadreId = 99, Nombre = "Lámina PET 1000x2000x0.50 mm", CodigoSku = "STD-PET-050", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.38m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 0.50m },
                new Producto { Id = 131, ProductoPadreId = 99, Nombre = "Lámina PET 1000x2000x0.80 mm", CodigoSku = "STD-PET-080", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.38m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 0.80m },
                new Producto { Id = 132, ProductoPadreId = 99, Nombre = "Lámina PET 1000x2000x1.00 mm", CodigoSku = "STD-PET-100", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.38m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 1.00m },

                // BIOLAM
                new Producto { Id = 140, ProductoPadreId = 90, Nombre = "Lámina BIOLAM 1000x2000x0.50 mm", CodigoSku = "STD-BIO-050", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.25m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 0.50m },
                new Producto { Id = 141, ProductoPadreId = 90, Nombre = "Lámina BIOLAM 1000x2000x0.70 mm", CodigoSku = "STD-BIO-070", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.25m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 0.70m },
                new Producto { Id = 142, ProductoPadreId = 90, Nombre = "Lámina BIOLAM 1000x2000x1.00 mm", CodigoSku = "STD-BIO-100", EsProductoTerminado = true, EsGenerico = false, PesoEspecifico = 1.25m, Activo = true, StockActual = 0, FechaCreacion = DateTime.Now, Largo = 2000, Ancho = 1000, Espesor = 1.00m }
            );

            // RECETAS TÍPICAS
            modelBuilder.Entity<Formula>().HasData(
                new Formula { Id = 200, ProductoTerminadoId = 50, MateriaPrimaId = 1, Cantidad = 98 },
                new Formula { Id = 201, ProductoTerminadoId = 50, MateriaPrimaId = 8, Cantidad = 2 },
                new Formula { Id = 202, ProductoTerminadoId = 51, MateriaPrimaId = 1, Cantidad = 98 },
                new Formula { Id = 203, ProductoTerminadoId = 51, MateriaPrimaId = 9, Cantidad = 2 },
                new Formula { Id = 204, ProductoTerminadoId = 52, MateriaPrimaId = 1, Cantidad = 100 },
                new Formula { Id = 205, ProductoTerminadoId = 70, MateriaPrimaId = 3, Cantidad = 100 }
                //new Formula { Id = 206, ProductoTerminadoId = 53, MateriaPrimaId = , Cantidad = }
            );
        }
    }
}