using Microsoft.EntityFrameworkCore;
using DAS_Proyecto.Models;

namespace DAS_Proyecto.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //public DbSet<Cliente> Clientes { get; set; }
        //public DbSet<Usuario> Usuarios { get; set; }
        //public DbSet<Servicio> Servicios { get; set; }
        //public DbSet<Cotizacion> Cotizaciones { get; set; }
        //public DbSet<OrdenTrabajo> OrdenesTrabajo { get; set; }
        //public DbSet<DetalleOrdenMaterial> DetalleOrdenMateriales { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de precisión decimal obligatoria para materiales
            modelBuilder.Entity<Material>().Property(m => m.CantidadDisponible).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Material>().Property(m => m.CostoUnitario).HasColumnType("decimal(18,2)");

            // Configuración de precisión decimal para pagos
            modelBuilder.Entity<Pago>().Property(p => p.MontoPago).HasColumnType("decimal(18,2)"); ;
        }
    }
}