using AutoMapper;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Cliente;

namespace FashionPay.Application.Profiles;
public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        // Mapeo Cliente → ClienteResponseDto
        CreateMap<Cliente, ClienteResponseDto>()
            .ForMember(dest => dest.EstadoCuenta, opt => opt.MapFrom(src => src.EstadoCuenta));

        // Mapeo ClienteCreateDto → Cliente  
        CreateMap<ClienteCreateDto, Cliente>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PeriodicidadPago, opt => opt.MapFrom(src => 30)) // Mensual por defecto
            .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.EstadoCuenta, opt => opt.Ignore())
            .ForMember(dest => dest.Compras, opt => opt.Ignore());

        // Mapeo ClienteUpdateDto → Cliente
        CreateMap<ClienteUpdateDto, Cliente>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore()) // No permitir cambio de email
            .ForMember(dest => dest.PeriodicidadPago, opt => opt.Ignore())
            .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
            .ForMember(dest => dest.EstadoCuenta, opt => opt.Ignore())
            .ForMember(dest => dest.Compras, opt => opt.Ignore());

        // Mapeo EstadoCuenta → EstadoCuentaDto
        CreateMap<EstadoCuenta, EstadoCuentaDto>();
    }
}