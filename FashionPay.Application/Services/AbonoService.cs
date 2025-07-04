using AutoMapper;
using FashionPay.Core.Interfaces;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Abono;
using FashionPay.Application.Exceptions;

namespace FashionPay.Application.Services;

public class AbonoService : IAbonoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AbonoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AbonoResponseDto> RegisterPaymentAsync(AbonoCreateDto abonoDto)
    {
        // 1. Validaciones de negocio
        await ValidatePaymentAsync(abonoDto);

        // 2. Buscar el pago más antiguo pendiente del cliente (lógica de negocio)
        var pagoPendiente = await FindOldestPendingPaymentAsync(abonoDto.IdCliente);
        if (pagoPendiente == null)
            throw new BusinessException("El cliente no tiene pagos pendientes");
        try
        {
            var abono = await _unitOfWork.Abonos.ApplyFullPaymentAsync(
                abonoDto.IdCliente,
                abonoDto.MontoAbono,
                abonoDto.FormaPago,
                abonoDto.Observaciones,
                pagoPendiente);

            // 4. Retornar respuesta completa (responsabilidad del Service)
            var abonoCompleto = await _unitOfWork.Abonos.GetByIdAsync(abono.IdAbono);
            return _mapper.Map<AbonoResponseDto>(abonoCompleto!);
        }
        catch (Exception ex)
        {
            throw new BusinessException("Error al registrar pago pendiente: " + ex.Message);
        }
    }


    public async Task<AbonoResponseDto?> GetPaymentByIdAsync(int id)
    {
        var abono = await _unitOfWork.Abonos.GetByIdAsync(id);
        return abono != null ? _mapper.Map<AbonoResponseDto>(abono) : null;
    }

    public async Task<IEnumerable<AbonoResponseDto>> GetPaymentsByClientAsync(int clienteId)
    {
        // Validar que el cliente existe
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);
        if (cliente == null)
            throw new NotFoundException($"Cliente con ID {clienteId} no encontrado");

        var abonos = await _unitOfWork.Abonos.GetPaymentsByClientAsync(clienteId);
        return _mapper.Map<IEnumerable<AbonoResponseDto>>(abonos);
    }

    public async Task<IEnumerable<AbonoResponseDto>> GetPaymentsWithFiltersAsync(AbonoFiltrosDto filtros)
    {
        var abonos = await _unitOfWork.Abonos.GetPaymentsWithFullRelationsAsync(filtros.IdCliente);

        // Aplicar filtros restantes en memoria
        var abonosFiltrados = abonos.Where(a =>
            (!filtros.FechaDesde.HasValue || a.FechaAbono >= filtros.FechaDesde.Value) &&
            (!filtros.FechaHasta.HasValue || a.FechaAbono <= filtros.FechaHasta.Value) &&
            (string.IsNullOrEmpty(filtros.FormaPago) || a.FormaPago.ToUpper() == filtros.FormaPago.ToUpper()) &&
            (!filtros.MontoMinimo.HasValue || a.MontoAbono >= filtros.MontoMinimo.Value) &&
            (!filtros.MontoMaximo.HasValue || a.MontoAbono <= filtros.MontoMaximo.Value)
        );

        return _mapper.Map<IEnumerable<AbonoResponseDto>>(abonosFiltrados);
    }

    public async Task<ResumenPagosClienteDto> GetClientPaymentSummaryAsync(int clienteId)
    {
        // Validar que el cliente existe
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);
        if (cliente == null)
            throw new NotFoundException($"Cliente con ID {clienteId} no encontrado");

        // Obtener abonos del cliente
        var abonos = await _unitOfWork.Abonos.GetPaymentsByClientAsync(clienteId);

        // Obtener pagos pendientes del cliente
        var pagosPendientes = await _unitOfWork.PlanPagos.GetPaymentsByClientAsync(clienteId);

        // Obtener estado de cuenta
        var estadoCuenta = await _unitOfWork.Clientes.GetAccountStatusAsync(clienteId);

        return new ResumenPagosClienteDto
        {
            IdCliente = clienteId,
            NombreCliente = cliente.Nombre,
            TotalAbonos = abonos.Sum(a => a.MontoAbono),
            CantidadAbonos = abonos.Count(),
            UltimoAbono = abonos.OrderByDescending(a => a.FechaAbono).FirstOrDefault()?.FechaAbono,
            DeudaActual = estadoCuenta?.DeudaTotal ?? 0,
            PagosPendientes = pagosPendientes.Count(p => p.Estado == "PENDIENTE"),
            PagosVencidos = pagosPendientes.Count(p => p.Estado == "VENCIDO")
        };
    }

    #region Métodos privados (Solo lógica de negocio)

    private async Task ValidatePaymentAsync(AbonoCreateDto abonoDto)
    {
        // Validar que el cliente existe y está activo
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(abonoDto.IdCliente);
        if (cliente == null || !cliente.Activo)
            throw new BusinessException("El cliente seleccionado no existe o está inactivo");

        // Validar que el cliente tiene deuda pendiente
        var deudaTotal = await _unitOfWork.Clientes.GetTotalDebtAsync(abonoDto.IdCliente);
        if (deudaTotal <= 0)
            throw new BusinessException("El cliente no tiene deuda pendiente");

        // Validar que el monto del abono no exceda la deuda total
        if (abonoDto.MontoAbono > deudaTotal)
            throw new BusinessException($"El monto del abono (${abonoDto.MontoAbono:F2}) excede la deuda total (${deudaTotal:F2})");

        // Validar forma de pago
        var formasPagoValidas = new[] { "EFECTIVO", "TRANSFERENCIA", "TARJETA" };
        if (!formasPagoValidas.Contains(abonoDto.FormaPago.ToUpper()))
            throw new BusinessException("Forma de pago no válida");
    }

    private async Task<PlanPago?> FindOldestPendingPaymentAsync(int clienteId)
    {
        var pagosPendientes = await _unitOfWork.PlanPagos.GetPaymentsByClientAsync(clienteId);

        return pagosPendientes
            .Where(p => p.SaldoPendiente > 0)
            .OrderBy(p => p.Estado == "VENCIDO" ? 0 : 1) // Vencidos primero
            .ThenBy(p => p.FechaVencimiento) // Más antiguos primero
            .FirstOrDefault();
    }
    #endregion

}