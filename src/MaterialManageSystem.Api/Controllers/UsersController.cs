using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaterialManageSystem.Api.DTOs;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Enums;
using MaterialManageSystem.Core.Interfaces;

namespace MaterialManageSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRoleRepository _roleRepository;

    public UsersController(
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _employeeRepository = employeeRepository;
        _roleRepository = roleRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<List<UserDto>>> GetAll()
    {
        var users = await _userRepository.GetAllWithEmployeeAsync();
        
        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            EmployeeId = u.EmployeeId,
            EmployeeName = u.Employee?.Name ?? "",
            EmployeeNo = u.Employee?.EmployeeNo ?? "",
            UserType = (int)u.UserType,
            UserTypeName = u.UserType.ToString(),
            IsActive = u.IsActive,
            LastLoginAt = u.LastLoginAt,
            CreatedAt = u.CreatedAt
        }).ToList();

        return ApiResponse<List<UserDto>>.Success(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<UserDto>> GetById(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<UserDto>.Fail(404, "用户不存在");
        }

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            EmployeeId = user.EmployeeId,
            EmployeeName = user.Employee?.Name ?? "",
            EmployeeNo = user.Employee?.EmployeeNo ?? "",
            UserType = (int)user.UserType,
            UserTypeName = user.UserType.ToString(),
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt
        };

        return ApiResponse<UserDto>.Success(dto);
    }

    [HttpPost]
    public async Task<ApiResponse<UserDto>> Create([FromBody] CreateUserRequest request)
    {
        var existing = await _userRepository.GetByUsernameAsync(request.Username);
        if (existing != null)
        {
            return ApiResponse<UserDto>.Fail(400, "用户名已存在");
        }

        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId ?? 0);
        if (employee == null)
        {
            return ApiResponse<UserDto>.Fail(400, "员工不存在");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            EmployeeId = request.EmployeeId ?? 0,
            UserType = (UserType)request.UserType,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = GetCurrentUser()
        };

        _userRepository.Add(user);
        await _userRepository.SaveChangesAsync();

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            EmployeeId = user.EmployeeId,
            EmployeeName = employee.Name,
            EmployeeNo = employee.EmployeeNo,
            UserType = (int)user.UserType,
            UserTypeName = user.UserType.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

        return ApiResponse<UserDto>.Success(dto, "用户创建成功");
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<UserDto>> Update(long id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<UserDto>.Fail(404, "用户不存在");
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        if (request.UserType.HasValue)
        {
            user.UserType = (UserType)request.UserType.Value;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        if (request.EmployeeId.HasValue)
        {
            if (request.EmployeeId.Value > 0)
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId.Value);
                if (employee == null)
                {
                    return ApiResponse<UserDto>.Fail(400, "员工不存在");
                }
                user.EmployeeId = request.EmployeeId.Value;
            }
            else
            {
                user.EmployeeId = 0;
            }
        }

        user.UpdatedAt = DateTime.Now;
        user.UpdatedBy = GetCurrentUser();
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        var updatedUser = await _userRepository.GetByIdWithEmployeeAsync(id);
        var dto = new UserDto
        {
            Id = updatedUser!.Id,
            Username = updatedUser.Username,
            EmployeeId = updatedUser.EmployeeId,
            EmployeeName = updatedUser.Employee?.Name ?? "",
            EmployeeNo = updatedUser.Employee?.EmployeeNo ?? "",
            UserType = (int)updatedUser.UserType,
            UserTypeName = updatedUser.UserType.ToString(),
            IsActive = updatedUser.IsActive,
            LastLoginAt = updatedUser.LastLoginAt,
            CreatedAt = updatedUser.CreatedAt
        };

        return ApiResponse<UserDto>.Success(dto, "用户更新成功");
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<bool>.Fail(404, "用户不存在");
        }

        if (user.UserType == UserType.SystemAdmin)
        {
            return ApiResponse<bool>.Fail(400, "不能删除系统管理员");
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.Now;
        user.UpdatedBy = GetCurrentUser();
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return ApiResponse<bool>.Success(true, "用户删除成功");
    }

    private string GetCurrentUser()
    {
        return User.Identity?.Name ?? "System";
    }
}
