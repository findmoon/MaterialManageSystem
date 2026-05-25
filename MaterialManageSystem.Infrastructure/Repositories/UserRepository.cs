using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaterialManageSystem.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    public async Task<IEnumerable<User>> GetAllWithEmployeeAsync()
    {
        return await _dbSet
            .Include(u => u.Employee)
            .ToListAsync();
    }

    public async Task<User?> GetByIdWithEmployeeAsync(long id)
    {
        return await _dbSet
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
