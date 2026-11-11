
using Inventario.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Data
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(
            DbContextOptions<InventarioDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bodega> Bodegas { get; set; }

        public DbSet<Existencia> Existencias { get; set; }

        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }

        public DbSet<ReservaStock> ReservasStock { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Existencia>()
                .HasIndex(e => new
                {
                    e.IdProducto,
                    e.IdBodega
                })
                .IsUnique();

            modelBuilder.Entity<Existencia>()
                .HasOne(e => e.Bodega)
                .WithMany()
                .HasForeignKey(e => e.IdBodega)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
