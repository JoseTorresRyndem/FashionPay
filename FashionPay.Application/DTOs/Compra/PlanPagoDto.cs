namespace FashionPay.Application.DTOs.Compra;

public class PlanPagoDto
{
    public int IdPlanPago { get; set; }
    public int NumeroPago { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal MontoProgramado { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int DiasVencidos { get; set; }
}