namespace FashionPay.Application.DTOs.Compra;

public class DetalleCompraCreateDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
