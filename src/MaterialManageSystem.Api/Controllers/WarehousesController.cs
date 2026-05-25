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
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehousesController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<List<WarehouseDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (items, total) = await _warehouseRepository.GetPagedAsync(page, pageSize);

        var dtos = items.Select(w => new WarehouseDto
        {
            Id = w.Id,
            Code = w.Code,
            Name = w.Name,
            Location = w.Location,
            Remark = w.Remark,
            IsActive = w.IsActive
        }).ToList();

        return ApiResponse<List<WarehouseDto>>.Success(dtos, total, page, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<WarehouseDto>> GetById(long id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null)
        {
            return ApiResponse<WarehouseDto>.Fail(404, "库房不存在");
        }

        return ApiResponse<WarehouseDto>.Success(new WarehouseDto
        {
            Id = warehouse.Id,
            Code = warehouse.Code,
            Name = warehouse.Name,
            Location = warehouse.Location,
            Remark = warehouse.Remark,
            IsActive = warehouse.IsActive
        });
    }

    [HttpPost]
    public async Task<ApiResponse<WarehouseDto>> Create([FromBody] WarehouseDto dto)
    {
        var warehouse = new Warehouse
        {
            Code = dto.Code,
            Name = dto.Name,
            Location = dto.Location,
            Remark = dto.Remark,
            CreatedBy = GetCurrentUser()
        };

        var result = await _warehouseRepository.AddAsync(warehouse);
        dto.Id = result.Id;

        return ApiResponse<WarehouseDto>.Success(dto, "创建成功");
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<WarehouseDto>> Update(long id, [FromBody] WarehouseDto dto)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null)
        {
            return ApiResponse<WarehouseDto>.Fail(404, "库房不存在");
        }

        warehouse.Code = dto.Code;
        warehouse.Name = dto.Name;
        warehouse.Location = dto.Location;
        warehouse.Remark = dto.Remark;
        warehouse.UpdatedBy = GetCurrentUser();

        await _warehouseRepository.UpdateAsync(warehouse);

        return ApiResponse<WarehouseDto>.Success(dto, "更新成功");
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _warehouseRepository.DeleteAsync(id);
        return ApiResponse<bool>.Success(true, "删除成功");
    }

    private string GetCurrentUser()
    {
        return User.Identity?.Name ?? "System";
    }
}
