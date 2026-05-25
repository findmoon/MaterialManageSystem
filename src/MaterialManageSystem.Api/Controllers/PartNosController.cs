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
public class PartNosController : ControllerBase
{
    private readonly IPartNoRepository _partNoRepository;
    private readonly IPartNoCategoryRepository _categoryRepository;

    public PartNosController(IPartNoRepository partNoRepository, IPartNoCategoryRepository categoryRepository)
    {
        _partNoRepository = partNoRepository;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<List<PartNoDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (items, total) = await _partNoRepository.GetPagedAsync(page, pageSize);

        var dtos = items.Select(p => new PartNoDto
        {
            Id = p.Id,
            PartNoCode = p.PartNoCode,
            Name = p.Name,
            Specification = p.Specification,
            Size = p.Size,
            Packaging = p.Packaging,
            Unit = p.Unit,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            TotalQuantity = p.TotalQuantity,
            WarningQuantity = p.WarningQuantity ?? 0m,
            WarningDays = p.WarningDays ?? 0
        }).ToList();

        return ApiResponse<List<PartNoDto>>.Success(dtos, total, page, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<PartNoDto>> GetById(long id)
    {
        var partNo = await _partNoRepository.GetByIdAsync(id);
        if (partNo == null)
        {
            return ApiResponse<PartNoDto>.Fail(404, "物料不存在");
        }

        return ApiResponse<PartNoDto>.Success(new PartNoDto
        {
            Id = partNo.Id,
            PartNoCode = partNo.PartNoCode,
            Name = partNo.Name,
            Specification = partNo.Specification,
            Size = partNo.Size,
            Packaging = partNo.Packaging,
            Unit = partNo.Unit,
            CategoryId = partNo.CategoryId,
            CategoryName = partNo.Category?.Name,
            TotalQuantity = partNo.TotalQuantity,
            WarningQuantity = partNo.WarningQuantity ?? 0,
            WarningDays = partNo.WarningDays ?? 0
        });
    }

    [HttpGet("{id}/reels")]
    public async Task<ApiResponse<List<ReelIdDto>>> GetReels(long id)
    {
        var reels = await _partNoRepository.GetByIdAsync(id);
        if (reels == null)
        {
            return ApiResponse<List<ReelIdDto>>.Fail(404, "物料不存在");
        }

        return ApiResponse<List<ReelIdDto>>.Success(new List<ReelIdDto>());
    }

    [HttpPost]
    public async Task<ApiResponse<PartNoDto>> Create([FromBody] PartNoDto dto)
    {
        var partNo = new PartNo
        {
            PartNoCode = dto.PartNoCode,
            Name = dto.Name,
            Specification = dto.Specification,
            Size = dto.Size,
            Packaging = dto.Packaging,
            Unit = dto.Unit,
            CategoryId = dto.CategoryId,
            WarningQuantity = dto.WarningQuantity,
            WarningDays = dto.WarningDays,
            CreatedBy = GetCurrentUser()
        };

        var result = await _partNoRepository.AddAsync(partNo);
        dto.Id = result.Id;

        return ApiResponse<PartNoDto>.Success(dto, "创建成功");
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<PartNoDto>> Update(long id, [FromBody] PartNoDto dto)
    {
        var partNo = await _partNoRepository.GetByIdAsync(id);
        if (partNo == null)
        {
            return ApiResponse<PartNoDto>.Fail(404, "物料不存在");
        }

        partNo.PartNoCode = dto.PartNoCode;
        partNo.Name = dto.Name;
        partNo.Specification = dto.Specification;
        partNo.Size = dto.Size;
        partNo.Packaging = dto.Packaging;
        partNo.Unit = dto.Unit;
        partNo.CategoryId = dto.CategoryId;
        partNo.WarningQuantity = dto.WarningQuantity;
        partNo.WarningDays = dto.WarningDays;
        partNo.UpdatedBy = GetCurrentUser();

        await _partNoRepository.UpdateAsync(partNo);

        return ApiResponse<PartNoDto>.Success(dto, "更新成功");
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _partNoRepository.DeleteAsync(id);
        return ApiResponse<bool>.Success(true, "删除成功");
    }

    private string GetCurrentUser()
    {
        return User.Identity?.Name ?? "System";
    }
}
