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
 => optionsBuilder.UseSqlServer("Server=RYNL044\\JOSETRYNDEMSQL;Database=FashionPayCore;User Id=sa;Password=root;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abono>(entity =>
        {
            entity.HasKey(e => e.IdAbono).HasName("PK__Abono__A4693DA769CDAFF0");

            entity.ToTable("Abono", tb => tb.HasTrigger("tr_Abono_ActualizarEstado"));

            entity.HasIndex(e => e.IdCliente, "IX_Abono_Cliente");

            entity.HasIndex(e => new { e.IdCliente, e.FechaAbono }, "IX_Abono_Cliente_Fecha").IsDescending(false, true);

            entity.HasIndex(e => e.FechaAbono, "IX_Abono_Fecha");

            entity.HasIndex(e => e.IdPlanPago, "IX_Abono_PlanPago");

            entity.HasIndex(e => e.NumeroRecibo, "IX_Abono_Recibo");

            entity.HasIndex(e => e.NumeroRecibo, "UQ__Abono__F83E3F29569D76C9").IsUnique();

            entity.Property(e => e.FechaAbono).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FormaPago)
                .HasMaxLength(30)
                .HasDefaultValue("EFECTIVO");
            entity.Property(e => e.MontoAbono).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NumeroRecibo).HasMaxLength(20);
            entity.Property(e => e.Observaciones).HasMaxLength(300);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abono__IdCliente__7B5B524B");

            entity.HasOne(d => d.IdPlanPagoNavigation).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.IdPlanPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Abono__IdPlanPag__7C4F7684");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Cliente__D5946642746E205C");

            entity.ToTable("Cliente");

            entity.HasIndex(e => e.Email, "IX_Cliente_Email");

            entity.HasIndex(e => e.Email, "UQ__Cliente__A9D10534791E98E1").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CantidadMaximaPagos).HasDefaultValue(12);
            entity.Property(e => e.CreditoDisponible)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
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
            entity.HasKey(e => e.IdCompra).HasName("PK__Compra__0A5CDB5CCF6E6951");

            entity.ToTable("Compra");

            entity.HasIndex(e => e.IdCliente, "IX_Compra_Cliente");

            entity.HasIndex(e => new { e.IdCliente, e.FechaCompra }, "IX_Compra_Cliente_Fecha").IsDescending(false, true);

            entity.HasIndex(e => e.FechaCompra, "IX_Compra_Fecha");

            entity.HasIndex(e => e.NumeroCompra, "IX_Compra_Numero");

            entity.HasIndex(e => e.NumeroCompra, "UQ__Compra__5F9B8DEC18521F02").IsUnique();

            entity.Property(e => e.EstadoCompra)
                .HasMaxLength(20)
                .HasDefaultValue("ACTIVA");
            entity.Property(e => e.FechaCompra).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MontoMensual).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NumeroCompra).HasMaxLength(20);
            entity.Property(e => e.Observaciones).HasMaxLength(500);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__IdClient__6754599E");
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCompra).HasName("PK__DetalleC__E046CCBBC2259B0A");

            entity.ToTable("DetalleCompra");

            entity.HasIndex(e => e.IdCompra, "IX_DetalleCompra_Compra");

            entity.HasIndex(e => e.IdProducto, "IX_DetalleCompra_Producto");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdCompra)
                .HasConstraintName("FK__DetalleCo__IdCom__6C190EBB");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleCo__IdPro__6D0D32F4");
        });

        modelBuilder.Entity<EstadoCuenta>(entity =>
        {
            entity.HasKey(e => e.IdEstadoCuenta).HasName("PK__EstadoCu__79FBA946D5FDEF93");

            entity.HasIndex(e => e.Clasificacion, "IX_EstadoCuenta_Clasificacion");

            entity.HasIndex(e => e.IdCliente, "UQ__EstadoCu__D5946643BE029266").IsUnique();

            entity.Property(e => e.Clasificacion)
                .HasMaxLength(20)
                .HasDefaultValue("CUMPLIDO");
            entity.Property(e => e.DeudaTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FechaActualizacion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdClienteNavigation).WithOne(p => p.EstadoCuenta)
                .HasForeignKey<EstadoCuenta>(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EstadoCue__IdCli__06CD04F7");
        });

        modelBuilder.Entity<PlanPago>(entity =>
        {
            entity.HasKey(e => e.IdPlanPago).HasName("PK__PlanPago__DECF3B6D9E5AE4A1");

            entity.ToTable("PlanPago");

            entity.HasIndex(e => new { e.IdCompra, e.Estado }, "IX_PlanPago_Cliente_Estado");

            entity.HasIndex(e => e.IdCompra, "IX_PlanPago_Compra");

            entity.HasIndex(e => e.Estado, "IX_PlanPago_Estado");

            entity.HasIndex(e => e.FechaVencimiento, "IX_PlanPago_Vencimiento");

            entity.HasIndex(e => new { e.IdCompra, e.NumeroPago }, "UQ__PlanPago__AD93A9EF72A46DFD").IsUnique();

            entity.Property(e => e.DiasVencidos).HasComputedColumnSql("(case when [Estado]='VENCIDO' then datediff(day,[FechaVencimiento],getdate()) else (0) end)", false);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("PENDIENTE");
            entity.Property(e => e.MontoPagado).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MontoProgramado).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SaldoPendiente).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.PlanPagos)
                .HasForeignKey(d => d.IdCompra)
                .HasConstraintName("FK__PlanPago__IdComp__72C60C4A");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__0988921067FA3C6B");

            entity.ToTable("Producto");

            entity.HasIndex(e => e.Codigo, "IX_Producto_Codigo");

            entity.HasIndex(e => e.IdProveedor, "IX_Producto_Proveedor");

            entity.HasIndex(e => e.Codigo, "UQ__Producto__06370DAC1F72AC3F").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Producto__IdProv__628FA481");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__E8B631AF9C84BBA1");

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

            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        return services;
    }
}