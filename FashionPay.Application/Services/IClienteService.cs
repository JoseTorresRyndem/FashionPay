using FashionPay.Application.DTOs.Cliente;

namespace FashionPay.Application.Services;

public interface IClienteService
{
    Task<ClienteResponseDto> CreateClientAsync(ClienteCreateDto clientDto);
    Task<ClienteResponseDto?> GetClientByIdAsync(int id);
    Task<ClienteResponseDto?> GetClientByEmailAsync(string email);
    Task<IEnumerable<ClienteResponseDto>> GetClientsAsync();
    Task<IEnumerable<ClienteResponseDto>> GetClientsByClassificationAsync(string classification);
    Task<ClienteResponseDto> UpdateClientAsync(int id, ClienteUpdateDto clientDto);
    Task<bool> DeleteClientAsync(int id);
    Task<bool> RecalculateBalanceAsync(int id);
}