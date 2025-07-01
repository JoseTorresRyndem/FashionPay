using AutoMapper;
using FashionPay.Application.DTOs.Producto;
using FashionPay.Application.Exceptions;
using FashionPay.Application.Services;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductosController> _logger;
    private readonly IProductoService _productoService;

    public ProductosController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductosController> logger, IProductoService productoService)
    {
        _unitOfWork = unitOfWork;
        _productoService = productoService;
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
            var productos = await _productoService.GetProductosActivosAsync();
            _logger.LogInformation("Se obtuvieron {Count} productos activos", productos.Count());
            return Ok(productos);
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
            var productoDto = await _productoService.GetProductoByIdAsync(id);
            if (productoDto == null)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }
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
            var productoDto = await _productoService.GetProductoByCodigoAsync(codigo);
            if (productoDto == null)
                return NotFound(new { message = $"Producto con CODIGO {codigo} no encontrado" });

            _logger.LogInformation("Se obtuvo el producto con el codigo: {codigo}", productoDto);
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
            var productosDto = await _productoService.GetProductosByProveedorAsync(proveedorId);
            if (productosDto == null)
                return NotFound(new { message = $"Productos con ID {proveedorId} de proveedor no encontrados" });

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
                return NotFound(new { message = "El término de búsqueda debe tener al menos 2 caracteres" });

            var productosDto = await _productoService.BuscarProductosAsync(termino);

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
            var producto = await _productoService.CrearProductoAsync(productoDto);
            _logger.LogInformation("Producto creado: {ProductoId} - {Codigo}", producto.Id, producto.Codigo);

            return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning("Error de negocio al crear producto: {Error}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al crear producto");
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
            var producto = await _productoService.ActualizarProductoAsync(id, productoDto);
            _logger.LogInformation("Producto atualizado con ID: {ProductoId} correctamente", producto.Id);

            return Ok(producto);
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
            await _productoService.EliminarProductoAsync(id);

            _logger.LogInformation("Producto desactivado exitosamente: {ProductoId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto {ProductoId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}