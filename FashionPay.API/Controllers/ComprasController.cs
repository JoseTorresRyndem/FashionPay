using Microsoft.AspNetCore.Mvc;
using FashionPay.Application.Services;
using FashionPay.Application.DTOs.Compra;
using FashionPay.Application.Exceptions;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ComprasController : ControllerBase
{
    private readonly ICompraService _compraService;

    public ComprasController(ICompraService compraService)
    {
        _compraService = compraService;
    }
    /// <summary>
    /// Obtiene todas las compras
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompraResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CompraResponseDto>>> GetCompras()
    {
        try
        {
            var compras = await _compraService.GetPurchasesAsync();
            return Ok(compras);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
        }
    }
    /// <summary>
    /// Obtiene una compra por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompraResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CompraResponseDto>> GetCompra(int id)
    {
        try
        {
            var compra = await _compraService.GetPurchaseByIdAsync(id);
            if (compra == null)
            {
                return NotFound(new { message = $"Compra con ID {id} no encontrada" });
            }

            return Ok(compra);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
        }
    }
    /// <summary>
    /// Obtiene compras por cliente
    /// </summary>
    [HttpGet("cliente/{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<CompraResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CompraResponseDto>>> GetComprasByCliente(int clienteId)
    {
        try
        {
            var compras = await _compraService.GetPurchasesByClientAsync(clienteId);

            return Ok(compras);
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
    /// <summary>
    /// Busca compras con filtros
    /// </summary>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<CompraResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CompraResponseDto>>> BuscarCompras([FromQuery] CompraFiltrosDto filtros)
    {
        try
        {
            var compras = await _compraService.GetPurchasesWithFiltersAsync(filtros);

            return Ok(compras);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message  });
        }
    }
    /// <summary>
    /// Crea una nueva compra a crédito
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompraResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CompraResponseDto>> CreateCompra(CompraCreateDto compraDto)
    {
        try
        {
            var compra = await _compraService.CreatePurchaseAsync(compraDto);

            return CreatedAtAction("GetCompra", new { id = compra.IdCompra }, compra);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
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
