using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IProductoRepository : IBaseRepository<Producto>
{
    Task<IEnumerable<Producto>> GetProductosActivosAsync();
    Task<IEnumerable<Producto>> GetProductosByProveedorAsync(int proveedorId);
    Task<Producto?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Producto>> BuscarProductosAsync(string termino);
}
