using Entity.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace Entity.Context { 

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DNI).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Telefono).HasMaxLength(15);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.DNI).IsUnique();
        });

        // Configuración Vehículo
        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Marca).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Modelo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Placa).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Color).HasMaxLength(30);
            entity.Property(e => e.Tipo).HasMaxLength(30);
            entity.Property(e => e.PrecioPorDia).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Placa).IsUnique();
        });

        // Configuración Reserva
        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.Observaciones).HasMaxLength(500);

            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Reservas)
                  .HasForeignKey(e => e.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Vehiculo)
                  .WithMany(v => v.Reservas)
                  .HasForeignKey(e => e.VehiculoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Filtros globales para borrado lógico
        modelBuilder.Entity<Cliente>().HasQueryFilter(e => e.EsActivo);
        modelBuilder.Entity<Vehiculo>().HasQueryFilter(e => e.EsActivo);
        modelBuilder.Entity<Reserva>().HasQueryFilter(e => e.EsActivo);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var baseEntity = (BaseEntity)entity.Entity;

            if (entity.State == EntityState.Modified)
            {
                baseEntity.FechaModificacion = DateTime.Now;
            }
        }
    }
}
}