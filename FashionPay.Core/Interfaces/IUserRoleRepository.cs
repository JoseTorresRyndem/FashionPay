using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IUserRoleRepository : IBaseRepository<UserRole>
{
    Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId);
    Task<UserRole?> GetUserRoleAsync(int userId, int roleId);
    Task AddUserToRoleAsync(int userId, int roleId);
    Task RemoveUserFromRoleAsync(int userId, int roleId);
}