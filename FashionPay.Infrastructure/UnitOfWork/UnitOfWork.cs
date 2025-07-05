using FashionPay.Core.Data;
using FashionPay.Core.Interfaces;
using FashionPay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FashionPay.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly FashionPayContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy loading de repositorios
    private IClienteRepository? _clientes;
    private ICompraRepository? _compras;
    private IPlanPagoRepository? _planPagos;
    private IAbonoRepository? _abonos;
    private IProductoRepository? _productos;
    private IProveedorRepository? _proveedores;

    public UnitOfWork(FashionPayContext context)
    {
        _context = context;
    }
    public DbContext Context => _context;


    // Propiedades que crean repositorios bajo demanda

    public IClienteRepository Clientes
    {
        get { return _clientes ??= new ClienteRepository(_context); }
    }

    public ICompraRepository Compras
    {
        get { return _compras ??= new CompraRepository(_context); }
    }

    public IPlanPagoRepository PlanPagos
    {
        get { return _planPagos ??= new PlanPagoRepository(_context); }
    }

    public IAbonoRepository Abonos
    {
        get { return _abonos ??= new AbonoRepository(_context); }
    }

    public IProductoRepository Productos
    {
        get { return _productos ??= new ProductoRepository(_context); }
    }

    public IProveedorRepository Proveedores
    {
        get { return _proveedores ??= new ProveedorRepository(_context); }
    }

    // Métodos de transacción
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    // Implementar IDisposable
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}