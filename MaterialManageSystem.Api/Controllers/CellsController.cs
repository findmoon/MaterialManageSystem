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
public class CellsController : ControllerBase
{
    private readonly ICellRepository _cellRepository;
    private readonly IRackRepository _rackRepository;
    private readonly ILayerRepository _layerRepository;

    public CellsController(
        ICellRepository cellRepository,
        IRackRepository rackRepository,
        ILayerRepository layerRepository)
    {
        _cellRepository = cellRepository;
        _rackRepository = rackRepository;
        _layerRepository = layerRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<List<CellDto>>> GetAll([FromQuery] long? layerId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var cells = await _cellRepository.GetAllAsync();
        
        if (layerId.HasValue)
        {
            cells = cells.Where(c => c.LayerId == layerId.Value).ToList();
        }

        var pagedCells = cells.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = pagedCells.Select(c => new CellDto
        {
            Id = c.Id,
            Code = c.Code,
            Row = c.Row,
            Col = c.Col,
            LayerId = c.LayerId,
            LayerCode = c.Layer?.Code ?? "",
            RackCode = c.Layer?.Rack?.Code ?? "",
            WarehouseCode = c.Layer?.Rack?.Warehouse?.Code ?? "",
            IsActive = c.IsActive
        }).ToList();

        return ApiResponse<List<CellDto>>.Success(dtos, cells.Count(), page, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<CellDto>> GetById(long id)
    {
        var cell = await _cellRepository.GetByIdAsync(id);
        if (cell == null)
        {
            return ApiResponse<CellDto>.Fail(404, "单元格不存在");
        }

        return ApiResponse<CellDto>.Success(new CellDto
        {
            Id = cell.Id,
            Code = cell.Code,
            Row = cell.Row,
            Col = cell.Col,
            LayerId = cell.LayerId,
            LayerCode = cell.Layer?.Code ?? "",
            RackCode = cell.Layer?.Rack?.Code ?? "",
            WarehouseCode = cell.Layer?.Rack?.Warehouse?.Code ?? "",
            IsActive = cell.IsActive
        });
    }

    [HttpPost]
    public async Task<ApiResponse<CellDto>> Create([FromBody] CreateCellRequest request)
    {
        var cell = new Cell
        {
            Code = request.Code,
            Row = request.Row,
            Col = request.Col,
            LayerId = request.LayerId,
            CreatedBy = GetCurrentUser()
        };

        var result = await _cellRepository.AddAsync(cell);

        var layer = await _layerRepository.GetByIdAsync(request.LayerId);

        return ApiResponse<CellDto>.Success(new CellDto
        {
            Id = result.Id,
            Code = result.Code,
            Row = result.Row,
            Col = result.Col,
            LayerId = result.LayerId,
            LayerCode = layer?.Code ?? "",
            RackCode = layer?.Rack?.Code ?? "",
            WarehouseCode = layer?.Rack?.Warehouse?.Code ?? "",
            IsActive = result.IsActive
        }, "创建成功");
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<CellDto>> Update(long id, [FromBody] UpdateCellRequest request)
    {
        var cell = await _cellRepository.GetByIdAsync(id);
        if (cell == null)
        {
            return ApiResponse<CellDto>.Fail(404, "单元格不存在");
        }

        cell.Code = request.Code;
        cell.Row = request.Row;
        cell.Col = request.Col;
        cell.IsActive = request.IsActive;
        cell.UpdatedBy = GetCurrentUser();

        await _cellRepository.UpdateAsync(cell);

        var layer = await _layerRepository.GetByIdAsync(cell.LayerId);

        return ApiResponse<CellDto>.Success(new CellDto
        {
            Id = cell.Id,
            Code = cell.Code,
            Row = cell.Row,
            Col = cell.Col,
            LayerId = cell.LayerId,
            LayerCode = layer?.Code ?? "",
            RackCode = layer?.Rack?.Code ?? "",
            WarehouseCode = layer?.Rack?.Warehouse?.Code ?? "",
            IsActive = cell.IsActive
        }, "更新成功");
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _cellRepository.DeleteAsync(id);
        return ApiResponse<bool>.Success(true, "删除成功");
    }

    [HttpGet("layers")]
    public async Task<ApiResponse<List<LayerDto>>> GetLayers([FromQuery] long? rackId = null)
    {
        var layers = await _layerRepository.GetAllAsync();
        
        if (rackId.HasValue)
        {
            layers = layers.Where(l => l.RackId == rackId.Value).ToList();
        }

        var dtos = layers.Select(l => new LayerDto
        {
            Id = l.Id,
            Code = l.Code,
            Level = l.Level,
            RackId = l.RackId,
            RackCode = l.Rack?.Code ?? "",
            WarehouseCode = l.Rack?.Warehouse?.Code ?? ""
        }).ToList();

        return ApiResponse<List<LayerDto>>.Success(dtos);
    }

    [HttpGet("racks")]
    public async Task<ApiResponse<List<RackDto>>> GetRacks([FromQuery] long? warehouseId = null)
    {
        var racks = await _rackRepository.GetAllAsync();
        
        if (warehouseId.HasValue)
        {
            racks = racks.Where(r => r.WarehouseId == warehouseId.Value).ToList();
        }

        var dtos = racks.Select(r => new RackDto
        {
            Id = r.Id,
            Code = r.Code,
            WarehouseId = r.WarehouseId,
            WarehouseCode = r.Warehouse?.Code ?? ""
        }).ToList();

        return ApiResponse<List<RackDto>>.Success(dtos);
    }

    private string GetCurrentUser()
    {
        return User.Identity?.Name ?? "System";
    }
}

public class CreateCellRequest
{
    public string Code { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public long LayerId { get; set; }
}

public class UpdateCellRequest
{
    public string Code { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsActive { get; set; }
}

public class LayerDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Level { get; set; }
    public long RackId { get; set; }
    public string RackCode { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
}

public class RackDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public long WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
}