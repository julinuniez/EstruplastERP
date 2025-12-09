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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}