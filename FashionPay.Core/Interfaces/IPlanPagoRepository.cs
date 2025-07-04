using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IPlanPagoRepository : IBaseRepository<PlanPago>
{
    Task<IEnumerable<PlanPago>> GetOverduePaymentsAsync();
    Task<IEnumerable<PlanPago>> GetPaymentsByClientAsync(int clientId);
    Task<IEnumerable<PlanPago>> GetPaymentsByPurchaseAsync(int purchaseId);
    Task<PlanPago?> GetNextPendingPaymentAsync(int clientId);
}