using AutoMapper;
using FashionPay.Core.Entities;
using FashionPay.Application.DTOs.Compra;

namespace FashionPay.Application.Profiles;

public class CompraProfile : Profile
{
    public CompraProfile()
    {
        // Mapeo Compra → CompraResponseDto
        CreateMap<Compra, CompraResponseDto>()
            .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente))
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.DetalleCompras))
            .ForMember(dest => dest.PlanPagos, opt => opt.MapFrom(src => src.PlanPagos));

        // Mapeo CompraCreateDto → Compra
        CreateMap<CompraCreateDto, Compra>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroCompra, opt => opt.Ignore()) // Se genera automáticamente
            .ForMember(dest => dest.FechaCompra, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.MontoTotal, opt => opt.Ignore()) // Se calcula
            .ForMember(dest => dest.MontoMensual, opt => opt.Ignore()) // Se calcula
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.DetalleCompras, opt => opt.Ignore()) // Se mapea separado
            .ForMember(dest => dest.PlanPagos, opt => opt.Ignore()); // Se crea separado

        // Mapeo DetalleCompraCreateDto → DetalleCompra
        CreateMap<DetalleCompraCreateDto, DetalleCompra>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CompraId, opt => opt.Ignore())
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Cantidad * src.PrecioUnitario))
            .ForMember(dest => dest.Compra, opt => opt.Ignore())
            .ForMember(dest => dest.Producto, opt => opt.Ignore());

        // Mapeo DetalleCompra → DetalleCompraDto
        CreateMap<DetalleCompra, DetalleCompraDto>()
            .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre))
            .ForMember(dest => dest.ProductoCodigo, opt => opt.MapFrom(src => src.Producto.Codigo));

        // Mapeo PlanPago → PlanPagoDto
        CreateMap<PlanPago, PlanPagoDto>()
            .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.FechaVencimiento.ToDateTime(TimeOnly.MinValue)));

        // Mapeo Cliente → ClienteBasicoDto
        CreateMap<Cliente, ClienteBasicoCompraDto>()
            .ForMember(dest => dest.Clasificacion, opt => opt.MapFrom(src =>
                src.EstadoCuenta != null ? src.EstadoCuenta.Clasificacion : "CUMPLIDO"))
            .ForMember(dest => dest.DeudaTotal, opt => opt.MapFrom(src =>
                src.EstadoCuenta != null ? src.EstadoCuenta.DeudaTotal : 0));
    }
}
