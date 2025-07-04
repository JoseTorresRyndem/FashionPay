using Microsoft.AspNetCore.Mvc;
using FashionPay.Application.Services;
using FashionPay.Application.DTOs.Proveedor;
using FashionPay.Application.Exceptions;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorService _proveedorService;
    private readonly ILogger<ProveedoresController> _logger;

    public ProveedoresController(IProveedorService proveedorService, ILogger<ProveedoresController> logger)
    {
        _proveedorService = proveedorService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los proveedores
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProveedorResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProveedorResponseDto>>> GetProviders()
    {
        try
        {
            var proveedores = await _proveedorService.GetProvidersAsync();
            _logger.LogInformation("Se obtuvieron {Count} proveedores", proveedores.Count());
            return Ok(proveedores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedores");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
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
        try
        {
            var proveedor = await _proveedorService.GetProviderByIdAsync(id);
            if (proveedor == null)
            {
                _logger.LogWarning("Proveedor con ID {ProveedorId} no encontrado", id);
                return NotFound(new { message = $"Proveedor con ID {id} no encontrado" });
            }

            return Ok(proveedor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedor {ProveedorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Busca proveedores con filtros
    /// </summary>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<ProveedorResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProveedorResponseDto>>> SearchProviders([FromQuery] ProveedorFiltrosDto filtros)
    {
        try
        {
            var proveedores = await _proveedorService.GetProvidersWithFiltersAsync(filtros);
            _logger.LogInformation("Búsqueda de proveedores devolvió {Count} resultados", proveedores.Count());

            return Ok(proveedores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar proveedores con filtros");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
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
        try
        {
            var proveedor = await _proveedorService.CreateProviderAsync(proveedorDto);
            _logger.LogInformation("Proveedor creado: {ProveedorId} - Nombre: {Nombre}",
                proveedor.IdProveedor, proveedor.Nombre);

            return CreatedAtAction("GetProviderById", new { id = proveedor.IdProveedor }, proveedor);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning("Error de negocio al crear proveedor: {Error}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al crear proveedor");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
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
        try
        {
            var proveedor = await _proveedorService.UpdateProviderAsync(id, proveedorDto);
            _logger.LogInformation("Proveedor actualizado: {ProveedorId} - Nombre: {Nombre}",
                proveedor.IdProveedor, proveedor.Nombre);

            return Ok(proveedor);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Proveedor no encontrado: {Error}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning("Error de negocio al actualizar proveedor: {Error}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al actualizar proveedor {ProveedorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
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
        try
        {
            await _proveedorService.DesactivateProviderAsync(id);
            _logger.LogInformation("Proveedor desactivado: {ProveedorId}", id);

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Proveedor no encontrado: {Error}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning("Error de negocio al desactivar proveedor: {Error}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al desactivar proveedor {ProveedorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
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
        try
        {
            var proveedor = await _proveedorService.ReactivateProviderAsync(id);
            _logger.LogInformation("Proveedor reactivado: {ProveedorId}", id);

            return Ok(proveedor);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Proveedor no encontrado: {Error}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al reactivar proveedor {ProveedorId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}