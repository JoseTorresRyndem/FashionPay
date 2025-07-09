namespace FashionPay.Application.DTOs.Auth;

public class RegisterDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}