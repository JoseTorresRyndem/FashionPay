using Microsoft.AspNetCore.Mvc;
using FashionPay.Core.Interfaces;

namespace FashionPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TestController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("connection")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            // Probar conexión a BD
            var clientesCount = await _unitOfWork.Clientes.CountAsync();
            var productosCount = await _unitOfWork.Productos.CountAsync();

            return Ok(new
            {
                message = "✅ Conexión exitosa",
                data = new
                {
                    clientes = clientesCount,
                    productos = productosCount,
                    timestamp = DateTime.Now
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = "❌ Error de conexión",
                error = ex.Message
            });
        }
    }

    [HttpGet("repositories")]
    public IActionResult TestRepositories()
    {
        try
        {
            // Verificar que todos los repositorios están inyectados
            var repositories = new
            {
                clientes = _unitOfWork.Clientes != null,
                compras = _unitOfWork.Compras != null,
                planPagos = _unitOfWork.PlanPagos != null,
                abonos = _unitOfWork.Abonos != null,
                productos = _unitOfWork.Productos != null,
                proveedores = _unitOfWork.Proveedores != null
            };

            return Ok(new
            {
                message = "✅ Repositorios configurados correctamente",
                repositories
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = "❌ Error en repositorios",
                error = ex.Message
            });
        }
    }
}