using AutoMapper;
using FashionPay.Core.Interfaces;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Producto;
using FashionPay.Application.Exceptions;

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
        // Validaciones de negocio
        await ValidateCreationProductAsync(productoDto);

        // Mapear y crear
        var producto = _mapper.Map<Producto>(productoDto);
        var productoCreado = await _unitOfWork.Productos.AddAsync(producto);

        // Obtener producto completo con proveedor
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

    public async Task<IEnumerable<ProductoResponseDto>?> GetProductsByProviderAsync(int proveedorId)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(proveedorId);
        var productos = await _unitOfWork.Productos.GetProductsByProviderAsync(proveedorId);
        return productos != null ? _mapper.Map<IEnumerable<ProductoResponseDto>>(productos) : null;
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
            throw new NotFoundException($"Producto con ID {id} no encontrado");

        await ValidateUpdatingProductAsync(id, productoDto);

        _mapper.Map(productoDto, producto);
        await _unitOfWork.Productos.UpdateAsync(producto);

        // Retornar producto actualizado
        var productoActualizado = await _unitOfWork.Productos.GetByIdAsync(id);
        return _mapper.Map<ProductoResponseDto>(productoActualizado!);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        if (producto == null)
            throw new NotFoundException($"Producto con ID {id} no encontrado");

        // TODO: Verificar que no esté en compras pendientes cuando implementemos CompraRepository

        // Soft delete
        producto.Activo = false;
        await _unitOfWork.Productos.UpdateAsync(producto);

        return true;
    }
    private async Task ValidateCreationProductAsync(ProductoCreateDto productoDto)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(productoDto.IdProveedor);
        if (proveedor == null || !proveedor.Activo)
            throw new BusinessException("El proveedor seleccionado no existe o está inactivo");

        var productoExistente = await _unitOfWork.Productos.GetByCodeAsync(productoDto.Codigo);
        if (productoExistente != null)
            throw new BusinessException($"Ya existe un producto con el código '{productoDto.Codigo}'");
    }

    private async Task ValidateUpdatingProductAsync(int id, ProductoUpdateDto productoDto)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(productoDto.IdProveedor);
        if (proveedor == null || !proveedor.Activo)
            throw new BusinessException("El proveedor seleccionado no existe o está inactivo");
    }
}