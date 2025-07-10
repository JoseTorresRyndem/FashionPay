using AutoMapper;
using BCrypt.Net;
using FashionPay.Application.DTOs.Auth;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FashionPay.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.GetUserWithRolesByUsernameAsync(loginDto.Username);
        
        if (user == null || !user.Active)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            return null;

        var userResponse = _mapper.Map<UserResponseDto>(user);
        var token = await GenerateTokenAsync(userResponse);

        return new TokenResponseDto
        {
            Token = token,
            Expires = DateTime.UtcNow.AddHours(double.Parse(_configuration["JwtSettings:ExpirationHours"] ?? "24")),
            Username = user.Username,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name)
        };
    }

    public async Task<UserResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _unitOfWork.Users.GetByUsernameAsync(registerDto.Username);
        if (existingUser != null)
            return null;

        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(registerDto.Email);
        if (existingEmail != null)
            return null;

        var user = new User
        {
            Username = registerDto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Email = registerDto.Email,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        foreach (var roleName in registerDto.Roles)
        {
            var role = await _unitOfWork.Roles.GetByNameAsync(roleName);
            if (role != null)
            {
                await _unitOfWork.UserRoles.AddUserToRoleAsync(user.IdUser, role.IdRole);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.IdUser);
        return _mapper.Map<UserResponseDto>(userWithRoles);
    }


    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"] ?? "");
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<string> GenerateTokenAsync(UserResponseDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"] ?? "");
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(double.Parse(_configuration["JwtSettings:ExpirationHours"] ?? "24")),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
}