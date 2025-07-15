using AutoMapper;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Producto;
using Microsoft.Data.SqlClient;

namespace FashionPay.Application.Profiles;

public class ProductoProfile : Profile
{
    public ProductoProfile()
    {
        // Mapeo Producto → ProductoResponseDto
        CreateMap<Producto, ProductoResponseDto>()
            .ForMember(dest => dest.Proveedor, opt => opt.MapFrom(src => src.IdProveedorNavigation));

        // Mapeo ProductoCreateDto → Producto
        CreateMap<ProductoCreateDto, Producto>()
            .ForMember(dest => dest.IdProducto, opt => opt.Ignore())
            .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.IdProveedorNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.DetalleCompras, opt => opt.Ignore());

        // Mapeo ProductoUpdateDto → Producto
        CreateMap<ProductoUpdateDto, Producto>()
            .ForMember(dest => dest.IdProducto, opt => opt.Ignore())
            .ForMember(dest => dest.Codigo, opt => opt.Ignore()) // No permitir cambio de código
            .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
            .ForMember(dest => dest.IdProveedorNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.DetalleCompras, opt => opt.Ignore());

        // Mapeo Proveedor → ProveedorBasicoDto
        CreateMap<Proveedor, ProveedorBasicoDto>();
    }
}
