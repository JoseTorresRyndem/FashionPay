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

    public async Task<Cliente> CrearClienteConEstadoCuentaAsync(
string nombre,
string email,
string? telefono,
string? direccion,
int diaPago,
decimal limiteCredito,
int cantidadMaximaPagos,
int toleranciasMorosidad)
    {
        // Parámetros para el procedimiento almacenado
        var parameters = new[]
        {
        new SqlParameter("@Nombre", nombre),
        new SqlParameter("@Email", email),
        new SqlParameter("@Telefono", telefono ?? (object)DBNull.Value),
        new SqlParameter("@Direccion", direccion ?? (object)DBNull.Value),
        new SqlParameter("@DiaPago", diaPago),
        new SqlParameter("@LimiteCredito", limiteCredito),
        new SqlParameter("@CantidadMaximaPagos", cantidadMaximaPagos),
        new SqlParameter("@ToleranciasMorosidad", toleranciasMorosidad)
    };

        // Ejecutar procedimiento y obtener resultado
        var resultado = await _context.Database.SqlQueryRaw<SpAltaClienteResult>(
            "EXEC sp_AltaCliente @Nombre, @Email, @Telefono, @Direccion, @DiaPago, @LimiteCredito, @CantidadMaximaPagos, @ToleranciasMorosidad",
            parameters).FirstOrDefaultAsync();

        if (resultado == null)
            throw new InvalidOperationException("Error al crear cliente");

        // Obtener el cliente recién creado con su estado de cuenta
        var clienteCreado = await GetByIdAsync(resultado.ClienteId);

        if (clienteCreado == null)
            throw new InvalidOperationException($"Cliente creado pero no encontrado: {resultado.ClienteId}");

        return clienteCreado;
    }

    // Clase para el resultado del procedimiento almacenado
    public class SpAltaClienteResult
    {
        public int ClienteId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}

