using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IProveedorRepository : IBaseRepository<Proveedor>
{
    Task<IEnumerable<Proveedor>> GetProveedoresActivosAsync();
    Task<Proveedor?> GetProveedorWithProductosAsync(int proveedorId);
}
