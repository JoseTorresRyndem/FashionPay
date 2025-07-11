using AutoMapper;
using FashionPay.Application.DTOs.Cliente;
using FashionPay.Application.Services;
using FashionPay.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionPay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;

        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientes()
        {
            var clientes = await _clienteService.GetClientsAsync();
            return Ok(clientes);
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
            var cliente = await _clienteService.GetClientByIdAsync(id);
            if (cliente == null)
            {
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
            }

            return Ok(cliente);
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
            var cliente = await _clienteService.GetClientByEmailAsync(email);
            if (cliente == null)
            {
                return NotFound(new { message = $"Cliente con email {email} no encontrado" });
            }

            return Ok(cliente);
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
            var clientes = await _clienteService.GetClientsByClassificationAsync(clasificacion);
            return Ok(clientes);
        }
        
        /// <summary>
        /// Obtiene el estado financiero de un cliente - Solo Operator
        /// </summary>
        [HttpGet("{id}/estado-financiero")]
        [Authorize(Roles = BusinessConstants.Roles.OPERATOR)]
        [ProducesResponseType(typeof(EstadoCuentaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EstadoCuentaDto>> GetEstadoFinanciero(int id)
        {
            var estadoCuenta = await _clienteService.GetClientAccountStatusAsync(id);
            if (estadoCuenta == null)
            {
                return NotFound(new { message = $"Estado de cuenta para cliente con ID {id} no encontrado" });
            }

            return Ok(estadoCuenta);
        }
        /// <summary>
        /// Crea un nuevo cliente - Solo Admin
        /// </summary>
        [HttpPost]
        [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
        [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponseDto>> CreateCliente(ClienteCreateDto clienteDto)
        {
            var cliente = await _clienteService.CreateClientAsync(clienteDto);
            return CreatedAtAction("GetCliente", new { id = cliente.IdCliente }, cliente);
        }

        /// <summary>
        /// Actualiza un cliente existente - Solo Admin
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
        [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponseDto>> UpdateCliente(int id, ClienteUpdateDto clienteDto)
        {
            var cliente = await _clienteService.UpdateClientAsync(id, clienteDto);
            return Ok(cliente);
        }


        /// <summary>
        /// Desactiva un cliente (soft delete) - Solo Admin
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCliente(int id)
        {
            await _clienteService.DeleteClientAsync(id);
            return NoContent();
        }


        /// <summary>
        /// Recalcula el estado de cuenta de un cliente - Solo Admin
        /// </summary>
        [HttpPost("{id}/recalcular-saldo")]
        [Authorize(Roles = BusinessConstants.Roles.ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RecalcularSaldo(int id)
        {
            await _clienteService.RecalculateBalanceAsync(id);
            return Ok(new
            {
                message = "Saldo recalculado exitosamente",
                clienteId = id,
                timestamp = DateTime.Now
            });
        }
    }
}
