using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaterialManageSystem.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByEmployeeNoAsync(string employeeNo)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo && e.IsActive);
    }

    public async Task<IEnumerable<Employee>> GetAllWithUserAsync()
    {
        return await _dbSet
            .Include(e => e.User)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdWithUserAsync(long id)
    {
        return await _dbSet
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
