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
    private readonly ILogger<ComprasController> _logger;

    public ComprasController(ICompraService compraService, ILogger<ComprasController> logger)
    {
        _compraService = compraService;
        _logger = logger;
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
            var compras = await _compraService.GetComprasAsync();
            _logger.LogInformation("Se obtuvieron {Count} compras", compras.Count());
            return Ok(compras);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener compras");
            return StatusCode(500, new { message = "Error interno del servidor" });
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
            var compra = await _compraService.GetCompraByIdAsync(id);
            if (compra == null)
            {
                _logger.LogWarning("Compra con ID {CompraId} no encontrada", id);
                return NotFound(new { message = $"Compra con ID {id} no encontrada" });
            }

            return Ok(compra);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener compra {CompraId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
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
            var compras = await _compraService.GetComprasByClienteAsync(clienteId);
            _logger.LogInformation("Se obtuvieron {Count} compras para el cliente {ClienteId}",
                compras.Count(), clienteId);

            return Ok(compras);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Cliente no encontrado: {Error}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener compras del cliente {ClienteId}", clienteId);
            return StatusCode(500, new { message = "Error interno del servidor" });
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
            var compras = await _compraService.GetComprasConFiltrosAsync(filtros);
            _logger.LogInformation("Búsqueda de compras devolvió {Count} resultados", compras.Count());

            return Ok(compras);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar compras con filtros");
            return StatusCode(500, new { message = "Error interno del servidor" });
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
            var compra = await _compraService.CrearCompraAsync(compraDto);
            _logger.LogInformation("Compra creada: {CompraId} - Cliente: {ClienteId} - Monto: ${MontoTotal:F2}",
                compra.IdCompra, compra.Cliente.IdCliente, compra.MontoTotal);

            return CreatedAtAction("GetCompra", new { id = compra.IdCompra }, compra);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning("Error de negocio al crear compra: {Error}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Entidad no encontrada: {Error}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno al crear compra");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
