using AutoMapper;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Abono;

namespace FashionPay.Application.Profiles;

public class AbonoProfile : Profile
{
    public AbonoProfile()
    {
        // Mapeo Abono → AbonoResponseDto
        CreateMap<Abono, AbonoResponseDto>()
            .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente))
            .ForMember(dest => dest.PlanPago, opt => opt.MapFrom(src => src.PlanPago));

        // Mapeo AbonoCreateDto → Abono
        CreateMap<AbonoCreateDto, Abono>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PlanPagoId, opt => opt.Ignore()) // Se asigna en el servicio
            .ForMember(dest => dest.NumeroRecibo, opt => opt.Ignore()) // Se genera automáticamente
            .ForMember(dest => dest.FechaAbono, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.PlanPago, opt => opt.Ignore());

        // Mapeo Cliente → ClienteBasicoDto
        CreateMap<Cliente, ClienteBasicoAbonoDto>()
            .ForMember(dest => dest.Clasificacion, opt => opt.MapFrom(src =>
                src.EstadoCuenta != null ? src.EstadoCuenta.Clasificacion : "CUMPLIDO"))
            .ForMember(dest => dest.DeudaTotal, opt => opt.MapFrom(src =>
                src.EstadoCuenta != null ? src.EstadoCuenta.DeudaTotal : 0));

        // Mapeo PlanPago → PlanPagoBasicoDto
        CreateMap<PlanPago, PlanPagoBasicoDto>()
            .ForMember(dest => dest.NumeroCompra, opt => opt.MapFrom(src => src.Compra.NumeroCompra))
            .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.FechaVencimiento.ToDateTime(TimeOnly.MinValue)));
    }
}