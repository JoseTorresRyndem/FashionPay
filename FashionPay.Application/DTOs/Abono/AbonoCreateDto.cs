namespace FashionPay.Application.DTOs.Abono;

public class AbonoCreateDto
{
    public int ClienteId { get; set; }
    public decimal MontoAbono { get; set; }
    public string FormaPago { get; set; } = "EFECTIVO";
    public string? Observaciones { get; set; }
}
