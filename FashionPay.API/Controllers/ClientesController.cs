using AutoMapper;
using FashionPay.Application.DTOs.Cliente;
using FashionPay.Application.Exceptions;
using FashionPay.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FashionPay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientes()
        {
            try
            {
                var clientes = await _clienteService.GetClientesAsync();
                _logger.LogInformation("Se obtuvieron {Count} clientes", clientes.Count());
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
        /// <summary>
        /// Obtiene un cliente por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponseDto>> GetCliente(int id)
        {
            try
            {
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {ClienteId} no encontrado", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente {ClienteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Busca cliente por email
        /// </summary>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponseDto>> GetClienteByEmail(string email)
        {
            try
            {
                var cliente = await _clienteService.GetClienteByEmailAsync(email);
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con email {Email} no encontrado", email);
                    return NotFound(new { message = $"Cliente con email {email} no encontrado" });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar cliente por email {Email}", email);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
        /// <summary>
        /// Obtiene clientes por clasificación
        /// </summary>
        [HttpGet("clasificacion/{clasificacion}")]
        [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientesByClasificacion(string clasificacion)
        {
            try
            {
                var clientes = await _clienteService.GetClientesByClasificacionAsync(clasificacion);
                _logger.LogInformation("Se obtuvieron {Count} clientes con clasificación {Clasificacion}",
                    clientes.Count(), clasificacion.ToUpper());

                return Ok(clientes);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Error de validación: {Error}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes por clasificación {Clasificacion}", clasificacion);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponseDto>> CreateCliente(ClienteCreateDto clienteDto)
        {
            try
            {
                var cliente = await _clienteService.CrearClienteAsync(clienteDto);
                _logger.LogInformation("Cliente creado: {ClienteId} - {Email}", cliente.IdCliente, cliente.Email);

                return CreatedAtAction("GetCliente", new { id = cliente.IdCliente }, cliente);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Error de negocio al crear cliente: {Error}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al crear cliente");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }

        }

        /// <summary>
        /// Actualiza un cliente existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponseDto>> UpdateCliente(int id, ClienteUpdateDto clienteDto)
        {
            try
            {
                var cliente = await _clienteService.ActualizarClienteAsync(id, clienteDto);
                _logger.LogInformation("Cliente actualizado: {ClienteId}", id);

                return Ok(cliente);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Cliente no encontrado: {Error}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Error de negocio: {Error}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar cliente {ClienteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }


        /// <summary>
        /// Desactiva un cliente (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCliente(int id)
        {
            try
            {
                await _clienteService.EliminarClienteAsync(id);
                _logger.LogInformation("Cliente eliminado: {ClienteId}", id);

                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Cliente no encontrado: {Error}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Error de negocio: {Error}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al eliminar cliente {ClienteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }


        /// <summary>
        /// Recalcula el estado de cuenta de un cliente
        /// </summary>
        [HttpPost("{id}/recalcular-saldo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RecalcularSaldo(int id)
        {
            try
            {
                await _clienteService.RecalcularSaldoAsync(id);
                _logger.LogInformation("Saldo recalculado para cliente: {ClienteId}", id);

                return Ok(new
                {
                    message = "Saldo recalculado exitosamente",
                    clienteId = id,
                    timestamp = DateTime.Now
                });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Cliente no encontrado: {Error}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al recalcular saldo {ClienteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
