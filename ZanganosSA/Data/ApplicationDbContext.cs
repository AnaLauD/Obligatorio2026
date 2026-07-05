using Microsoft.EntityFrameworkCore;
using ZanganosSA.Models;

namespace ZanganosSA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Apiario> Apiarios { get; set; }
        public DbSet<Colmena> Colmenas { get; set; }
        public DbSet<Inspeccion> Inspecciones { get; set; }
        public DbSet<Cosecha> Cosechas { get; set; }
        public DbSet<Barril> Barriles { get; set; }
        public DbSet<Alimentacion> Alimentaciones { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }
        public DbSet<ColmenaCosecha> ColmenaCosechas { get; set; }
        public DbSet<ColmenaTratamiento> ColmenaTratamientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relación N a M entre Colmena y Cosecha
            modelBuilder.Entity<ColmenaCosecha>()
                .HasKey(cc => new { cc.CosechaId, cc.ColmenaId });

            modelBuilder.Entity<ColmenaCosecha>()
                .HasOne(cc => cc.Colmena)
                .WithMany(c => c.ColmenaCosechas)
                .HasForeignKey(cc => cc.ColmenaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ColmenaCosecha>()
                .HasOne(cc => cc.Cosecha)
                .WithMany(c => c.ColmenaCosechas)
                .HasForeignKey(cc => cc.CosechaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar relación N a M entre Colmena y Tratamiento
            modelBuilder.Entity<ColmenaTratamiento>()
                .HasKey(ct => new { ct.ColmenaId, ct.TratamientoId, ct.FechaAplicacion });

            modelBuilder.Entity<ColmenaTratamiento>()
                .HasOne(ct => ct.Colmena)
                .WithMany(c => c.ColmenaTratamientos)
                .HasForeignKey(ct => ct.ColmenaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ColmenaTratamiento>()
                .HasOne(ct => ct.Tratamiento)
                .WithMany(t => t.ColmenaTratamientos)
                .HasForeignKey(ct => ct.TratamientoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
