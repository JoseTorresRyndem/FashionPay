using AutoMapper;
using FashionPay.Application.DTOs.Cliente;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionPay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClientesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ClientesController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                var clientes = await _unitOfWork.Clientes.GetAllAsync();
                var clientesDto = _mapper.Map<IEnumerable<ClienteResponseDto>>(clientes);

                _logger.LogInformation("Se obtuvieron {Count} clientes", clientesDto.Count());
                return Ok(clientesDto);
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
                var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);

                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {ClienteId} no encontrado", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                var clienteDto = _mapper.Map<ClienteResponseDto>(cliente);
                return Ok(clienteDto);
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
                var cliente = await _unitOfWork.Clientes.GetByEmailAsync(email);

                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con email {Email} no encontrado", email);
                    return NotFound(new { message = $"Cliente con email {email} no encontrado" });
                }

                var clienteDto = _mapper.Map<ClienteResponseDto>(cliente);
                return Ok(clienteDto);
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
                var clasificacionUpper = clasificacion.ToUpper();
                if (!new[] { "CUMPLIDO", "RIESGOSO", "MOROSO" }.Contains(clasificacionUpper))
                {
                    return BadRequest(new { message = "Clasificación debe ser: CUMPLIDO, RIESGOSO o MOROSO" });
                }

                var clientes = await _unitOfWork.Clientes.GetClientesByClasificacionAsync(clasificacionUpper);
                var clientesDto = _mapper.Map<IEnumerable<ClienteResponseDto>>(clientes);

                _logger.LogInformation("Se obtuvieron {Count} clientes con clasificación {Clasificacion}",
                    clientesDto.Count(), clasificacionUpper);

                return Ok(clientesDto);
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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponseDto>> CreateCliente(ClienteCreateDto clienteDto)
        {

            try
            {

                // Usar procedimiento almacenado sp_AltaCliente que maneja todo
                var clienteCreado = await _unitOfWork.Clientes.CrearClienteConEstadoCuentaAsync(
                    clienteDto.Nombre,
                    clienteDto.Email,
                    clienteDto.Telefono,
                    clienteDto.Direccion,
                    clienteDto.DiaPago,
                    clienteDto.LimiteCredito,
                    clienteDto.CantidadMaximaPagos,
                    clienteDto.ToleranciasMorosidad
                );

                _logger.LogInformation("Cliente creado exitosamente: {ClienteId} - {Email}",
                    clienteCreado.Id, clienteCreado.Email);

                var clienteResponse = _mapper.Map<ClienteResponseDto>(clienteCreado);
                return CreatedAtAction(
                    "GetCliente",
                    new { id = clienteCreado.Id },
                    clienteResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ClienteResponseDto>> UpdateCliente(int id, ClienteUpdateDto clienteDto)
        {
            try
            {
                var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Intento de actualizar cliente inexistente: {ClienteId}", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                // Mapear cambios del DTO a la entidad existente
                _mapper.Map(clienteDto, cliente);

                await _unitOfWork.Clientes.UpdateAsync(cliente);

                _logger.LogInformation("Cliente actualizado exitosamente: {ClienteId}", id);

                var clienteResponse = _mapper.Map<ClienteResponseDto>(cliente);
                return Ok(clienteResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente {ClienteId}", id);
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
                var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Intento de eliminar cliente inexistente: {ClienteId}", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                // Verificar que no tenga deuda pendiente
                var deudaTotal = await _unitOfWork.Clientes.GetDeudaTotalAsync(id);
                if (deudaTotal > 0)
                {
                    _logger.LogWarning("Intento de eliminar cliente con deuda: {ClienteId}, Deuda: {Deuda}",
                        id, deudaTotal);
                    return BadRequest(new
                    {
                        message = "No se puede eliminar cliente con deuda pendiente",
                        deudaTotal
                    });
                }

                // Soft delete
                cliente.Activo = false;
                await _unitOfWork.Clientes.UpdateAsync(cliente);

                _logger.LogInformation("Cliente desactivado exitosamente: {ClienteId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente {ClienteId}", id);
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
                var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Intento de recalcular saldo de cliente inexistente: {ClienteId}", id);
                    return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
                }

                await _unitOfWork.Clientes.ExecuteCalcularSaldoAsync(id);

                _logger.LogInformation("Saldo recalculado para cliente: {ClienteId}", id);

                return Ok(new
                {
                    message = "Saldo recalculado exitosamente",
                    clienteId = id,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recalcular saldo del cliente {ClienteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
