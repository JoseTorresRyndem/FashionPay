using AutoMapper;
using FashionPay.Core.Interfaces;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Producto;

namespace FashionPay.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductoResponseDto> CreateProductAsync(ProductoCreateDto productoDto)
    {
        await ValidateCreationProductAsync(productoDto);

        var producto = _mapper.Map<Producto>(productoDto);
        var productoCreado = await _unitOfWork.Productos.AddAsync(producto);
        await _unitOfWork.SaveChangesAsync();
        
        var productoCompleto = await _unitOfWork.Productos.GetByIdAsync(productoCreado.IdProducto);
        return _mapper.Map<ProductoResponseDto>(productoCompleto!);
    }

    public async Task<ProductoResponseDto?> GetProductByIdAsync(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        return producto != null ? _mapper.Map<ProductoResponseDto>(producto) : null;
    }

    public async Task<ProductoResponseDto?> GetProductByCodeAsync(string codigo)
    {
        var producto = await _unitOfWork.Productos.GetByCodeAsync(codigo);
        return producto != null ? _mapper.Map<ProductoResponseDto>(producto) : null;
    }

    public async Task<IEnumerable<ProductoResponseDto>> GetProductsActiveAsync()
    {
        var productos = await _unitOfWork.Productos.GetProductsActiveAsync();
        return _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);
    }

    public async Task<IEnumerable<ProductoResponseDto>> GetProductsByProviderAsync(int proveedorId)
    {
        var productos = await _unitOfWork.Productos.GetProductsByProviderAsync(proveedorId);
        return _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);
    }

    public async Task<IEnumerable<ProductoResponseDto>> SearchProductsAsync(string termino)
    {
        var productos = await _unitOfWork.Productos.SearchProductsAsync(termino);
        return _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);
    }

    public async Task<ProductoResponseDto> UpdateProductAsync(int id, ProductoUpdateDto productoDto)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        if (producto == null)
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

        await ValidateUpdatingProductAsync(id, productoDto);

        _mapper.Map(productoDto, producto);
        await _unitOfWork.Productos.UpdateAsync(producto);
        await _unitOfWork.SaveChangesAsync();

        // Retornar producto actualizado
        var productoActualizado = await _unitOfWork.Productos.GetByIdAsync(id);
        return _mapper.Map<ProductoResponseDto>(productoActualizado!);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        if (producto == null)
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado");

        // Verificar que no esté en compras activas
        var comprasActivas = await _unitOfWork.Compras.GetComprasWithProductAsync(id);
        if (comprasActivas.Any(c => c.EstadoCompra == "ACTIVA"))
            throw new InvalidOperationException($"No se puede eliminar el producto porque está asociado a {comprasActivas.Count(c => c.EstadoCompra == "ACTIVA")} compras activas");
        
        // Soft delete
        producto.Activo = false;
        await _unitOfWork.Productos.UpdateAsync(producto);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
    public async Task<ProductoResponseDto> ReactivateProductAsync(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        if (producto == null)
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
            
        if (producto.Activo == true)
            throw new InvalidOperationException($"El producto con ID {id} ya está activo");

        producto.Activo = true;
        await _unitOfWork.Productos.UpdateAsync(producto);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductoResponseDto>(producto);
    }
    private async Task ValidateCreationProductAsync(ProductoCreateDto productoDto)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(productoDto.IdProveedor);
        if (proveedor == null || !proveedor.Activo)
            throw new ArgumentException("El proveedor seleccionado no existe o está inactivo");

        var productoExistente = await _unitOfWork.Productos.GetByCodeAsync(productoDto.Codigo);
        if (productoExistente != null)
            throw new ArgumentException($"Ya existe un producto con el código '{productoDto.Codigo}'");
    }

    private async Task ValidateUpdatingProductAsync(int id, ProductoUpdateDto productoDto)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(productoDto.IdProveedor);
        if (proveedor == null || !proveedor.Activo)
            throw new ArgumentException("El proveedor seleccionado no existe o está inactivo");
    }
}