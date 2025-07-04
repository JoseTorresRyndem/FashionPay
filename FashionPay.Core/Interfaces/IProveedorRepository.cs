using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IProveedorRepository : IBaseRepository<Proveedor>
{
    Task<IEnumerable<Proveedor>> GetProviderActiveAsync();
    Task<Proveedor?> GetProviderWithProductsAsync(int proveedorId);
}
