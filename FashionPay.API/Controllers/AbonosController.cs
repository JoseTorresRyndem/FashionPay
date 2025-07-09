using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FashionPay.Application.Services;
using FashionPay.Application.DTOs.Abono;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
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
        var abonos = await _abonoService.GetPaymentsWithFiltersAsync(filtros);
        return Ok(abonos);
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
        var abono = await _abonoService.GetPaymentByIdAsync(id);
        if (abono == null)
        {
            return NotFound(new { message = $"Abono con ID {id} no encontrado" });
        }

        return Ok(abono);
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
        var abonos = await _abonoService.GetPaymentsByClientAsync(clienteId);
        return Ok(abonos);
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
        var resumen = await _abonoService.GetClientPaymentSummaryAsync(clienteId);
        return Ok(resumen);
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
        var abono = await _abonoService.RegisterPaymentAsync(abonoDto);
        return CreatedAtAction("GetAbono", new { id = abono.IdAbono }, abono);
    }
}