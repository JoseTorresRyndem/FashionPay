using FashionPay.Application.DTOs.Auth;
using FashionPay.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionPay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        
        if (result == null)
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        
        if (result == null)
        {
            return BadRequest(new { message = "No se pudo crear el usuario. Verifique que el nombre de usuario y email no estén en uso." });
        }

        return Ok(result);
    }

    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        var isValid = await _authService.ValidateTokenAsync(token);
        return Ok(new { isValid });
    }
}