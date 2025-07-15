namespace FashionPay.Application.DTOs.Producto;

public class ProductoBusquedaDto
{
    public string? Termino { get; set; }
    public int? IdProveedor { get; set; }
    public decimal? PrecioMinimo { get; set; }
    public decimal? PrecioMaximo { get; set; }
    public bool SoloActivos { get; set; } = true;
    public bool ConStock { get; set; } = false;
}