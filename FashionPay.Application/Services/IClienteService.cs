using FashionPay.Application.DTOs.Cliente;

namespace FashionPay.Application.Services;

public interface IClienteService
{
    Task<ClienteResponseDto> CrearClienteAsync(ClienteCreateDto clienteDto);
    Task<ClienteResponseDto?> GetClienteByIdAsync(int id);
    Task<ClienteResponseDto?> GetClienteByEmailAsync(string email);
    Task<IEnumerable<ClienteResponseDto>> GetClientesAsync();
    Task<IEnumerable<ClienteResponseDto>> GetClientesByClasificacionAsync(string clasificacion);
    Task<ClienteResponseDto> ActualizarClienteAsync(int id, ClienteUpdateDto clienteDto);
    Task<bool> EliminarClienteAsync(int id);
    Task<bool> RecalcularSaldoAsync(int id);
}