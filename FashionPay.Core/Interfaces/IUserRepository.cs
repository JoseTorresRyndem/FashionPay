using FashionPay.Core.Entities;

namespace FashionPay.Core.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUserWithRolesAsync(int userId);
    Task<User?> GetUserWithRolesByUsernameAsync(string username);
    Task<bool> ValidateCredentialsAsync(string username, string password);
}