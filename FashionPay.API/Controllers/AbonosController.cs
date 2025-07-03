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
    private readonly ILogger<AbonosController> _logger;

    public AbonosController(IAbonoService abonoService, ILogger<AbonosController> logger)
    {
        _abonoService = abonoService;
        _logger = logger;
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
            var abonos = await _abonoService.GetAbonosConFiltrosAsync(filtros);
            _logger.LogInformation("Se obtuvieron {Count} abonos", abonos.Count());
            return Ok(abonos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener abonos");
            return StatusCode(500, new { message = "Error interno del servidor" });
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
            var abono = await _abonoService.GetAbonoByIdAsync(id);
            if (abono == null)
            {
                _logger.LogWarning("Abono con ID {AbonoId} no encontrado", id);
                return NotFound(new { message = $"Abono con ID {id} no encontrado" });
            }

            return Ok(abono);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener abono {AbonoId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
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
            var abonos = await _abonoService.GetAbonosByClienteAsync(clienteId);
            _logger.LogInformation("Se obtuvieron {Count} abonos para el cliente {ClienteId}",
                abonos.Count(), clienteId);

            return Ok(abonos);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Cliente no encontrado: {Error}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener abonos del cliente {ClienteId}", clienteId);
            return StatusCode(500, new { message = "Error interno del servidor" });
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
            var resumen = await _abonoService.GetResumenPagosClienteAsync(clienteId);
            _logger.LogInformation("Resumen generado para cliente {ClienteId}", clienteId);

            return Ok(resumen);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Cliente no encontrado: {Error}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen del cliente {ClienteId}", clienteId);
            return StatusCode(500, new { message = "Error interno del servidor" });
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
            var abono = await _abonoService.RegistrarAbonoAsync(abonoDto);
            _logger.LogInformation("Abono registrado: {AbonoId} - Cliente: {ClienteId} - Monto: ${MontoAbono:F2}",
                abono.IdAbono, abono.Cliente.IdCliente, abono.MontoAbono);

            return CreatedAtAction("GetAbono", new { id = abono.IdAbono }, abono);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning("Error de negocio al registrar abono: {Error}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Entidad no encontrada: {Error}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al registrar abono");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}