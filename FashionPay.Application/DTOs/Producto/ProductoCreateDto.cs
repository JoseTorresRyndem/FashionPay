namespace FashionPay.Application.DTOs.Producto;

public class ProductoCreateDto
{
    public int IdProveedor { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; } = 0;
}