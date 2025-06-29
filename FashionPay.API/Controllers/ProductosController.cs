using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FashionPay.Core.Interfaces;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Producto;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductosController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los productos activos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetProductos()
    {
        try
        {
            var productos = await _unitOfWork.Productos.GetProductosActivosAsync();
            var productosDto = _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);

            _logger.LogInformation("Se obtuvieron {Count} productos activos", productosDto.Count());
            return Ok(productosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un producto por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> GetProducto(int id)
    {
        try
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(id);

            if (producto == null)
            {
                _logger.LogWarning("Producto con ID {ProductoId} no encontrado", id);
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }

            var productoDto = _mapper.Map<ProductoResponseDto>(producto);
            return Ok(productoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto {ProductoId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Busca producto por código
    /// </summary>
    [HttpGet("codigo/{codigo}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> GetProductoByCodigo(string codigo)
    {
        try
        {
            var producto = await _unitOfWork.Productos.GetByCodigoAsync(codigo);

            if (producto == null)
            {
                _logger.LogWarning("Producto con código {Codigo} no encontrado", codigo);
                return NotFound(new { message = $"Producto con código {codigo} no encontrado" });
            }

            var productoDto = _mapper.Map<ProductoResponseDto>(producto);
            return Ok(productoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar producto por código {Codigo}", codigo);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene productos por proveedor
    /// </summary>
    [HttpGet("proveedor/{proveedorId}")]
    [ProducesResponseType(typeof(IEnumerable<ProductoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetProductosByProveedor(int proveedorId)
    {
        try
        {
            // Verificar que el proveedor existe
            var proveedor = await _unitOfWork.Proveedores.GetByIdAsync(proveedorId);
            if (proveedor == null)
            {
                return NotFound(new { message = $"Proveedor con ID {proveedorId} no encontrado" });
            }

            var productos = await _unitOfWork.Productos.GetProductosByProveedorAsync(proveedorId);
            var productosDto = _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);

            _logger.LogInformation("Se obtuvieron {Count} productos del proveedor {ProveedorId}",
                productosDto.Count(), proveedorId);

            return Ok(productosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos del proveedor {ProveedorId}", proveedorId);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Busca productos por término (nombre, código, descripción)
    /// </summary>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<ProductoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> BuscarProductos([FromQuery] string termino)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
            {
                return BadRequest(new { message = "El término de búsqueda debe tener al menos 2 caracteres" });
            }

            var productos = await _unitOfWork.Productos.BuscarProductosAsync(termino);
            var productosDto = _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);

            _logger.LogInformation("Búsqueda '{Termino}' devolvió {Count} productos",
                termino, productosDto.Count());

            return Ok(productosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar productos con término '{Termino}'", termino);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> CreateProducto(ProductoCreateDto productoDto)
    {
        try
        {
            // FluentValidation se ejecuta automáticamente

            // Mapear DTO a entidad
            var producto = _mapper.Map<Producto>(productoDto);

            // Crear producto
            var productoCreado = await _unitOfWork.Productos.AddAsync(producto);

            _logger.LogInformation("Producto creado exitosamente: {ProductoId} - {Codigo}",
                productoCreado.Id, productoCreado.Codigo);

            // Obtener producto con proveedor para la respuesta
            var productoCompleto = await _unitOfWork.Productos.GetByIdAsync(productoCreado.Id);
            var productoResponse = _mapper.Map<ProductoResponseDto>(productoCompleto);

            return CreatedAtAction(
                "GetProducto",
                new { id = productoCreado.Id },
                productoResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto con código {Codigo}", productoDto.Codigo);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un producto existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> UpdateProducto(int id, ProductoUpdateDto productoDto)
    {
        try
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(id);
            if (producto == null)
            {
                _logger.LogWarning("Intento de actualizar producto inexistente: {ProductoId}", id);
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }

            // Mapear cambios del DTO a la entidad existente
            _mapper.Map(productoDto, producto);

            await _unitOfWork.Productos.UpdateAsync(producto);

            _logger.LogInformation("Producto actualizado exitosamente: {ProductoId}", id);

            // Obtener producto actualizado con proveedor
            var productoActualizado = await _unitOfWork.Productos.GetByIdAsync(id);
            var productoResponse = _mapper.Map<ProductoResponseDto>(productoActualizado);

            return Ok(productoResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto {ProductoId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Desactiva un producto (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteProducto(int id)
    {
        try
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(id);
            if (producto == null)
            {
                _logger.LogWarning("Intento de eliminar producto inexistente: {ProductoId}", id);
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }

            // TODO: Verificar que no esté en compras pendientes
            // var tieneComprasPendientes = await _unitOfWork.Compras.ProductoTieneComprasPendientesAsync(id);
            // if (tieneComprasPendientes)
            // {
            //     return BadRequest(new { message = "No se puede eliminar producto con compras pendientes" });
            // }

            // Soft delete
            producto.Activo = false;
            await _unitOfWork.Productos.UpdateAsync(producto);

            _logger.LogInformation("Producto desactivado exitosamente: {ProductoId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto {ProductoId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza el stock de un producto
    /// </summary>
    [HttpPatch("{id}/stock")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> UpdateStock(int id, [FromBody] int nuevoStock)
    {
        try
        {
            if (nuevoStock < 0)
            {
                return BadRequest(new { message = "El stock no puede ser negativo" });
            }

            var producto = await _unitOfWork.Productos.GetByIdAsync(id);
            if (producto == null)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }

            var stockAnterior = producto.Stock;
            producto.Stock = nuevoStock;

            await _unitOfWork.Productos.UpdateAsync(producto);

            _logger.LogInformation("Stock actualizado para producto {ProductoId}: {StockAnterior} → {StockNuevo}",
                id, stockAnterior, nuevoStock);

            var productoResponse = _mapper.Map<ProductoResponseDto>(producto);
            return Ok(productoResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar stock del producto {ProductoId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene productos con stock bajo (menos de 5 unidades)
    /// </summary>
    [HttpGet("stock-bajo")]
    [ProducesResponseType(typeof(IEnumerable<ProductoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetProductosStockBajo()
    {
        try
        {
            var productos = await _unitOfWork.Productos.FindAsync(p => p.Activo && p.Stock < 5);
            var productosDto = _mapper.Map<IEnumerable<ProductoResponseDto>>(productos);

            _logger.LogInformation("Se encontraron {Count} productos con stock bajo", productosDto.Count());

            return Ok(productosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos con stock bajo");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}