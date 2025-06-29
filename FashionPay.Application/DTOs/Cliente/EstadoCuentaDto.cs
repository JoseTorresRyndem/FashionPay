namespace FashionPay.Application.DTOs.Cliente;

public class EstadoCuentaDto
{
    public string Clasificacion { get; set; } = string.Empty;
    public decimal DeudaTotal { get; set; }
    public DateTime? FechaUltimoPago { get; set; }
    public int CantidadPagosVencidos { get; set; }
    public int DiasMaximoVencimiento { get; set; }
    public DateTime FechaActualizacion { get; set; }
}