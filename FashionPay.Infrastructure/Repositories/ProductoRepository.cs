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

    public async Task<IEnumerable<Producto>> GetProductosActivosAsync()
    {
        return await _dbSet
            .Include(p => p.Proveedor)
            .Where(p => p.Activo == true)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Producto>> GetProductosByProveedorAsync(int proveedorId)
    {
        return await _dbSet
            .Include(p => p.Proveedor)
            .Where(p => p.ProveedorId == proveedorId && p.Activo == true)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<Producto?> GetByCodigoAsync(string codigo)
    {
        return await _dbSet
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<IEnumerable<Producto>> BuscarProductosAsync(string termino)
    {
        return await _dbSet
            .Include(p => p.Proveedor)
            .Where(p => p.Activo == true &&
                       (p.Nombre.Contains(termino) ||
                        p.Codigo.Contains(termino) ||
                        p.Descripcion!.Contains(termino)))
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }
}