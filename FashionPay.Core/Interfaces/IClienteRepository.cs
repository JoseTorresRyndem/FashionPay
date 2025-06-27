using FashionPay.Core.Entities;
namespace FashionPay.Core.Interfaces;

public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<Cliente?> GetByEmailAsync(string email);
    Task<decimal> GetDeudaTotalAsync(int clienteId);
    Task<EstadoCuenta?> GetEstadoCuentaAsync(int clienteId);
    Task<IEnumerable<Cliente>> GetClientesByClasificacionAsync(string clasificacion);
    Task ExecuteCalcularSaldoAsync(int clienteId);
    Task<Cliente> CrearClienteConEstadoCuentaAsync(
        string nombre,
        string email,
        string? telefono,
        string? direccion,
        int diaPago,
        decimal limiteCredito,
        int cantidadMaximaPagos,
        int toleranciasMorosidad);
}