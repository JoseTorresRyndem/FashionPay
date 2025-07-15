
namespace FashionPay.Application.DTOs.Abono;

public class ClienteBasicoAbonoDto
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = string.Empty;
    public decimal DeudaTotal { get; set; }
}

