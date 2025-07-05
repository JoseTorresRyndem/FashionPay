using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface ICompraRepository : IBaseRepository<Compra>
{
    Task<IEnumerable<Compra>> GetAllWithRelationsAsync();
    Task<Compra?> GetByIdWithRelationsAsync(int id);
    Task<IEnumerable<Compra>> GetByClientWithRelationsAsync(int clientId);
    Task<IEnumerable<Compra>> GetPurchasesWithFiltersAsync(
        int? clientId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? minAmount = null,
        decimal? maxAmount = null
    );
}