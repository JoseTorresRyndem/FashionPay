namespace FashionPay.Application.DTOs.Compra;

public class CompraResponseDto
{
    public int IdCompra { get; set; }
    public string NumeroCompra { get; set; } = string.Empty;
    public DateTime FechaCompra { get; set; }
    public decimal MontoTotal { get; set; }
    public int CantidadPagos { get; set; }
    public decimal MontoMensual { get; set; }
    public string? Observaciones { get; set; }

    // Información del cliente
    public ClienteBasicoCompraDto Cliente { get; set; } = new();

    // Productos comprados
    public List<DetalleCompraDto> Detalles { get; set; } = new();

    // Plan de pagos
    public List<PlanPagoDto> PlanPagos { get; set; } = new();
}