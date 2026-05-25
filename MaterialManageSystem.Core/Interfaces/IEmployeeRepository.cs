using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Entities;

namespace MaterialManageSystem.Core.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmployeeNoAsync(string employeeNo);
    Task<IEnumerable<Employee>> GetAllWithUserAsync();
    Task<Employee?> GetByIdWithUserAsync(long id);
}
