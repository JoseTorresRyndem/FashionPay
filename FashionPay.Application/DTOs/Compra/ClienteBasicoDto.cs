namespace FashionPay.Application.DTOs.Compra;

public class ClienteBasicoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal LimiteCredito { get; set; }
    public string Clasificacion { get; set; } = string.Empty;
    public decimal DeudaTotal { get; set; }
}
