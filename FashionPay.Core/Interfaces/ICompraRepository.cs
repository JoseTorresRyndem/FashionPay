using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface ICompraRepository : IBaseRepository<Compra>
{
    Task<IEnumerable<Compra>> GetComprasByClienteAsync(int clienteId);
    Task<Compra?> GetCompraWithDetallesAsync(int compraId);
    Task<IEnumerable<Compra>> GetComprasWithFiltrosAsync(
        int? clienteId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null);
}