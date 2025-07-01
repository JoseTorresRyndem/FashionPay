using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;

namespace FashionPay.Infrastructure.Repositories;

public class CompraRepository : BaseRepository<Compra>, ICompraRepository
{
    public CompraRepository(FashionPayContext context) : base(context)
    {
    }
    // Método para obtener todas las compras con relaciones
    public async Task<IEnumerable<Compra>> GetAllWithRelationsAsync()
    {
        return await _context.Compras
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl.EstadoCuenta)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
            .Include(c => c.PlanPagos)
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }
    // Método para obtener compra por ID con relaciones
    public async Task<Compra?> GetByIdWithRelationsAsync(int id)
    {
        return await _context.Compras
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl.EstadoCuenta)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
            .Include(c => c.PlanPagos)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    // Método para obtener compra por ID cliente con relaciones
    public async Task<IEnumerable<Compra>> GetByClienteWithRelationsAsync(int clienteId)
    {
        return await _context.Compras
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl.EstadoCuenta)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
            .Include(c => c.PlanPagos)
            .Where(c => c.ClienteId == clienteId)
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }
    public async Task<IEnumerable<Compra>> GetComprasWithFiltrosAsync(
        int? clienteId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        decimal? montoMinimo = null,
        decimal? montoMaximo = null
        )
    {
        var query = _context.Compras
            .Include(c => c.Cliente)
                .ThenInclude(cl => cl.EstadoCuenta)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
            .Include(c => c.PlanPagos)
            .AsQueryable();

        // Aplicar filtros
        if (clienteId.HasValue)
            query = query.Where(c => c.ClienteId == clienteId.Value);

        if (fechaDesde.HasValue)
            query = query.Where(c => c.FechaCompra >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(c => c.FechaCompra <= fechaHasta.Value);

        if (montoMinimo.HasValue)
            query = query.Where(c => c.MontoTotal >= montoMinimo.Value);

        if (montoMaximo.HasValue)
            query = query.Where(c => c.MontoTotal <= montoMaximo.Value);

        return await query
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }
    public async Task<Compra> CrearCompraAsync(
        int clienteId,
        int cantidadPagos,
        string? observaciones,
        List<(int ProductoId, int Cantidad, decimal PrecioUnitario)> detalles)
    {
        // Usar la estrategia de ejecución de EF Core para manejar transacciones
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Validar que el cliente existe y puede comprar
                var cliente = await _context.Clientes
                .Include(c => c.EstadoCuenta)
                .FirstOrDefaultAsync(c => c.Id == clienteId);

                if (cliente == null || !cliente.Activo)
                    throw new InvalidOperationException("Cliente no válido para realizar compras");

                if (cliente.EstadoCuenta?.Clasificacion == "MOROSO")
                    throw new InvalidOperationException("Cliente moroso no puede realizar compras");

                // 2. Validar productos y calcular totales
                var detallesValidados = new List<DetalleCompra>();
                decimal montoTotal = 0;

                foreach (var detalle in detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null || !producto.Activo)
                        throw new InvalidOperationException($"Producto {detalle.ProductoId} no válido");

                    if (producto.Precio != detalle.PrecioUnitario)
                        throw new InvalidOperationException($"Precio del producto {producto.Codigo} ha cambiado");

                    var subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                    montoTotal += subtotal;

                    detallesValidados.Add(new DetalleCompra
                    {
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Subtotal = subtotal
                    });
                }

                // 3. Validar límite de crédito
                var deudaActual = cliente.EstadoCuenta?.DeudaTotal ?? 0;
                if (deudaActual + montoTotal > cliente.LimiteCredito)
                    throw new InvalidOperationException("Monto excede el límite de crédito disponible");

                // 4. Validar cantidad de pagos
                if (cantidadPagos > cliente.CantidadMaximaPagos)
                    throw new InvalidOperationException($"Máximo {cliente.CantidadMaximaPagos} pagos permitidos");

                // 5. Crear la compra
                var compra = new Compra
                {
                    ClienteId = clienteId,
                    NumeroCompra = GenerarNumeroCompra(),
                    FechaCompra = DateTime.Now,
                    MontoTotal = montoTotal,
                    CantidadPagos = cantidadPagos,
                    MontoMensual = Math.Round(montoTotal / cantidadPagos, 2),
                    Observaciones = observaciones,
                    DetalleCompras = detallesValidados
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                // 6. Crear plan de pagos con montos exactos
                var planPagos = CalcularPlanPagos(
                    compra.Id,
                    montoTotal,
                    cantidadPagos,
                    cliente.DiaPago);

                _context.PlanPagos.AddRange(planPagos);
                await _context.SaveChangesAsync();

                // 7. Actualizar estado de cuenta del cliente
                await ActualizarEstadoCuentaClienteAsync(clienteId);

                await transaction.CommitAsync();

                // 8. Retornar compra completa con navegación
                return await GetByIdWithRelationsAsync(compra.Id)
                    ?? throw new InvalidOperationException("Error al recuperar compra creada");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }



    //Métodos privados para calcular los planes de pago y montos
    private List<PlanPago> CalcularPlanPagos(int compraId, decimal montoTotal, int cantidadPagos, int diaPagoCliente)
    {
        // Calcular montos exactos (evitar errores de redondeo)
        var montosPago = CalcularMontosExactos(montoTotal, cantidadPagos);
        var planPagos = new List<PlanPago>();
        var fechaBase = DateTime.Today;

        for (int i = 0; i < cantidadPagos; i++)
        {
            var fechaVencimiento = CalcularFechaVencimiento(fechaBase, i + 1, diaPagoCliente);

            planPagos.Add(new PlanPago
            {
                CompraId = compraId,
                NumeroPago = i + 1,
                FechaVencimiento = fechaVencimiento,
                MontoProgramado = montosPago[i],
                MontoPagado = 0,
                SaldoPendiente = montosPago[i],
                Estado = "PENDIENTE"
            });
        }

        return planPagos;
    }
    private List<decimal> CalcularMontosExactos(decimal montoTotal, int cantidadPagos)
    {
        // Monto base por pago (redondeado hacia abajo a centavos)
        var montoBase = Math.Floor(montoTotal / cantidadPagos * 100) / 100;

        // Calcular diferencia total
        var totalMontoBase = montoBase * cantidadPagos;
        var diferencia = montoTotal - totalMontoBase;

        // Convertir diferencia a centavos para distribución
        var centavosDiferencia = (int)Math.Round(diferencia * 100);

        var pagos = new List<decimal>();

        for (int i = 0; i < cantidadPagos; i++)
        {
            var montoPago = montoBase;

            // Distribuir centavos extra en los primeros pagos
            if (i < centavosDiferencia)
            {
                montoPago += 0.01m;
            }

            pagos.Add(montoPago);
        }

        // Validación: debe sumar exactamente el monto total
        var sumaTotal = pagos.Sum();
        if (Math.Abs(sumaTotal - montoTotal) > 0.01m)
        {
            throw new InvalidOperationException(
                $"Error en cálculo de pagos: suma {sumaTotal} != total {montoTotal}");
        }

        return pagos;
    }
    private DateOnly CalcularFechaVencimiento(DateTime fechaBase, int numeroPago, int diaPago)
    {
        var fechaVencimiento = fechaBase.AddMonths(numeroPago);

        // Ajustar al día de pago del cliente
        var ultimoDiaDelMes = DateTime.DaysInMonth(fechaVencimiento.Year, fechaVencimiento.Month);
        var diaEfectivo = Math.Min(diaPago, ultimoDiaDelMes);

        var fechaCalculada = new DateTime(fechaVencimiento.Year, fechaVencimiento.Month, diaEfectivo);
        return DateOnly.FromDateTime(fechaCalculada);
    }
    private string GenerarNumeroCompra()
    {
        return $"CMP-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..4].ToUpper()}";
    }
    private async Task ActualizarEstadoCuentaClienteAsync(int clienteId)
    {
        // Ejecutar procedimiento almacenado para recalcular estado
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_CalcularSaldoCliente @p0", clienteId);
    }
}