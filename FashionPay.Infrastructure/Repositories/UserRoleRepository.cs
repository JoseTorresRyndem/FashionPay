using FashionPay.Core.Data;
using FashionPay.Core.Entities;
using FashionPay.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FashionPay.Infrastructure.Repositories;

public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(FashionPayContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(ur => ur.Role)
            .Where(ur => ur.IdUser == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId)
    {
        return await _dbSet
            .Include(ur => ur.User)
            .Where(ur => ur.IdRole == roleId)
            .ToListAsync();
    }

    public async Task<UserRole?> GetUserRoleAsync(int userId, int roleId)
    {
        return await _dbSet
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.IdUser == userId && ur.IdRole == roleId);
    }

    public async Task AddUserToRoleAsync(int userId, int roleId)
    {
        var userRole = new UserRole
        {
            IdUser = userId,
            IdRole = roleId
        };
        await AddAsync(userRole);
    }

    public async Task RemoveUserFromRoleAsync(int userId, int roleId)
    {
        var userRole = await GetUserRoleAsync(userId, roleId);
        if (userRole != null)
        {
            await DeleteAsync(userRole);
        }
    }
}