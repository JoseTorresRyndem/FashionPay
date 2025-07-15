using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FashionPay.Application.Services;
using FashionPay.Application.DTOs.Proveedor;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorService _proveedorService;

    public ProveedoresController(IProveedorService proveedorService)
    {
        _proveedorService = proveedorService;
    }

    /// <summary>
    /// Obtiene todos los proveedores
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProveedorResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProveedorResponseDto>>> GetProviders()
    {
        var proveedores = await _proveedorService.GetProvidersAsync();
        return Ok(proveedores);
    }

    /// <summary>
    /// Obtiene un proveedor por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProveedorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProveedorResponseDto>> GetProviderById(int id)
    {
        var proveedor = await _proveedorService.GetProviderByIdAsync(id);
        if (proveedor == null)
        {
            return NotFound(new { message = $"Proveedor con ID {id} no encontrado" });
        }

        return Ok(proveedor);
    }

    /// <summary>
    /// Busca proveedores con filtros
    /// </summary>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<ProveedorResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProveedorResponseDto>>> SearchProviders([FromQuery] ProveedorFiltrosDto filtros)
    {
        var proveedores = await _proveedorService.GetProvidersWithFiltersAsync(filtros);
        return Ok(proveedores);
    }

    /// <summary>
    /// Crea un nuevo proveedor
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProveedorResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProveedorResponseDto>> CreateProveedor(ProveedorCreateDto proveedorDto)
    {
        var proveedor = await _proveedorService.CreateProviderAsync(proveedorDto);
        return CreatedAtAction(nameof(GetProviderById), new { id = proveedor.IdProveedor }, proveedor);
    }

    /// <summary>
    /// Actualiza un proveedor existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProveedorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProveedorResponseDto>> UpdateProveedor(int id, ProveedorUpdateDto proveedorDto)
    {
        var proveedor = await _proveedorService.UpdateProviderAsync(id, proveedorDto);
        return Ok(proveedor);
    }

    /// <summary>
    /// Desactiva un proveedor (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteProveedor(int id)
    {
        await _proveedorService.DesactivateProviderAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Reactiva un proveedor desactivado
    /// </summary>
    [HttpPatch("{id}/reactivar")]
    [ProducesResponseType(typeof(ProveedorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProveedorResponseDto>> ReactivarProveedor(int id)
    {
        var proveedor = await _proveedorService.ReactivateProviderAsync(id);
        return Ok(proveedor);
    }
}