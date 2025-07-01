using FashionPay.Application.DTOs.Proveedor;

namespace FashionPay.Application.Services;

public interface IProveedorService
{
    Task<IEnumerable<ProveedorResponseDto>> GetProveedoresAsync();
    Task<ProveedorResponseDto?> GetProveedorByIdAsync(int id);
    Task<IEnumerable<ProveedorResponseDto>> GetProveedoresConFiltrosAsync(ProveedorFiltrosDto filtros);
    Task<ProveedorResponseDto> CrearProveedorAsync(ProveedorCreateDto proveedorDto);
    Task<ProveedorResponseDto> ActualizarProveedorAsync(int id, ProveedorUpdateDto proveedorDto);
    Task DesactivarProveedorAsync(int id);
    Task<ProveedorResponseDto> ReactivarProveedorAsync(int id);
    Task<bool> ExisteProveedorAsync(int id);
    Task<bool> ExisteProveedorPorNombreAsync(string nombre, int? excludeId = null);
}