
namespace FashionPay.Application.DTOs.Abono;

public class AbonoFiltrosDto
{
    public int? ClienteId { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public string? FormaPago { get; set; }
    public decimal? MontoMinimo { get; set; }
    public decimal? MontoMaximo { get; set; }
}

