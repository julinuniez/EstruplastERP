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
    }
}