using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FashionPay.Application.Services;
using FashionPay.Application.DTOs.Compra;
using FashionPay.Application.Common;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ComprasController : ControllerBase
{
    private readonly ICompraService _compraService;

    public ComprasController(ICompraService compraService)
    {
        _compraService = compraService;
    }
    /// <summary>
    /// Obtiene todas las compras - Solo SystemAdmin
    /// </summary>
    [HttpGet]
    [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(IEnumerable<CompraResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CompraResponseDto>>> GetCompras()
    {
        var compras = await _compraService.GetPurchasesAsync();
        return Ok(compras);
    }
    /// <summary>
    /// Obtiene una compra por ID - Usuario autorizado
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = BusinessConstants.Roles.Combined.AUTHORIZED_USER)]
    [ProducesResponseType(typeof(CompraResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CompraResponseDto>> GetCompra(int id)
    {
        var compra = await _compraService.GetPurchaseByIdAsync(id);
        if (compra == null)
        {
            return NotFound(new { message = $"Compra con ID {id} no encontrada" });
        }

        return Ok(compra);
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
        var compras = await _compraService.GetPurchasesByClientAsync(clienteId);
        return Ok(compras);
    }
    /// <summary>
    /// Busca compras con filtros - Solo SystemAdmin
    /// </summary>
    [HttpGet("buscar")]
    [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(IEnumerable<CompraResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CompraResponseDto>>> BuscarCompras([FromQuery] CompraFiltrosDto filtros)
    {
        var compras = await _compraService.GetPurchasesWithFiltersAsync(filtros);
        return Ok(compras);
    }
    /// <summary>
    /// Crea una nueva compra a crédito - Solo User
    /// </summary>
    [HttpPost]
    [Authorize(Roles = BusinessConstants.Roles.USER)]
    [ProducesResponseType(typeof(CompraResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CompraResponseDto>> CreateCompra(CompraCreateDto compraDto)
    {
        var compra = await _compraService.CreatePurchaseAsync(compraDto);
        return CreatedAtAction("GetCompra", new { id = compra.IdCompra }, compra);
    }
}
