namespace FashionPay.Application.DTOs.Proveedor;

public class ProveedorBasicoDto
{
    public int IdProveedor { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; }
    public int TotalProductos { get; set; }
}