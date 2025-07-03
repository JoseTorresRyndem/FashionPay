using AutoMapper;
using FashionPay.Core.Interfaces;
using FashionPay.Application.DTOs.Compra;
using FashionPay.Application.Exceptions;

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

    public async Task<CompraResponseDto> CrearCompraAsync(CompraCreateDto compraDto)
    {
        // 1. Validaciones de negocio exhaustivas
        await ValidarCompraAsync(compraDto);

        // 2. Convertir DTO a parámetros primitivos para el repositorio
        var detalles = compraDto.Detalles.Select(d =>
            (d.IdProducto, d.Cantidad, d.PrecioUnitario)).ToList();

        var compra = await _unitOfWork.Compras.CrearCompraAsync(
            compraDto.IdCliente,
            compraDto.CantidadPagos,
            compraDto.Observaciones,
            detalles);

        return _mapper.Map<CompraResponseDto>(compra);
    }
    public async Task<CompraResponseDto?> GetCompraByIdAsync(int id)
    {
        var compra = await _unitOfWork.Compras.GetByIdWithRelationsAsync(id);
        return compra != null ? _mapper.Map<CompraResponseDto>(compra) : null;
    }

    public async Task<IEnumerable<CompraResponseDto>> GetComprasAsync()
    {
        var compras = await _unitOfWork.Compras.GetAllWithRelationsAsync();
        return _mapper.Map<IEnumerable<CompraResponseDto>>(compras);
    }

    public async Task<IEnumerable<CompraResponseDto>> GetComprasByClienteAsync(int clienteId)
    {
        // Validar que el cliente existe
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);
        if (cliente == null)
            throw new NotFoundException($"Cliente con ID {clienteId} no encontrado");

        var compras = await _unitOfWork.Compras.GetByClienteWithRelationsAsync(clienteId);
        return _mapper.Map<IEnumerable<CompraResponseDto>>(compras);
    }

    public async Task<IEnumerable<CompraResponseDto>> GetComprasConFiltrosAsync(CompraFiltrosDto filtros)
    {
        var compras = await _unitOfWork.Compras.GetComprasWithFiltrosAsync(
            filtros.IdCliente,
            filtros.FechaDesde,
            filtros.FechaHasta,
            filtros.MontoMinimo,
            filtros.MontoMaximo
            );

        return _mapper.Map<IEnumerable<CompraResponseDto>>(compras);
    }
    private async Task ValidarCompraAsync(CompraCreateDto compraDto)
    {
        // 1. Validar que el cliente existe y está activo
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(compraDto.IdCliente);
        if (cliente == null || !cliente.Activo)
            throw new BusinessException("El cliente seleccionado no existe o está inactivo");

        // 2. Validar que el cliente no esté moroso
        var estadoCuenta = await _unitOfWork.Clientes.GetEstadoCuentaAsync(compraDto.IdCliente);
        if (estadoCuenta?.Clasificacion == "MOROSO")
            throw new BusinessException("El cliente está clasificado como moroso y no puede realizar compras");

        // 3. Validar que el cliente no tenga pagos vencidos
        if (estadoCuenta?.CantidadPagosVencidos > 0)
            throw new BusinessException("El cliente tiene pagos vencidos pendientes y no puede realizar nuevas compras");

        // 4. Validar cantidad de pagos para este cliente
        if (compraDto.CantidadPagos > cliente.CantidadMaximaPagos)
            throw new BusinessException($"La cantidad de pagos ({compraDto.CantidadPagos}) excede el máximo permitido para este cliente ({cliente.CantidadMaximaPagos})");

        // 5. Validar productos y calcular monto total
        decimal montoTotal = 0;
        var productosValidados = new List<string>();

        foreach (var detalle in compraDto.Detalles)
        {
            // Validar que el producto existe y está activo
            var producto = await _unitOfWork.Productos.GetByIdAsync(detalle.IdProducto);
            if (producto == null || !producto.Activo)
                throw new BusinessException($"El producto con ID {detalle.IdProducto} no existe o está inactivo");

            // Validar que el precio coincide con el precio actual
            if (producto.Precio != detalle.PrecioUnitario)
                throw new BusinessException($"El precio del producto '{producto.Nombre}' ha cambiado. Precio actual: ${producto.Precio:F2}");

            // Validar stock suficiente (opcional)
            if (producto.Stock < detalle.Cantidad)
                throw new BusinessException($"Stock insuficiente para el producto '{producto.Nombre}'. Stock disponible: {producto.Stock}");

            productosValidados.Add(producto.Nombre);
            montoTotal += detalle.Cantidad * detalle.PrecioUnitario;
        }

        // 6. Validar límite de crédito
        var deudaActual = await _unitOfWork.Clientes.GetDeudaTotalAsync(compraDto.IdCliente);
        var creditoDisponible = cliente.LimiteCredito - deudaActual;

        if (montoTotal > creditoDisponible)
            throw new BusinessException($"El monto de la compra (${montoTotal:F2}) excede el límite de crédito disponible (${creditoDisponible:F2})");

        // 7. Validar monto mínimo de compra (opcional)
        if (montoTotal < 100)
            throw new BusinessException("El monto mínimo de compra es $100.00");

        Console.WriteLine($"Compra validada: Cliente {cliente.Nombre}, Productos: {string.Join(", ", productosValidados)}, Monto Total: ${montoTotal:F2}");
    }
}