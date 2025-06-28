using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
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
        // services.AddScoped<IClienteService, ClienteService>();

        return services;
    }
}