using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;

namespace FashionPay.Infrastructure.Repositories;

public class PlanPagoRepository : BaseRepository<PlanPago>, IPlanPagoRepository
{
    public PlanPagoRepository(FashionPayContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PlanPago>> GetPagosVencidosAsync()
    {
        return await _dbSet
            .Include(pp => pp.Compra)
                .ThenInclude(c => c.Cliente)
            .Where(pp => pp.Estado == "VENCIDO" && pp.SaldoPendiente > 0)
            .OrderBy(pp => pp.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanPago>> GetPagosByClienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(pp => pp.Compra)
            .Where(pp => pp.Compra.IdCliente == clienteId)
            .OrderBy(pp => pp.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlanPago>> GetPagosByCompraAsync(int compraId)
    {
        return await _dbSet
            .Where(pp => pp.IdCompra == compraId)
            .OrderBy(pp => pp.NumeroPago)
            .ToListAsync();
    }

    public async Task<PlanPago?> GetProximoPagoPendienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(pp => pp.Compra)
            .Where(pp => pp.Compra.IdCliente == clienteId &&
                        pp.Estado == "PENDIENTE" &&
                        pp.SaldoPendiente > 0)
            .OrderBy(pp => pp.FechaVencimiento)
            .FirstOrDefaultAsync();
    }
}