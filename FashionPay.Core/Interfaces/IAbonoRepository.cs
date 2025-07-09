using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IAbonoRepository : IBaseRepository<Abono>
{
    Task<IEnumerable<Abono>> GetPaymentsByClientAsync(int clientId);
    Task<IEnumerable<Abono>> GetPaymentsByDateAsync(DateTime date);
    Task<Abono> ApplyFullPaymentAsync(
        int clienteId,
        decimal montoAbono,
        string formaPago,
        string? observaciones,
        PlanPago pagoPendiente);
    Task<IEnumerable<Abono>> GetPaymentsWithFullRelationsAsync(int? clienteId = null);

}