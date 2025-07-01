namespace FashionPay.Application.DTOs.Proveedor;

public class ProveedorFiltrosDto
{
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public bool? Activo { get; set; }
    public DateTime? FechaRegistroDesde { get; set; }
    public DateTime? FechaRegistroHasta { get; set; }
    public bool ConProductos { get; set; } = false; // Solo proveedores con productos
}