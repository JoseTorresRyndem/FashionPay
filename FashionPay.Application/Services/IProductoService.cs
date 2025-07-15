using FashionPay.Application.DTOs.Producto;

namespace FashionPay.Application.Services;

public interface IProductoService
{
    Task<ProductoResponseDto> CreateProductAsync(ProductoCreateDto productoDto);
    Task<ProductoResponseDto?> GetProductByIdAsync(int id);
    Task<ProductoResponseDto?> GetProductByCodeAsync(string codigo);
    Task<IEnumerable<ProductoResponseDto>> GetProductsActiveAsync();
    Task<IEnumerable<ProductoResponseDto>> GetProductsByProviderAsync(int proveedorId);
    Task<ProductoResponseDto> UpdateProductAsync(int id, ProductoUpdateDto productoDto);
    Task<bool> DeleteProductAsync(int id);
    Task<ProductoResponseDto> ReactivateProductAsync(int id);
}