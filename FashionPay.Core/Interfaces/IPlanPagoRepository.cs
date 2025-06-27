using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IPlanPagoRepository : IBaseRepository<PlanPago>
{
    Task<IEnumerable<PlanPago>> GetPagosVencidosAsync();
    Task<IEnumerable<PlanPago>> GetPagosByClienteAsync(int clienteId);
    Task<IEnumerable<PlanPago>> GetPagosByCompraAsync(int compraId);
    Task<PlanPago?> GetProximoPagoPendienteAsync(int clienteId);
}