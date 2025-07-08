using Microsoft.AspNetCore.Mvc;
using FashionPay.Application.Services;
using FashionPay.Application.DTOs.Abono;
using FashionPay.Application.Exceptions;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AbonosController : ControllerBase
{
    private readonly IAbonoService _abonoService;
    public AbonosController(IAbonoService abonoService )
    {
        _abonoService = abonoService;

    }

    /// <summary>
    /// Obtiene todos los abonos con filtros opcionales
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AbonoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AbonoResponseDto>>> GetAbonos([FromQuery] AbonoFiltrosDto filtros)
    {
        try
        {
            var abonos = await _abonoService.GetPaymentsWithFiltersAsync(filtros);
            return Ok(abonos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un abono por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AbonoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AbonoResponseDto>> GetAbono(int id)
    {
        try
        {
            var abono = await _abonoService.GetPaymentByIdAsync(id);
            if (abono == null)
            {
                return NotFound(new { message = $"Abono con ID {id} no encontrado" });
            }

            return Ok(abono);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene abonos por cliente
    /// </summary>
    [HttpGet("cliente/{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<AbonoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AbonoResponseDto>>> GetAbonosByCliente(int clienteId)
    {
        try
        {
            var abonos = await _abonoService.GetPaymentsByClientAsync(clienteId);
            return Ok(abonos);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el resumen de pagos de un cliente
    /// </summary>
    [HttpGet("cliente/{clienteId}/resumen")]
    [ProducesResponseType(typeof(ResumenPagosClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResumenPagosClienteDto>> GetResumenPagosCliente(int clienteId)
    {
        try
        {
            var resumen = await _abonoService.GetClientPaymentSummaryAsync(clienteId);
                  return Ok(resumen);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Registra un nuevo abono
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AbonoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AbonoResponseDto>> CreateAbono(AbonoCreateDto abonoDto)
    {
        try
        {
            var abono = await _abonoService.RegisterPaymentAsync(abonoDto);

            return CreatedAtAction("GetAbono", new { id = abono.IdAbono }, abono);
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
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}