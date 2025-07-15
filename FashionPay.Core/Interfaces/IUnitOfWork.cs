using Microsoft.EntityFrameworkCore;

namespace FashionPay.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IClienteRepository Clientes { get; }
    ICompraRepository Compras { get; }
    IPlanPagoRepository PlanPagos { get; }
    IAbonoRepository Abonos { get; }
    IProductoRepository Productos { get; }
    IProveedorRepository Proveedores { get; }
    IEstadoCuentaRepository EstadoCuentas { get; }
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserRoleRepository UserRoles { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    DbContext Context { get; }
}