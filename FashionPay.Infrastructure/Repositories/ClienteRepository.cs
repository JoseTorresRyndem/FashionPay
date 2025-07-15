using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using Microsoft.Data.SqlClient;

using Microsoft.EntityFrameworkCore;

namespace FashionPay.Infrastructure.Repositories;
public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(FashionPayContext context) : base(context)
    {
    }
    public async Task<Cliente?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }
    public async Task<decimal> GetTotalDebtAsync(int clienteId)
    {
        var estadoCuenta = await _context.EstadoCuenta
            .FirstOrDefaultAsync(ec => ec.IdCliente == clienteId);

        if (estadoCuenta != null)
        {
            return estadoCuenta.DeudaTotal;
        }

        // Calcular si no existe estado de cuenta
        return await _context.PlanPagos
            .Where(pp => pp.IdCompraNavigation.IdCliente == clienteId && pp.SaldoPendiente > 0)
            .SumAsync(pp => pp.SaldoPendiente);
    }
    public async Task<EstadoCuenta?> GetAccountStatusAsync(int clienteId)
    {
        return await _context.EstadoCuenta
            .FirstOrDefaultAsync(ec => ec.IdCliente == clienteId);
    }

    public async Task<IEnumerable<Cliente>> GetClientsByClassificationAsync(string clasificacion)
    {
        return await _dbSet
            .Include(c => c.EstadoCuenta)
            .Where(c => c.EstadoCuenta != null && c.EstadoCuenta.Clasificacion == clasificacion)
            .ToListAsync();
    }

    public async Task ExecuteCalculateBalanceAsync(int clienteId)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_CalcularSaldoCliente @p0", clienteId);
    }

    public override async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.EstadoCuenta)
            .Include(c => c.Compras)
            .FirstOrDefaultAsync(c => c.IdCliente == id);
    }

    public override async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.EstadoCuenta)
            .ToListAsync();
    }
    public async Task<Cliente?> GetByIdWithAccountAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.EstadoCuenta)
            .FirstOrDefaultAsync(c => c.IdCliente == id);
    }

    public class SpAddClientResult
    {
        public int IdCliente { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}

