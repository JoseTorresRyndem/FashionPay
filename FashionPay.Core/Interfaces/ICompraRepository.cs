using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface ICompraRepository : IBaseRepository<Compra>
{
    Task<IEnumerable<Compra>> GetAllWithRelationsAsync();
    Task<Compra?> GetByIdWithRelationsAsync(int id);
    Task<IEnumerable<Compra>> GetByClienteWithRelationsAsync(int clienteId);
    Task<IEnumerable<Compra>> GetComprasWithFiltrosAsync(
        int? clienteId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        decimal? montoMinimo = null,
        decimal? montoMaximo = null
        );
    Task<Compra> CrearCompraAsync(
        int clienteId,
        int cantidadPagos,
        string? observaciones,
        List<(int ProductoId, int Cantidad, decimal PrecioUnitario)> detalles);
}