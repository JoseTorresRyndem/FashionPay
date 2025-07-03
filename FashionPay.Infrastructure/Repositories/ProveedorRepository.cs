using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;

namespace FashionPay.Infrastructure.Repositories;

public class ProveedorRepository : BaseRepository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(FashionPayContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Proveedor>> GetProveedoresActivosAsync()
    {
        return await _dbSet
            .Where(p => p.Activo == true)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<Proveedor?> GetProveedorWithProductosAsync(int proveedorId)
    {
        return await _dbSet
            .Include(p => p.Productos.Where(prod => prod.Activo == true))
            .FirstOrDefaultAsync(p => p.IdProveedor == proveedorId);
    }
}