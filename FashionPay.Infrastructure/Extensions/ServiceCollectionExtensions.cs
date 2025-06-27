using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FashionPay.Core.Data;
using FashionPay.Core.Interfaces;
using FashionPay.Infrastructure.Repositories;

namespace FashionPay.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Registrar DbContext con la cadena de conexión
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddFashionPayContext(connectionString!);

        // 2. Registrar repositorios como Scoped
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<IPlanPagoRepository, PlanPagoRepository>();
        services.AddScoped<IAbonoRepository, AbonoRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();

        // 3. Registrar Unit of Work como Scoped
        services.AddScoped<IUnitOfWork, FashionPay.Infrastructure.UnitOfWork.UnitOfWork>();

        return services;
    }
}
