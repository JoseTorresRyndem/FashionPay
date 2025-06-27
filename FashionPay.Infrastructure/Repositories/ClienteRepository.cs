using Microsoft.EntityFrameworkCore;
using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;

namespace FashionPay.Infrastructure.Repositories;
public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(FashionPayContext context) : base(context)
    {
    }
    public async Task<Cliente?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email == email);
    }
    public async Task<decimal> GetDeudaTotalAsync(int clienteId)
    {
        // Obtener de EstadoCuenta o calcular desde PlanPagos
        var estadoCuenta = await _context.EstadoCuenta
            .FirstOrDefaultAsync(ec => ec.ClienteId == clienteId);

        if (estadoCuenta != null)
        {
            return estadoCuenta.DeudaTotal;
        }

        // Calcular si no existe estado de cuenta
        return await _context.PlanPagos
            .Where(pp => pp.Compra.ClienteId == clienteId && pp.SaldoPendiente > 0)
            .SumAsync(pp => pp.SaldoPendiente);
    }
    public async Task<EstadoCuenta?> GetEstadoCuentaAsync(int clienteId)
    {
        return await _context.EstadoCuenta
            .FirstOrDefaultAsync(ec => ec.ClienteId == clienteId);
    }

    public async Task<IEnumerable<Cliente>> GetClientesByClasificacionAsync(string clasificacion)
    {
        return await _dbSet
            .Include(c => c.EstadoCuenta)
            .Where(c => c.EstadoCuenta != null && c.EstadoCuenta.Clasificacion == clasificacion)
            .ToListAsync();
    }

    public async Task ExecuteCalcularSaldoAsync(int clienteId)
    {
        // Ejecutar procedimiento almacenado
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_CalcularSaldoCliente @p0", clienteId);
    }

    // Sobrescribir métodos base para incluir navegación
    public override async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.EstadoCuenta)
            .Include(c => c.Compras)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.EstadoCuenta)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

}

