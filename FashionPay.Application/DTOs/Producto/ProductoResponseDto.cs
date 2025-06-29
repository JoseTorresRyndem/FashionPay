namespace FashionPay.Application.DTOs.Producto;

public class ProductoResponseDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    public ProveedorBasicoDto Proveedor { get; set; } = new();
}