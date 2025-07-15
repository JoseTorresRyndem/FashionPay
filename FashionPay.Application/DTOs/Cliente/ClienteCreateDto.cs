namespace FashionPay.Application.DTOs.Cliente;

public class ClienteCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public int DiaPago { get; set; }
    public decimal LimiteCredito { get; set; }
    public int CantidadMaximaPagos { get; set; } = 12;
    public int ToleranciasMorosidad { get; set; } = 0;
}
