using AutoMapper;
using FashionPay.Application.DTOs.Proveedor;
using FashionPay.Core.Entities;

namespace FashionPay.Application.Profiles;

public class ProveedorProfile : Profile
{
    public ProveedorProfile()
    {
        // Mapeo Entity → ResponseDto
        CreateMap<Proveedor, ProveedorResponseDto>()
            .ForMember(dest => dest.TotalProductos, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.ValorInventario, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.UltimaActualizacion, opt => opt.Ignore()); // Se calcula en el servicio

        // Mapeo CreateDto → Entity
        CreateMap<ProveedorCreateDto, Proveedor>()
            .ForMember(dest => dest.IdProveedor, opt => opt.Ignore()) // Se genera automáticamente
            .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.Now)) // Fecha actual al crear
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true)) // Por defecto activo
            .ForMember(dest => dest.Productos, opt => opt.Ignore()); // Relación navegacional

        // Mapeo UpdateDto → Entity
        CreateMap<ProveedorUpdateDto, Proveedor>()
            .ForMember(dest => dest.IdProveedor, opt => opt.Ignore()) // No se actualiza
            .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore()) // No se actualiza
            .ForMember(dest => dest.Productos, opt => opt.Ignore()); // Relación navegacional

        // Mapeo Entity → ProveedorBasicoDto
        CreateMap<Proveedor, ProveedorBasicoDto>()
            .ForMember(dest => dest.TotalProductos, opt => opt.Ignore()); // Se calcula en el servicio

    }
}