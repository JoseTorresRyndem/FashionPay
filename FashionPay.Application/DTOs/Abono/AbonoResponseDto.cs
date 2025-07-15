using FashionPay.Application.DTOs.Compra;

namespace FashionPay.Application.DTOs.Abono;

public class AbonoResponseDto
{
    public int IdAbono { get; set; }
    public string NumeroRecibo { get; set; } = string.Empty;
    public DateTime FechaAbono { get; set; }
    public decimal MontoAbono { get; set; }
    public string FormaPago { get; set; } = string.Empty;
    public string? Observaciones { get; set; }

    // Información del cliente
    public ClienteBasicoAbonoDto Cliente { get; set; } = new();

    // Información del plan de pago al que se aplicó
    public PlanPagoBasicoDto PlanPago { get; set; } = new();
}