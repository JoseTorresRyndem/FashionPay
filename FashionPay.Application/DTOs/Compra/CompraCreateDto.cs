namespace FashionPay.Application.DTOs.Compra;

public class CompraCreateDto
{
    public int ClienteId { get; set; }
    public int CantidadPagos { get; set; }
    public string? Observaciones { get; set; }
    public List<DetalleCompraCreateDto> Detalles { get; set; } = new();
}