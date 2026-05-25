using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Entities;

namespace MaterialManageSystem.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllWithEmployeeAsync();
    Task<User?> GetByIdWithEmployeeAsync(long id);
}
