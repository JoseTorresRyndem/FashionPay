using FashionPay.Application.DTOs.Auth;

namespace FashionPay.Application.Services;

public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserResponseDto?> RegisterAsync(RegisterDto registerDto);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> GenerateTokenAsync(UserResponseDto user);
}