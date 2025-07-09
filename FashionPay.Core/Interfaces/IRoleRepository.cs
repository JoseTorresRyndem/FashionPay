using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
}