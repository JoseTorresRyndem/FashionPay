namespace FashionPay.Application.DTOs.Proveedor;

public class ProveedorResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    // Información adicional
    public int TotalProductos { get; set; }
    public decimal ValorInventario { get; set; }
    public DateTime? UltimaActualizacion { get; set; }
}