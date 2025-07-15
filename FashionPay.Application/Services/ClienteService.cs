using AutoMapper;
using FashionPay.Core.Interfaces;
using FashionPay.Application.DTOs.Cliente;
using FashionPay.Core.Entities;

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
        var clasificacionUpper = clasificacion.ToUpper();
        if (!new[] { "CUMPLIDO", "RIESGOSO", "MOROSO" }.Contains(clasificacionUpper))
            throw new ArgumentException("Clasificación debe ser: CUMPLIDO, RIESGOSO o MOROSO");

        var clientes = await _unitOfWork.Clientes.GetClientsByClassificationAsync(clasificacionUpper);
        return _mapper.Map<IEnumerable<ClienteResponseDto>>(clientes);
    }
    
    public async Task<EstadoCuentaDto?> GetClientAccountStatusAsync(int clienteId)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);
        if (cliente == null)
            throw new KeyNotFoundException($"Cliente con ID {clienteId} no encontrado");

        var estadoCuenta = await _unitOfWork.EstadoCuentas.FirstOrDefaultAsync(ec => ec.IdCliente == clienteId);
        return estadoCuenta != null ? _mapper.Map<EstadoCuentaDto>(estadoCuenta) : null;
    }
    public async Task<ClienteResponseDto> CreateClientAsync(ClienteCreateDto clienteDto)
    {
        await ValidateUniqueEmailAsync(clienteDto.Email);

        var cliente = _mapper.Map<Cliente>(clienteDto);

        cliente.CreditoDisponible = clienteDto.LimiteCredito;
        var result = await _unitOfWork.Clientes.AddAsync(cliente);
        var estadoCuenta = new EstadoCuenta
        {
            IdCliente = result.IdCliente,
            Clasificacion = "CUMPLIDO",
            DeudaTotal = 0
        }; 
        await _unitOfWork.EstadoCuentas.AddAsync(estadoCuenta);

        await _unitOfWork.SaveChangesAsync();

        var clienteCreate = await _unitOfWork.Clientes.GetByIdAsync(result.IdCliente);

        return _mapper.Map<ClienteResponseDto>(clienteCreate);
    }
    public async Task<ClienteResponseDto> UpdateClientAsync(int id, ClienteUpdateDto clienteDto)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null)
            throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");

        // Mapear cambios y actualizar
        _mapper.Map(clienteDto, cliente);
        await _unitOfWork.Clientes.UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        // Retornar cliente actualizado
        var clienteActualizado = await _unitOfWork.Clientes.GetByIdAsync(id);
        return _mapper.Map<ClienteResponseDto>(clienteActualizado!);
    }
    public async Task<bool> DeleteClientAsync(int id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null)
            throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");

        // Validar que no tenga deuda pendiente
        var deudaTotal = await _unitOfWork.Clientes.GetTotalDebtAsync(id);
        if (deudaTotal > 0)
            throw new InvalidOperationException($"No se puede eliminar cliente con deuda pendiente: ${deudaTotal:F2}");

        // Soft delete
        cliente.Activo = false;
        await _unitOfWork.Clientes.UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
    public async Task<bool> RecalculateBalanceAsync(int id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        if (cliente == null)
            throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");

        await _unitOfWork.Clientes.ExecuteCalculateBalanceAsync(id);
        return true;
    }
    private async Task ValidateUniqueEmailAsync(string email)
    {
        var clienteExistente = await _unitOfWork.Clientes.GetByEmailAsync(email);
        if (clienteExistente != null)
            throw new ArgumentException($"Ya existe un cliente con el email '{email}'");
    }
}

