using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<Cliente?> GetByEmailAsync(string email);
    Task<decimal> GetDeudaTotalAsync(int clienteId);
    Task<EstadoCuenta?> GetEstadoCuentaAsync(int clienteId);
    Task<IEnumerable<Cliente>> GetClientesByClasificacionAsync(string clasificacion);
    Task ExecuteCalcularSaldoAsync(int clienteId);
}