using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using FashionPay.Infrastructure.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FashionPay.Infrastructure.Repositories;

public class AbonoRepository : BaseRepository<Abono>, IAbonoRepository
{
    public AbonoRepository(FashionPayContext context) : base(context)
    {
    }
    public override async Task<Abono?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(a => a.IdClienteNavigation)
                .ThenInclude(c => c.EstadoCuenta)
            .Include(a => a.IdPlanPagoNavigation)
                .ThenInclude(pp => pp.IdCompraNavigation)
                .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAbono == id);
    }

    public async Task<IEnumerable<Abono>> GetPaymentsByClientAsync(int clienteId)
    {
        return await _dbSet
            .Include(a => a.IdClienteNavigation)
                .ThenInclude(c => c.EstadoCuenta)
            .Include(a => a.IdPlanPagoNavigation)
                .ThenInclude(pp => pp.IdCompraNavigation)
            .Where(a => a.IdCliente == clienteId)
            .AsNoTracking()
            .OrderByDescending(a => a.FechaAbono)
            .ToListAsync();
    }
    public async Task<IEnumerable<Abono>> GetPaymentsWithFullRelationsAsync(int? clienteId = null)
    {
        var query = _dbSet
            .Include(a => a.IdClienteNavigation)
                .ThenInclude(c => c.EstadoCuenta)
            .Include(a => a.IdPlanPagoNavigation)
                .ThenInclude(pp => pp.IdCompraNavigation)
            .AsQueryable();

        if (clienteId.HasValue)
        {
            query = query.Where(a => a.IdCliente == clienteId.Value);
        }

        return await query
            .AsNoTracking()
            .OrderByDescending(a => a.FechaAbono)
            .ToListAsync();
    }
    public async Task<IEnumerable<Abono>> GetPaymentsByDateAsync(DateTime fecha)
    {
        var fechaInicio = fecha.Date;
        var fechaFin = fechaInicio.AddDays(1);

        return await _dbSet
            .Include(a => a.IdClienteNavigation)
            .Where(a => a.FechaAbono >= fechaInicio && a.FechaAbono < fechaFin)
            .AsNoTracking()
            .OrderBy(a => a.FechaAbono)
            .ToListAsync();
    }
    public async Task<Abono> ApplyFullPaymentAsync(
     int clienteId,
     decimal montoAbono,
     string formaPago,
     string? observaciones,
     PlanPago pagoPendiente)
    {
        try
        {
            var abonoIdParameter = new SqlParameter
            {
                ParameterName = "@AbonoId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(@"
            EXEC sp_AplicarAbono
                @ClienteId = {0}, 
                @MontoAbono = {1}, 
                @FormaPago = {2}, 
                @Observaciones = {3}, 
                @AbonoId = @AbonoId OUTPUT",
                clienteId,
                montoAbono,
                formaPago,
                observaciones ?? (object)DBNull.Value,
                abonoIdParameter);

            // Obtener el ID del abono creado
            var abonoId = (int)abonoIdParameter.Value;

            await VerifyAndReleaseCreditAsync(clienteId);

            var abono = await _dbSet
                .Include(a => a.IdClienteNavigation)
                    .ThenInclude(c => c.EstadoCuenta)
                .Include(a => a.IdPlanPagoNavigation)
                    .ThenInclude(pp => pp.IdCompraNavigation)
                    .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAbono == abonoId);

            if (abono == null)
            {
                throw new InvalidOperationException($"No se pudo recuperar el abono creado con ID {abonoId}");
            }

            return abono;
        }
        catch (SqlException sqlEx)
        {
            throw new Exception($"Error SQL al registrar abono: {sqlEx.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al registrar abono: {ex.Message}");
        }
    }
    private async Task VerifyAndReleaseCreditAsync(int clienteId)
    {
        try
        {
            // Buscar compras ACTIVAS del cliente que estén completamente pagadas
            var comprasParaLiberar = await _context.Compras
                .Include(c => c.PlanPagos)
                .Where(c => c.IdCliente == clienteId && c.EstadoCompra == "ACTIVA")
                .Where(c => c.PlanPagos.All(p => p.SaldoPendiente <= 0))
                .ToListAsync();

            foreach (var compra in comprasParaLiberar)
            {
                // Marcar compra como PAGADA
                compra.EstadoCompra = "PAGADA";

                // Liberar crédito del cliente
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(cl => cl.IdCliente == clienteId);

                if (cliente != null)
                {
                    cliente.CreditoDisponible += compra.MontoTotal;
                    _context.Clientes.Update(cliente);
                }

                _context.Compras.Update(compra);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al liberar crédito: {ex.Message}");
        }
    }
}