using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;

namespace FashionPay.Infrastructure.Repositories;

public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
{
    public ProductoRepository(FashionPayContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Producto>> GetProductsActiveAsync()
    {
        return await _dbSet
            .Include(p => p.IdProveedorNavigation)
            .Where(p => p.Activo == true)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Producto>> GetProductsByProviderAsync(int proveedorId)
    {
        return await _dbSet
            .Include(p => p.IdProveedorNavigation)
            .Where(p => p.IdProveedor == proveedorId && p.Activo == true)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<Producto?> GetByCodeAsync(string codigo)
    {
        return await _dbSet
            .Include(p => p.IdProveedorNavigation)
            .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }
}