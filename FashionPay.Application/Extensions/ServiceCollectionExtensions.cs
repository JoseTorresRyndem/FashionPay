using FashionPay.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FashionPay.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Configurar AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Configurar FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Registrar servicios de Application
        services.AddScoped<ICompraService, CompraService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IAbonoService, AbonoService>();
        services.AddScoped<IProveedorService, ProveedorService>();


        return services;
    }
}