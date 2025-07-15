namespace FashionPay.Application.DTOs.Compra;

public class CompraFiltrosDto
{
    public int? IdCliente { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public decimal? MontoMinimo { get; set; }
    public decimal? MontoMaximo { get; set; }
}