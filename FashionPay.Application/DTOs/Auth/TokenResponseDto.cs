namespace FashionPay.Application.DTOs.Auth;

public class TokenResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}