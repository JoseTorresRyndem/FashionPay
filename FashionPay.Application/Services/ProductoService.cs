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

    public async Task<ProductoResponseDto> CrearProductoAsync(ProductoCreateDto productoDto)
    {
        // Validaciones de negocio
        await ValidarCreacionProductoAsync(productoDto);

        // Mapear y crear
        var producto = _mapper.Map<Producto>(productoDto);
        var productoCreado = await _unitOfWork.Productos.AddAsync(producto);

        // Obtener producto completo con proveedor
        var productoCompleto = await _unitOfWork.Productos.GetByIdAsync(productoCreado.Id);
        return _mapper.Map<ProductoResponseDto>(productoCompleto!);
    }

    public async Task<ProductoResponseDto?> GetProductoByIdAsync(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        return producto != null ? _mapper.Map<ProductoResponseDto>(producto) : null;
    }

    public async Task<ProductoResponseDto?> GetProductoByCodigoAsync(string codigo)
    {
        var producto = await _unitOfWork.Productos.GetByCodigoAsync(codigo);
        return producto != null ? _mapper.Map<ProductoResponseDto>(producto) : null;
    }

    public async Task<IEnumerable<ProductoResponseDto>> GetProductosActivosAsync()
    {
        var productos = await _unitOfWork.Productos.GetProductosActivosAsync();
        return _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);
    }

    public async Task<IEnumerable<ProductoResponseDto>?> GetProductosByProveedorAsync(int proveedorId)
    {
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(proveedorId);
        var productos = await _unitOfWork.Productos.GetProductosByProveedorAsync(proveedorId);
        return productos != null ? _mapper.Map<IEnumerable<ProductoResponseDto>>(productos) : null;
    }

    public async Task<IEnumerable<ProductoResponseDto>> BuscarProductosAsync(string termino)
    {
        var productos = await _unitOfWork.Productos.BuscarProductosAsync(termino);
        return _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);
    }

    public async Task<ProductoResponseDto> ActualizarProductoAsync(int id, ProductoUpdateDto productoDto)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);
        if (producto == null)
            throw new NotFoundException($"Producto con ID {id} no encontrado");

        // Validaciones de negocio
        await ValidarActualizacionProductoAsync(id, productoDto);

        // Mapear cambios y actualizar
        _mapper.Map(productoDto, producto);
        await _unitOfWork.Productos.UpdateAsync(producto);

        // Retornar producto actualizado
        var productoActualizado = await _unitOfWork.Productos.GetByIdAsync(id);
        return _mapper.Map<ProductoResponseDto>(productoActualizado!);
    }

    public async Task<bool> EliminarProductoAsync(int id)
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
    private async Task ValidarCreacionProductoAsync(ProductoCreateDto productoDto)
    {
        // Validar que el proveedor existe y está activo
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(productoDto.IdProveedor);
        if (proveedor == null || !proveedor.Activo)
            throw new BusinessException("El proveedor seleccionado no existe o está inactivo");

        // Validar código único
        var productoExistente = await _unitOfWork.Productos.GetByCodigoAsync(productoDto.Codigo);
        if (productoExistente != null)
            throw new BusinessException($"Ya existe un producto con el código '{productoDto.Codigo}'");
    }

    private async Task ValidarActualizacionProductoAsync(int id, ProductoUpdateDto productoDto)
    {
        // Validar que el proveedor existe y está activo
        var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(productoDto.IdProveedor);
        if (proveedor == null || !proveedor.Activo)
            throw new BusinessException("El proveedor seleccionado no existe o está inactivo");
    }
}