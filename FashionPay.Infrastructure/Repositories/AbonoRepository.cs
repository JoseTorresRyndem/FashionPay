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
            .Include(a => a.Cliente)
                .ThenInclude(c => c.EstadoCuenta)
            .Include(a => a.PlanPago)
                .ThenInclude(pp => pp.Compra)
                .OrderByDescending(a => a.FechaAbono)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Abono>> GetAbonosByClienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(a => a.Cliente)
                .ThenInclude(c => c.EstadoCuenta)
            .Include(a => a.PlanPago)
                .ThenInclude(pp => pp.Compra)
            .Where(a => a.IdCliente == clienteId)
            .OrderByDescending(a => a.FechaAbono)
            .ToListAsync();
    }
    public async Task<IEnumerable<Abono>> GetAbonosWithFullRelationsAsync(int? clienteId = null)
    {
        var query = _dbSet
            .Include(a => a.Cliente)
                .ThenInclude(c => c.EstadoCuenta)
            .Include(a => a.PlanPago)
                .ThenInclude(pp => pp.Compra)
            .AsQueryable();

        if (clienteId.HasValue)
        {
            query = query.Where(a => a.IdCliente == clienteId.Value);
        }

        return await query
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
    public async Task<Abono> AplicarAbonoCompletoAsync(
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
            EXEC sp_AplicarAbonoCorregido 
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

            var abono = await _dbSet
                .Include(a => a.Cliente)
                    .ThenInclude(c => c.EstadoCuenta)
                .Include(a => a.PlanPago)
                    .ThenInclude(pp => pp.Compra)
                .FirstOrDefaultAsync(a => a.Id == abonoId);

            if (abono == null)
            {
                throw new InvalidOperationException($"No se pudo recuperar el abono creado con ID {abonoId}");
            }

            return abono;
        }
        catch (SqlException sqlEx)
        {
            throw new Exception($"Error al registrar abono: {sqlEx.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al registrar abono: {ex.Message}");
        }
    }
}