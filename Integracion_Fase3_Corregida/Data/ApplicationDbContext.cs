using DAS_Proyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace DAS_Proyecto.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Servicio> Servicios { get; set; } = null!;
        public DbSet<Cotizacion> Cotizaciones { get; set; } = null!;
        public DbSet<OrdenTrabajo> OrdenesTrabajo { get; set; } = null!;
        public DbSet<DetalleOrdenMaterial> DetalleOrdenMateriales { get; set; } = null!;
        public DbSet<Material> Materiales { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Material>()
                .Property(m => m.CantidadDisponible)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Material>()
                .Property(m => m.CostoUnitario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.Cliente)
                .WithMany()
                .HasForeignKey(c => c.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.Servicio)
                .WithMany()
                .HasForeignKey(c => c.IdServicio)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdenTrabajo>()
                .HasOne(o => o.Cotizacion)
                .WithMany()
                .HasForeignKey(o => o.IdCotizacion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleOrdenMaterial>()
                .HasOne(d => d.OrdenTrabajo)
                .WithMany()
                .HasForeignKey(d => d.IdOrdenTrabajo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleOrdenMaterial>()
                .HasOne(d => d.Material)
                .WithMany()
                .HasForeignKey(d => d.IdMaterial)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .Property(p => p.MontoPago)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.OrdenTrabajo)
                .WithMany(o => o.Pagos)
                .HasForeignKey(p => p.IdOrdenTrabajo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasIndex(p => p.CodigoPago)
                .IsUnique();

            // Una orden no puede tener dos anticipos iniciales.
            modelBuilder.Entity<Pago>()
                .HasIndex(p => new { p.IdOrdenTrabajo, p.FechaPago })
                .HasDatabaseName("IX_Pagos_IdOrdenTrabajo");

            modelBuilder.Entity<Pago>()
                .HasIndex(p => p.IdOrdenTrabajo)
                .HasDatabaseName("IX_Pagos_AnticipoPorOrden")
                .IsUnique()
                .HasFilter("[TipoPago] = 0");
        }
    }
}
