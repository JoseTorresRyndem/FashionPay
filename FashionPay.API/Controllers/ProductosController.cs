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
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    /// <summary>
    /// Obtiene todos los productos activos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetProducts()
    {
        try
        {
            var productos = await _productoService.GetProductsActiveAsync();
            return Ok(productos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
        }
    }

    /// <summary>
    /// Obtiene un producto por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> GetProduct(int id)
    {
        try
        {
            var productoDto = await _productoService.GetProductByIdAsync(id);
            if (productoDto == null)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado" });
            }
            return Ok(productoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
        }
    }

    /// <summary>
    /// Busca producto por código
    /// </summary>
    [HttpGet("codigo/{codigo}")]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> GetProductByCode(string codigo)
    {
        try
        {
            var productoDto = await _productoService.GetProductByCodeAsync(codigo);
            if (productoDto == null)
                return NotFound(new { message = $"Producto con CODIGO {codigo} no encontrado" });

            return Ok(productoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
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
            var productosDto = await _productoService.GetProductsByProviderAsync(proveedorId);
            if (productosDto == null)
                return NotFound(new { message = $"Productos con ID {proveedorId} de proveedor no encontrados" });


            return Ok(productosDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
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

            var productosDto = await _productoService.SearchProductsAsync(termino);


            return Ok(productosDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
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
            var producto = await _productoService.CreateProductAsync(productoDto);

            return CreatedAtAction("GetProduct", new { id = producto.IdProducto }, producto);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
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
            var producto = await _productoService.UpdateProductAsync(id, productoDto);

            return Ok(producto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
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
            await _productoService.DeleteProductAsync(id);


            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
        }
    }
}