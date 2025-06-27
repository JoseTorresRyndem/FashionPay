using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FashionPay.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Configurar AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Aquí agregamos otros servicios de Application en el futuro
        // services.AddScoped<IClienteService, ClienteService>();

        return services;
    }
}