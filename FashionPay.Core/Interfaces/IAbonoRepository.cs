using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IAbonoRepository : IBaseRepository<Abono>
{
    Task<IEnumerable<Abono>> GetAbonosByClienteAsync(int clienteId);
    Task<IEnumerable<Abono>> GetAbonosByFechaAsync(DateTime fecha);
    Task<Abono> AplicarAbonoCompletoAsync(
        int clienteId,
        decimal montoAbono,
        string formaPago,
        string? observaciones,
        PlanPago pagoPendiente);
    Task<IEnumerable<Abono>> GetAbonosWithFullRelationsAsync(int? clienteId = null);

}