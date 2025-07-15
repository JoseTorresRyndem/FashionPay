using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IProductoRepository : IBaseRepository<Producto>
{
    Task<IEnumerable<Producto>> GetProductsActiveAsync();
    Task<IEnumerable<Producto>> GetProductsByProviderAsync(int proveedorId);
    Task<Producto?> GetByCodeAsync(string codigo);
}
