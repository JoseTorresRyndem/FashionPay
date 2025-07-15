using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace FashionPay.Infrastructure.Repositories;

public class CompraRepository : BaseRepository<Compra>, ICompraRepository
{
    public CompraRepository(FashionPayContext context) : base(context)
    {
    }
    public async Task<IEnumerable<Compra>> GetAllWithRelationsAsync()
    {
        return await _context.Compras
            .Include(c => c.IdClienteNavigation)
                .ThenInclude(cl => cl.EstadoCuenta)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.IdProductoNavigation)
            .Include(c => c.PlanPagos)
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }
    public async Task<Compra?> GetByIdWithRelationsAsync(int id)
    {
        return await _context.Compras
            .Include(c => c.IdClienteNavigation)
                .ThenInclude(cl => cl.EstadoCuenta)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.IdProductoNavigation)
            .Include(c => c.PlanPagos)
            .FirstOrDefaultAsync(c => c.IdCompra == id);
    }
    public async Task<IEnumerable<Compra>> GetByClientWithRelationsAsync(int clienteId)
    {
        return await _context.Compras
            .Include(c => c.IdClienteNavigation)
                .ThenInclude(cl => cl.EstadoCuenta)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.IdProductoNavigation)
            .Include(c => c.PlanPagos)
            .Where(c => c.IdCliente == clienteId)
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }
    public async Task<IEnumerable<Compra>> GetPurchasesWithFiltersAsync(
        int? clienteId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        decimal? montoMinimo = null,
        decimal? montoMaximo = null
        )
    {
        var query = _context.Compras
            .Include(c => c.IdClienteNavigation)
                .ThenInclude(cl => cl.EstadoCuenta)
            .Include(c => c.DetalleCompras)
                .ThenInclude(dc => dc.IdProductoNavigation)
            .Include(c => c.PlanPagos)
            .AsQueryable();

        // Aplicar filtros
        if (clienteId.HasValue)
            query = query.Where(c => c.IdCliente == clienteId.Value);

        if (fechaDesde.HasValue)
            query = query.Where(c => c.FechaCompra >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(c => c.FechaCompra <= fechaHasta.Value);

        if (montoMinimo.HasValue)
            query = query.Where(c => c.MontoTotal >= montoMinimo.Value);

        if (montoMaximo.HasValue)
            query = query.Where(c => c.MontoTotal <= montoMaximo.Value);

        return await query
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }

    public async Task<IEnumerable<Compra>> GetComprasWithProductAsync(int productId)
    {
        return await _context.Compras
            .Include(c => c.DetalleCompras)
            .Where(c => c.DetalleCompras.Any(dc => dc.IdProducto == productId))
            .ToListAsync();
    }
}