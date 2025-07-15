
namespace FashionPay.Application.DTOs.Cliente;

public class ClienteResponseDto
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public int DiaPago { get; set; }
    public int PeriodicidadPago { get; set; }
    public decimal LimiteCredito { get; set; }
    public decimal CreditoDisponible { get; set; }
    public int CantidadMaximaPagos { get; set; }
    public int ToleranciasMorosidad { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }
    public EstadoCuentaDto? EstadoCuenta { get; set; }
}

