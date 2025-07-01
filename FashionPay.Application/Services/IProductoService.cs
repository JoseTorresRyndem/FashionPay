using FashionPay.Application.DTOs.Producto;

namespace FashionPay.Application.Services;

public interface IProductoService
{
    Task<ProductoResponseDto> CrearProductoAsync(ProductoCreateDto productoDto);
    Task<ProductoResponseDto?> GetProductoByIdAsync(int id);
    Task<ProductoResponseDto?> GetProductoByCodigoAsync(string codigo);
    Task<IEnumerable<ProductoResponseDto>> GetProductosActivosAsync();
    Task<IEnumerable<ProductoResponseDto>> GetProductosByProveedorAsync(int proveedorId);
    Task<IEnumerable<ProductoResponseDto>> BuscarProductosAsync(string termino);
    Task<ProductoResponseDto> ActualizarProductoAsync(int id, ProductoUpdateDto productoDto);
    Task<bool> EliminarProductoAsync(int id);
}