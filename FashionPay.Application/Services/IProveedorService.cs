using FashionPay.Application.DTOs.Proveedor;

namespace FashionPay.Application.Services;

public interface IProveedorService
{
    Task<IEnumerable<ProveedorResponseDto>> GetProvidersAsync();
    Task<ProveedorResponseDto?> GetProviderByIdAsync(int id);
    Task<IEnumerable<ProveedorResponseDto>> GetProvidersWithFiltersAsync(ProveedorFiltrosDto filtros);
    Task<ProveedorResponseDto> CreateProviderAsync(ProveedorCreateDto proveedorDto);
    Task<ProveedorResponseDto> UpdateProviderAsync(int id, ProveedorUpdateDto proveedorDto);
    Task DesactivateProviderAsync(int id);
    Task<ProveedorResponseDto> ReactivateProviderAsync(int id);
    Task<bool> ExistProviderAsync(int id);
}