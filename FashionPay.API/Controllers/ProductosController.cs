using AutoMapper;
using FashionPay.Application.DTOs.Producto;
using FashionPay.Application.Services;
using FashionPay.Application.Common;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
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
        var productos = await _productoService.GetProductsActiveAsync();
        return Ok(productos);
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
        var productoDto = await _productoService.GetProductByIdAsync(id);
        if (productoDto == null)
        {
            return NotFound(new { message = $"Producto con ID {id} no encontrado" });
        }
        return Ok(productoDto);
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
        var productoDto = await _productoService.GetProductByCodeAsync(codigo);
        if (productoDto == null)
            return NotFound(new { message = $"Producto con CODIGO {codigo} no encontrado" });

        return Ok(productoDto);
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
        var productosDto = await _productoService.GetProductsByProviderAsync(proveedorId);
        if (productosDto == null)
            return NotFound(new { message = $"Productos con ID {proveedorId} de proveedor no encontrados" });

        return Ok(productosDto);
    }

    /// <summary>
    /// Crea un nuevo producto - Solo Admin
    /// </summary>
    [HttpPost]
    [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> CreateProducto(ProductoCreateDto productoDto)
    {
        var producto = await _productoService.CreateProductAsync(productoDto);
        return CreatedAtAction("GetProduct", new { id = producto.IdProducto }, producto);
    }

    /// <summary>
    /// Actualiza un producto existente - Solo Admin
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductoResponseDto>> UpdateProducto(int id, ProductoUpdateDto productoDto)
    {
        var producto = await _productoService.UpdateProductAsync(id, productoDto);
        return Ok(producto);
    }

    /// <summary>
    /// Desactiva un producto (soft delete) - Solo Admin
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteProducto(int id)
    {
        await _productoService.DeleteProductAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Reactiva un producto (soft delete) - Solo Admin
    /// </summary>
    [HttpPatch("reactivate/{id}")]
    [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ReactivateProducto(int id)
    {
        await _productoService.ReactivateProductAsync(id);
        return NoContent();
    }
}