using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IAbonoRepository : IBaseRepository<Abono>
{
    Task<IEnumerable<Abono>> GetAbonosByClienteAsync(int clienteId);
    Task<IEnumerable<Abono>> GetAbonosByFechaAsync(DateTime fecha);
    Task<decimal> GetTotalAbonosClienteAsync(int clienteId, DateTime? fechaDesde = null);
}