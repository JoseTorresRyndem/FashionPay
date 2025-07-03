namespace FashionPay.Application.DTOs.Cliente;

public class ClienteUpdateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public int DiaPago { get; set; }
    public decimal LimiteCredito { get; set; }

    public int CantidadMaximaPagos { get; set; }
    public int ToleranciasMorosidad { get; set; }
    public bool Activo { get; set; }
}
