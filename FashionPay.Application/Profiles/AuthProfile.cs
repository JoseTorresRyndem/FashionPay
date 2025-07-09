using AutoMapper;
using FashionPay.Application.DTOs.Auth;
using FashionPay.Core.Entities;

namespace FashionPay.Application.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)));
        
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.IdUser, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
            .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}