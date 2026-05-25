using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Api.DTOs;

namespace MaterialManageSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RolesController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<List<RoleDto>>> GetAll()
    {
        var roles = await _roleRepository.GetAllAsync();
        var dtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsActive = r.IsActive
        }).ToList();

        return ApiResponse<List<RoleDto>>.Success(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<RoleDto>> GetById(long id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            return ApiResponse<RoleDto>.Fail(404, "角色不存在");
        }

        return ApiResponse<RoleDto>.Success(new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive
        });
    }

    [HttpPost]
    public async Task<ApiResponse<RoleDto>> Create([FromBody] CreateRoleRequest request)
    {
        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            CreatedBy = GetCurrentUser()
        };

        var result = await _roleRepository.AddAsync(role);

        return ApiResponse<RoleDto>.Success(new RoleDto
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.Description,
            IsActive = result.IsActive
        }, "创建成功");
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<RoleDto>> Update(long id, [FromBody] UpdateRoleRequest request)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            return ApiResponse<RoleDto>.Fail(404, "角色不存在");
        }

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsActive = request.IsActive;
        role.UpdatedBy = GetCurrentUser();

        await _roleRepository.UpdateAsync(role);

        return ApiResponse<RoleDto>.Success(new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive
        }, "更新成功");
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _roleRepository.DeleteAsync(id);
        return ApiResponse<bool>.Success(true, "删除成功");
    }

    private string GetCurrentUser()
    {
        return User.Identity?.Name ?? "System";
    }
}

public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}