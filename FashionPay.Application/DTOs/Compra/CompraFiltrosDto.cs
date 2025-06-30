namespace FashionPay.Application.DTOs.Compra;

public class CompraFiltrosDto
{
    public int? ClienteId { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public decimal? MontoMinimo { get; set; }
    public decimal? MontoMaximo { get; set; }
    public string? Estado { get; set; } // PENDIENTE, PAGADO, VENCIDO
}