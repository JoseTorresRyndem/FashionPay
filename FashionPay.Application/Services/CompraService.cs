using AutoMapper;
using FashionPay.Application.DTOs.Compra;
using FashionPay.Application.Common;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FashionPay.Application.Services;

public class CompraService : ICompraService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompraService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CompraResponseDto> CreatePurchaseAsync(CompraCreateDto compraDto)
    {
        await ValidatePurchaseAsync(compraDto);

        var detalles = compraDto.Detalles.Select(d =>
            (d.IdProducto, d.Cantidad, d.PrecioUnitario)).ToList();

        var strategy = _unitOfWork.Context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();
            try
            {
                var cliente = await _unitOfWork.Clientes.GetByIdWithAccountAsync(compraDto.IdCliente);
                if (cliente == null || !cliente.Activo)
                    throw new InvalidOperationException("Cliente no válido para realizar compras");

                if (cliente.EstadoCuenta?.Clasificacion == "MOROSO")
                    throw new InvalidOperationException("Cliente moroso no puede realizar compras");

                var (detallesValidados, montoTotal) = await ProcessPurchaseDetailsAsync(detalles);

                ValidateFinancialLimits(cliente, montoTotal, compraDto.CantidadPagos);

                var compra = new Compra
                {
                    IdCliente = compraDto.IdCliente,
                    NumeroCompra = GeneratePurchaseNumber(),
                    FechaCompra = DateTime.Now,
                    MontoTotal = montoTotal,
                    CantidadPagos = compraDto.CantidadPagos,
                    MontoMensual = Math.Round(montoTotal / compraDto.CantidadPagos, 2),
                    Observaciones = compraDto.Observaciones,
                    EstadoCompra = "ACTIVA",
                    DetalleCompras = detallesValidados
                };
                var planPagos = CalculatePaymentPlanWithoutId(montoTotal, compraDto.CantidadPagos, cliente.DiaPago);

                compra.PlanPagos = planPagos;

                await _unitOfWork.Compras.AddAsync(compra);

                cliente.CreditoDisponible -= montoTotal;
                await _unitOfWork.Clientes.UpdateAsync(cliente);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.Context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_CalcularSaldoCliente @p0", compraDto.IdCliente);

                await transaction.CommitAsync();

                var compraF = await _unitOfWork.Compras.GetByIdWithRelationsAsync(compra.IdCompra)
                   ?? throw new InvalidOperationException("Error al recuperar compra creada");

                return _mapper.Map<CompraResponseDto>(compraF);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
    public async Task<CompraResponseDto?> GetPurchaseByIdAsync(int id)
    {
        var compra = await _unitOfWork.Compras.GetByIdWithRelationsAsync(id);
        return compra != null ? _mapper.Map<CompraResponseDto>(compra) : null;
    }

    public async Task<IEnumerable<CompraResponseDto>> GetPurchasesAsync()
    {
        var compras = await _unitOfWork.Compras.GetAllWithRelationsAsync();
        return _mapper.Map<IEnumerable<CompraResponseDto>>(compras);
    }

    public async Task<IEnumerable<CompraResponseDto>> GetPurchasesByClientAsync(int clienteId)
    {
        // Validar que el cliente existe
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);
        if (cliente == null)
            throw new KeyNotFoundException($"Cliente con ID {clienteId} no encontrado");

        var compras = await _unitOfWork.Compras.GetByClientWithRelationsAsync(clienteId);
        return _mapper.Map<IEnumerable<CompraResponseDto>>(compras);
    }

    public async Task<IEnumerable<CompraResponseDto>> GetPurchasesWithFiltersAsync(CompraFiltrosDto filtros)
    {
        var compras = await _unitOfWork.Compras.GetPurchasesWithFiltersAsync(
            filtros.IdCliente,
            filtros.FechaDesde,
            filtros.FechaHasta,
            filtros.MontoMinimo,
            filtros.MontoMaximo
            );

        return _mapper.Map<IEnumerable<CompraResponseDto>>(compras);
    }
    //METODO PRIVATE
    private async Task<(List<DetalleCompra> detalles, decimal montoTotal)> ProcessPurchaseDetailsAsync(
    List<(int IdProducto, int Cantidad, decimal PrecioUnitario)> detalles)
    {
        var detallesValidados = new List<DetalleCompra>();
        decimal montoTotal = 0;

        foreach (var (idProducto, cantidad, precioUnitario) in detalles)
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(idProducto);
            if (producto == null || !producto.Activo)
                throw new InvalidOperationException($"Producto {idProducto} no válido");

            if (producto.Precio != precioUnitario)
                throw new InvalidOperationException($"Precio del producto {producto.Codigo} ha cambiado");

            var subtotal = cantidad * precioUnitario;
            montoTotal += subtotal;

            detallesValidados.Add(new DetalleCompra
            {
                IdProducto = idProducto,
                Cantidad = cantidad,
                PrecioUnitario = precioUnitario,
                Subtotal = subtotal
            });
        }

        return (detallesValidados, montoTotal);
    }
    private static void ValidateFinancialLimits(Cliente cliente, decimal montoTotal, int cantidadPagos)
    {
        if (cliente.CreditoDisponible < montoTotal)
        {
            throw new InvalidOperationException(
                $"Crédito insuficiente. Disponible: ${cliente.CreditoDisponible:F2}, " +
                $"Requerido: ${montoTotal:F2}");
        }

        var deudaActual = cliente.EstadoCuenta?.DeudaTotal ?? 0;
        if (deudaActual + montoTotal > cliente.LimiteCredito)
            throw new InvalidOperationException("Monto excede el límite de crédito disponible");

        if (cantidadPagos > cliente.CantidadMaximaPagos)
            throw new InvalidOperationException($"Máximo {cliente.CantidadMaximaPagos} pagos permitidos");
    }

    private async Task ValidatePurchaseAsync(CompraCreateDto compraDto)
    {
        // 1. Validar que el cliente existe y está activo
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(compraDto.IdCliente);
        if (cliente == null || !cliente.Activo)
            throw new ArgumentException("El cliente seleccionado no existe o está inactivo");

        // 2. Validar que el cliente no esté moroso
        var estadoCuenta = await _unitOfWork.Clientes.GetAccountStatusAsync(compraDto.IdCliente);
        if (estadoCuenta?.Clasificacion == "MOROSO")
            throw new InvalidOperationException("El cliente está clasificado como moroso y no puede realizar compras");

        // 3. Validar que el cliente no tenga pagos vencidos
        if (estadoCuenta?.CantidadPagosVencidos > 0)
            throw new InvalidOperationException("El cliente tiene pagos vencidos pendientes y no puede realizar nuevas compras");

        // 4. Validar cantidad de pagos para este cliente
        if (compraDto.CantidadPagos > cliente.CantidadMaximaPagos)
            throw new ArgumentException($"La cantidad de pagos ({compraDto.CantidadPagos}) excede el máximo permitido para este cliente ({cliente.CantidadMaximaPagos})");

        // 5. Validar productos y calcular monto total
        decimal montoTotal = 0;
        var productosValidados = new List<string>();

        foreach (var detalle in compraDto.Detalles)
        {
            // Validar que el producto existe y está activo
            var producto = await _unitOfWork.Productos.GetByIdAsync(detalle.IdProducto);
            if (producto == null || !producto.Activo)
                throw new ArgumentException($"El producto con ID {detalle.IdProducto} no existe o está inactivo");

            // Validar que el precio coincide con el precio actual
            if (producto.Precio != detalle.PrecioUnitario)
                throw new ArgumentException($"El precio del producto '{producto.Nombre}' ha cambiado. Precio actual: ${producto.Precio:F2}");

            // Validar stock suficiente (opcional)
            if (producto.Stock < detalle.Cantidad)
                throw new InvalidOperationException($"Stock insuficiente para el producto '{producto.Nombre}'. Stock disponible: {producto.Stock}");

            productosValidados.Add(producto.Nombre);
            montoTotal += detalle.Cantidad * detalle.PrecioUnitario;
        }

        // 6. Validar límite de crédito
        var deudaActual = await _unitOfWork.Clientes.GetTotalDebtAsync(compraDto.IdCliente);
        var creditoDisponible = cliente.LimiteCredito - deudaActual;

        if (montoTotal > creditoDisponible)
            throw new ArgumentException($"El monto de la compra (${montoTotal:F2}) excede el límite de crédito disponible (${creditoDisponible:F2})");

        // 7. Validar monto mínimo de compra
        if (montoTotal < BusinessConstants.Purchase.MIN_PURCHASE_AMOUNT)
            throw new ArgumentException($"El monto mínimo de compra es ${BusinessConstants.Purchase.MIN_PURCHASE_AMOUNT:F2}");

    }
    private List<PlanPago> CalculatePaymentPlanWithoutId(decimal montoTotal, int cantidadPagos, int diaPagoCliente)
    {
        var montosPago = CalculateExactAmounts(montoTotal, cantidadPagos);
        var planPagos = new List<PlanPago>();
        var fechaBase = DateTime.Today;

        for (int i = 0; i < cantidadPagos; i++)
        {
            var fechaVencimiento = CalculateDueDate(fechaBase, i + 1, diaPagoCliente);

            planPagos.Add(new PlanPago
            {

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
    private List<decimal> CalculateExactAmounts(decimal montoTotal, int cantidadPagos)
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
    private DateOnly CalculateDueDate(DateTime fechaBase, int numeroPago, int diaPago)
    {
        var fechaVencimiento = fechaBase.AddMonths(numeroPago);

        // Ajustar al día de pago del cliente
        var ultimoDiaDelMes = DateTime.DaysInMonth(fechaVencimiento.Year, fechaVencimiento.Month);
        var diaEfectivo = Math.Min(diaPago, ultimoDiaDelMes);

        var fechaCalculada = new DateTime(fechaVencimiento.Year, fechaVencimiento.Month, diaEfectivo);
        return DateOnly.FromDateTime(fechaCalculada);
    }
    private string GeneratePurchaseNumber()
    {
        return $"CMP-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..4].ToUpper()}";
    }

}