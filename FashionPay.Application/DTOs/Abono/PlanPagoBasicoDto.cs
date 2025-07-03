namespace FashionPay.Application.DTOs.Abono;

public class PlanPagoBasicoDto
{
    public int IdPlanPago { get; set; }
    public int IdCompra { get; set; }
    public string NumeroCompra { get; set; } = string.Empty;
    public int NumeroPago { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal MontoProgramado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public string Estado { get; set; } = string.Empty;
}