

namespace FashionPay.Application.DTOs.Abono;

public class ResumenPagosClienteDto
{
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public decimal TotalAbonos { get; set; }
    public int CantidadAbonos { get; set; }
    public DateTime? UltimoAbono { get; set; }
    public decimal DeudaActual { get; set; }
    public int PagosPendientes { get; set; }
    public int PagosVencidos { get; set; }
}

