namespace FashionPay.Application.DTOs.Auth;

public class UserResponseDto
{
    public int IdUser { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}