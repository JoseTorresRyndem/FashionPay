using AutoMapper;
using FashionPay.Core.Interfaces;
using FashionPay.Application.DTOs.Cliente;
using FashionPay.Application.Exceptions;

namespace FashionPay.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClienteService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ClienteResponseDto?> GetClientByIdAsync(int id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        return cliente != null ? _mapper.Map<ClienteResponseDto>(cliente) : null;
    }
    public async Task<ClienteResponseDto?> GetClientByEmailAsync(string email)
    {
        var cliente = await _unitOfWork.Clientes.GetByEmailAsync(email);
        return cliente != null ? _mapper.Map<ClienteResponseDto>(cliente) : null;
    }
    public async Task<IEnumerable<ClienteResponseDto>> GetClientsAsync()
    {
        var clientes = await _unitOfWork.Clientes.GetAllAsync();
        return _mapper.Map<IEnumerable<ClienteResponseDto>>(clientes);
    }
    public async Task<IEnumerable<ClienteResponseDto>> GetClientsByClassificationAsync(string clasificacion)
    {
        // Validar clasificación
        var clasificacionUpper = clasificacion.ToUpper();
        if (!new[] { "CUMPLIDO", "RIESGOSO", "MOROSO" }.Contains(clasificacionUpper))
            throw new BusinessException("Clasificación debe ser: CUMPLIDO, RIESGOSO o MOROSO");

        var clientes = await _unitOfWork.Clientes.GetClientsByClassificationAsync(clasificacionUpper);
        return _mapper.Map<IEnumerable<ClienteResponseDto>>(clientes);
    }
    public async Task<ClienteResponseDto> CreateClientAsync(ClienteCreateDto clienteDto)
    {
        // Validación de negocio: Email único
        await ValidateUniqueEmailAsync(clienteDto.Email);

        // Usar procedimiento almacenado sp_AltaCliente
        var clienteCreado = await _unitOfWork.Clientes.CreateClientWithAccountStatusAsync(
            clienteDto.Nombre,
            clienteDto.Email,
            clienteDto.Telefono,
            clienteDto.Direccion,
            clienteDto.DiaPago,
            clienteDto.LimiteCredito,
            clienteDto.CantidadMaximaPagos,
            clienteDto.ToleranciasMorosidad
        );

        return _mapper.Map<ClienteResponseDto>(clienteCreado);
    }
    public async Task<ClienteResponseDto> UpdateClientAsync(int id, ClienteUpdateDto clienteDto)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null)
            throw new NotFoundException($"Cliente con ID {id} no encontrado");

        // Mapear cambios y actualizar
        _mapper.Map(clienteDto, cliente);
        await _unitOfWork.Clientes.UpdateAsync(cliente);

        // Retornar cliente actualizado
        var clienteActualizado = await _unitOfWork.Clientes.GetByIdAsync(id);
        return _mapper.Map<ClienteResponseDto>(clienteActualizado!);
    }
    public async Task<bool> DeleteClientAsync(int id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null)
            throw new NotFoundException($"Cliente con ID {id} no encontrado");

        // Validar que no tenga deuda pendiente
        var deudaTotal = await _unitOfWork.Clientes.GetTotalDebtAsync(id);
        if (deudaTotal > 0)
            throw new BusinessException($"No se puede eliminar cliente con deuda pendiente: ${deudaTotal:F2}");

        // Soft delete
        cliente.Activo = false;
        await _unitOfWork.Clientes.UpdateAsync(cliente);

        return true;
    }
    public async Task<bool> RecalculateBalanceAsync(int id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null)
            throw new NotFoundException($"Cliente con ID {id} no encontrado");

        await _unitOfWork.Clientes.ExecuteCalculateBalanceAsync(id);
        return true;
    }
    private async Task ValidateUniqueEmailAsync(string email)
    {
        var clienteExistente = await _unitOfWork.Clientes.GetByEmailAsync(email);
        if (clienteExistente != null)
            throw new BusinessException($"Ya existe un cliente con el email '{email}'");
    }
}

