using FashionPay.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace FashionPay.Core.Data;

public partial class FashionPayContext : DbContext
{
    public FashionPayContext()
    {
    }

    public FashionPayContext(DbContextOptions<FashionPayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abono> Abonos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<DetalleCompra> DetalleCompras { get; set; }

    public virtual DbSet<EstadoCuenta> EstadoCuenta { get; set; }

    public virtual DbSet<PlanPago> PlanPagos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=RYNL044\\JOSETRYNDEMSQL;Database=FashionPay;User Id=sa;Password=root;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abono>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Abono__3214EC07C82FE99B");

            entity.ToTable("Abono", tb => tb.HasTrigger("tr_Abono_ActualizarEstado"));

            entity.HasIndex(e => e.ClienteId, "IX_Abono_Cliente");

            entity.HasIndex(e => e.FechaAbono, "IX_Abono_Fecha");

            entity.HasIndex(e => e.PlanPagoId, "IX_Abono_PlanPago");

            entity.HasIndex(e => e.NumeroRecibo, "IX_Abono_Recibo");

            entity.HasIndex(e => e.NumeroRecibo, "UQ__Abono__F83E3F2962C31EF4").IsUnique();

            entity.Property(e => e.FechaAbono).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FormaPago)
                .HasMaxLength(30)
                .HasDefaultValue("EFECTIVO");
            entity.Property(e => e.MontoAbono).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NumeroRecibo).HasMaxLength(20);
            entity.Property(e => e.Observaciones).HasMaxLength(300);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abono__ClienteId__656C112C");

            entity.HasOne(d => d.PlanPago).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.PlanPagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abono__PlanPagoI__66603565");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cliente__3214EC07CC5F750F");

            entity.ToTable("Cliente");

            entity.HasIndex(e => e.Email, "IX_Cliente_Email");

            entity.HasIndex(e => e.Email, "UQ__Cliente__A9D105349C5B9B61").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CantidadMaximaPagos).HasDefaultValue(12);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LimiteCredito).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PeriodicidadPago).HasDefaultValue(30);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.ToleranciasMorosidad).HasDefaultValue(5);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Compra__3214EC07D2DAB502");

            entity.ToTable("Compra");

            entity.HasIndex(e => e.ClienteId, "IX_Compra_Cliente");

            entity.HasIndex(e => e.FechaCompra, "IX_Compra_Fecha");

            entity.HasIndex(e => e.NumeroCompra, "IX_Compra_Numero");

            entity.HasIndex(e => e.NumeroCompra, "UQ__Compra__5F9B8DECFE4A6E6E").IsUnique();

            entity.Property(e => e.FechaCompra).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MontoMensual).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NumeroCompra).HasMaxLength(20);
            entity.Property(e => e.Observaciones).HasMaxLength(500);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Compras)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__ClienteI__5165187F");
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DetalleC__3214EC07F6AB7417");

            entity.ToTable("DetalleCompra");

            entity.HasIndex(e => e.CompraId, "IX_DetalleCompra_Compra");

            entity.HasIndex(e => e.ProductoId, "IX_DetalleCompra_Producto");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Compra).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.CompraId)
                .HasConstraintName("FK__DetalleCo__Compr__5629CD9C");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleCo__Produ__571DF1D5");
        });

        modelBuilder.Entity<EstadoCuenta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadoCu__3214EC07E1207FB0");

            entity.HasIndex(e => e.Clasificacion, "IX_EstadoCuenta_Clasificacion");

            entity.HasIndex(e => e.ClienteId, "UQ__EstadoCu__71ABD0865A8F54CD").IsUnique();

            entity.Property(e => e.Clasificacion)
                .HasMaxLength(20)
                .HasDefaultValue("CUMPLIDO");
            entity.Property(e => e.DeudaTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FechaActualizacion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Cliente).WithOne(p => p.EstadoCuenta)
                .HasForeignKey<EstadoCuenta>(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EstadoCue__Clien__70DDC3D8");
        });

        modelBuilder.Entity<PlanPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlanPago__3214EC072F27DCBB");

            entity.ToTable("PlanPago");

            entity.HasIndex(e => e.CompraId, "IX_PlanPago_Compra");

            entity.HasIndex(e => e.Estado, "IX_PlanPago_Estado");

            entity.HasIndex(e => e.FechaVencimiento, "IX_PlanPago_Vencimiento");

            entity.HasIndex(e => new { e.CompraId, e.NumeroPago }, "UQ__PlanPago__A1B2D5F68B81E4AE").IsUnique();

            entity.Property(e => e.DiasVencidos).HasComputedColumnSql("(case when [Estado]='VENCIDO' then datediff(day,[FechaVencimiento],getdate()) else (0) end)", false);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("PENDIENTE");
            entity.Property(e => e.MontoPagado).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MontoProgramado).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SaldoPendiente).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Compra).WithMany(p => p.PlanPagos)
                .HasForeignKey(d => d.CompraId)
                .HasConstraintName("FK__PlanPago__Compra__5CD6CB2B");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC07B1F738E5");

            entity.ToTable("Producto");

            entity.HasIndex(e => e.Codigo, "IX_Producto_Codigo");

            entity.HasIndex(e => e.ProveedorId, "IX_Producto_Proveedor");

            entity.HasIndex(e => e.Codigo, "UQ__Producto__06370DAC0CC5A9B6").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Productos)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Producto__Provee__4CA06362");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Proveedo__3214EC0722E07696");

            entity.ToTable("Proveedor");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Contacto).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
        ConfigureCustomEntities(modelBuilder);
    }
    private void ConfigureCustomEntities(ModelBuilder modelBuilder)
    {
        // Configurar propiedades de auditoria
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.Property(e => e.FechaCompra)
                .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Abono>(entity =>
        {
            entity.Property(e => e.FechaAbono)
                .HasDefaultValueSql("GETDATE()");
        });

        // Configurar campos calculados
        modelBuilder.Entity<PlanPago>(entity =>
        {
            entity.Property(e => e.DiasVencidos)
                .HasComputedColumnSql("CASE WHEN Estado = 'VENCIDO' THEN DATEDIFF(DAY, FechaVencimiento, GETDATE()) ELSE 0 END");
        });

        // Configurar índices para performance
        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Email)
            .IsUnique()
            .HasDatabaseName("IX_Cliente_Email");

        modelBuilder.Entity<Compra>()
            .HasIndex(c => new { c.ClienteId, c.FechaCompra })
            .HasDatabaseName("IX_Compra_Cliente_Fecha");

        modelBuilder.Entity<PlanPago>()
            .HasIndex(p => new { p.CompraId, p.Estado })
            .HasDatabaseName("IX_PlanPago_Compra_Estado")
            .IncludeProperties(p => p.SaldoPendiente);

        // Configurar comportamiento de eliminación
        modelBuilder.Entity<Compra>()
            .HasMany(c => c.DetalleCompras)
            .WithOne(d => d.Compra)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Compra>()
            .HasMany(c => c.PlanPagos)
            .WithOne(p => p.Compra)
            .OnDelete(DeleteBehavior.Cascade);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
public static class FashionPayContextExtensions
{
    public static IServiceCollection AddFashionPayContext(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<FashionPayContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            // Configuraciones adicionales para desarrollo
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        return services;
    }
}