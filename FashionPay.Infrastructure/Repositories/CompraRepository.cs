using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;

namespace FashionPay.Infrastructure.Repositories;

public class CompraRepository : BaseRepository<Compra>, ICompraRepository
{
    public CompraRepository(FashionPayContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Compra>> GetComprasByClienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .Include(c => c.DetalleCompras)
            .Include(c => c.PlanPagos)
            .Where(c => c.ClienteId == clienteId)
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }

    public async Task<Compra?> GetCompraWithDetallesAsync(int compraId)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.Producto)
            .Include(c => c.PlanPagos)
            .FirstOrDefaultAsync(c => c.Id == compraId);
    }

    public async Task<IEnumerable<Compra>> GetComprasWithFiltrosAsync(
        int? clienteId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null)
    {
        var query = _dbSet
            .Include(c => c.Cliente)
            .Include(c => c.DetalleCompras)
            .AsQueryable();

        if (clienteId.HasValue)
        {
            query = query.Where(c => c.ClienteId == clienteId.Value);
        }

        if (fechaDesde.HasValue)
        {
            query = query.Where(c => c.FechaCompra >= fechaDesde.Value);
        }

        if (fechaHasta.HasValue)
        {
            query = query.Where(c => c.FechaCompra <= fechaHasta.Value);
        }

        return await query
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }

    public override async Task<Compra?> GetByIdAsync(int id)
    {
        return await GetCompraWithDetallesAsync(id);
    }
}