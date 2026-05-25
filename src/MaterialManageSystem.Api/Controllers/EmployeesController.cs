using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaterialManageSystem.Api.DTOs;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Interfaces;

namespace MaterialManageSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;

    public EmployeesController(IEmployeeRepository employeeRepository, IUserRepository userRepository)
    {
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<List<EmployeeDto>>> GetAll()
    {
        var employees = await _employeeRepository.GetAllWithUserAsync();
        
        var dtos = employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            EmployeeNo = e.EmployeeNo,
            Name = e.Name,
            Department = e.Department,
            Position = e.Position,
            Phone = e.Phone,
            Email = e.Email,
            UserId = e.User?.Id ?? 0,
            Username = e.User?.Username,
            HasUser = e.User != null,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt
        }).ToList();

        return ApiResponse<List<EmployeeDto>>.Success(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<EmployeeDto>> GetById(long id)
    {
        var employee = await _employeeRepository.GetByIdWithUserAsync(id);
        if (employee == null)
        {
            return ApiResponse<EmployeeDto>.Fail(404, "员工不存在");
        }

        var dto = new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            Name = employee.Name,
            Department = employee.Department,
            Position = employee.Position,
            Phone = employee.Phone,
            Email = employee.Email,
            UserId = employee.User?.Id ?? 0,
            Username = employee.User?.Username,
            HasUser = employee.User != null,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt
        };

        return ApiResponse<EmployeeDto>.Success(dto);
    }

    [HttpPost]
    public async Task<ApiResponse<EmployeeDto>> Create([FromBody] CreateEmployeeRequest request)
    {
        var existing = await _employeeRepository.GetByEmployeeNoAsync(request.EmployeeNo);
        if (existing != null)
        {
            return ApiResponse<EmployeeDto>.Fail(400, "员工编号已存在");
        }

        var employee = new Employee
        {
            EmployeeNo = request.EmployeeNo,
            Name = request.Name,
            Department = request.Department,
            Position = request.Position,
            Phone = request.Phone,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = GetCurrentUser()
        };

        _employeeRepository.Add(employee);
        await _employeeRepository.SaveChangesAsync();

        var dto = new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNo = employee.EmployeeNo,
            Name = employee.Name,
            Department = employee.Department,
            Position = employee.Position,
            Phone = employee.Phone,
            Email = employee.Email,
            UserId = 0,
            Username = null,
            HasUser = false,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt
        };

        return ApiResponse<EmployeeDto>.Success(dto, "员工创建成功");
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<EmployeeDto>> Update(long id, [FromBody] UpdateEmployeeRequest request)
    {
        var employee = await _employeeRepository.GetByIdWithUserAsync(id);
        if (employee == null)
        {
            return ApiResponse<EmployeeDto>.Fail(404, "员工不存在");
        }

        if (!string.IsNullOrEmpty(request.EmployeeNo) && request.EmployeeNo != employee.EmployeeNo)
        {
            var existing = await _employeeRepository.GetByEmployeeNoAsync(request.EmployeeNo);
            if (existing != null && existing.Id != id)
            {
                return ApiResponse<EmployeeDto>.Fail(400, "员工编号已存在");
            }
            employee.EmployeeNo = request.EmployeeNo;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            employee.Name = request.Name;
        }

        if (request.Department != null)
        {
            employee.Department = request.Department;
        }

        if (request.Position != null)
        {
            employee.Position = request.Position;
        }

        if (request.Phone != null)
        {
            employee.Phone = request.Phone;
        }

        if (request.Email != null)
        {
            employee.Email = request.Email;
        }

        if (request.IsActive.HasValue)
        {
            employee.IsActive = request.IsActive.Value;
        }

        if (request.UserId.HasValue)
        {
            if (request.UserId.Value > 0)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId.Value);
                if (user == null)
                {
                    return ApiResponse<EmployeeDto>.Fail(400, "用户不存在");
                }
                if (user.EmployeeId != employee.Id && user.EmployeeId > 0)
                {
                    return ApiResponse<EmployeeDto>.Fail(400, "该用户已关联其他员工");
                }
                user.EmployeeId = employee.Id;
                _userRepository.Update(user);
            }
            else if (employee.User != null)
            {
                var currentUser = await _userRepository.GetByIdAsync(employee.User.Id);
                if (currentUser != null)
                {
                    currentUser.EmployeeId = 0;
                    _userRepository.Update(currentUser);
                }
            }
        }

        employee.UpdatedAt = DateTime.Now;
        employee.UpdatedBy = GetCurrentUser();
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync();

        var updatedEmployee = await _employeeRepository.GetByIdWithUserAsync(id);
        var dto = new EmployeeDto
        {
            Id = updatedEmployee!.Id,
            EmployeeNo = updatedEmployee.EmployeeNo,
            Name = updatedEmployee.Name,
            Department = updatedEmployee.Department,
            Position = updatedEmployee.Position,
            Phone = updatedEmployee.Phone,
            Email = updatedEmployee.Email,
            UserId = updatedEmployee.User?.Id ?? 0,
            Username = updatedEmployee.User?.Username,
            HasUser = updatedEmployee.User != null,
            IsActive = updatedEmployee.IsActive,
            CreatedAt = updatedEmployee.CreatedAt
        };

        return ApiResponse<EmployeeDto>.Success(dto, "员工更新成功");
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        var employee = await _employeeRepository.GetByIdWithUserAsync(id);
        if (employee == null)
        {
            return ApiResponse<bool>.Fail(404, "员工不存在");
        }

        if (employee.User != null)
        {
            return ApiResponse<bool>.Fail(400, "该员工关联了用户账号，请先删除用户");
        }

        employee.IsActive = false;
        employee.UpdatedAt = DateTime.Now;
        employee.UpdatedBy = GetCurrentUser();
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync();

        return ApiResponse<bool>.Success(true, "员工删除成功");
    }

    private string GetCurrentUser()
    {
        return User.Identity?.Name ?? "System";
    }
}
