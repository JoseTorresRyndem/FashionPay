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
        try
        {
            var proveedores = await _proveedorService.GetProvidersAsync();
            return Ok(proveedores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
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
                return NotFound(new { message = $"Proveedor con ID {id} no encontrado" });
            }

            return Ok(proveedor);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
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

            return Ok(proveedores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
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

            return CreatedAtAction(nameof(GetProviderById), new { id = proveedor.IdProveedor }, proveedor);
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

            return Ok(proveedor);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

            return Ok(proveedor);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
        }
    }
}