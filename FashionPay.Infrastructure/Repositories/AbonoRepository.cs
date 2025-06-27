using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;

namespace FashionPay.Infrastructure.Repositories;

public class AbonoRepository : BaseRepository<Abono>, IAbonoRepository
{
    public AbonoRepository(FashionPayContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Abono>> GetAbonosByClienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(a => a.PlanPago)
                .ThenInclude(pp => pp.Compra)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.FechaAbono)
            .ToListAsync();
    }

    public async Task<IEnumerable<Abono>> GetAbonosByFechaAsync(DateTime fecha)
    {
        var fechaInicio = fecha.Date;
        var fechaFin = fechaInicio.AddDays(1);

        return await _dbSet
            .Include(a => a.Cliente)
            .Where(a => a.FechaAbono >= fechaInicio && a.FechaAbono < fechaFin)
            .OrderBy(a => a.FechaAbono)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalAbonosClienteAsync(int clienteId, DateTime? fechaDesde = null)
    {
        var query = _dbSet.Where(a => a.ClienteId == clienteId);

        if (fechaDesde.HasValue)
        {
            query = query.Where(a => a.FechaAbono >= fechaDesde.Value);
        }

        return await query.SumAsync(a => a.MontoAbono);
    }
}