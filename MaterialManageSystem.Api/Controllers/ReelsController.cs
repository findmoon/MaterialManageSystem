using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaterialManageSystem.Api.DTOs;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Enums;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Infrastructure.Data;

namespace MaterialManageSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReelsController : ControllerBase
{
    private readonly MaterialDbContext _dbContext;
    private readonly IReelIdRepository _reelRepository;
    private readonly IPartNoRepository _partNoRepository;
    private readonly IReelUsageLogRepository _usageLogRepository;

    public ReelsController(
        MaterialDbContext dbContext,
        IReelIdRepository reelRepository,
        IPartNoRepository partNoRepository,
        IReelUsageLogRepository usageLogRepository)
    {
        _dbContext = dbContext;
        _reelRepository = reelRepository;
        _partNoRepository = partNoRepository;
        _usageLogRepository = usageLogRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<List<ReelIdDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? status = null)
    {
        var (items, total) = status.HasValue
            ? await _reelRepository.GetPagedAsync(page, pageSize, r => r.Status == (ReelStatus)status.Value)
            : await _reelRepository.GetPagedAsync(page, pageSize);

        var dtos = items.Select(r => new ReelIdDto
        {
            Id = r.Id,
            ReelNo = r.ReelNo,
            PartNoId = r.PartNoId,
            PartNoCode = r.PartNo?.PartNoCode ?? "",
            PartNoName = r.PartNo?.Name ?? "",
            CellId = r.CellId ?? 0,
            CellLocation = r.Cell != null ? $"{r.Cell.Layer?.Rack?.Warehouse?.Name}/{r.Cell.Layer?.Rack?.Code}/{r.Cell.Layer?.Level}/{r.Cell.Row}-{r.Cell.Col}" : "",
            InitialQuantity = r.InitialQuantity,
            CurrentQuantity = r.CurrentQuantity,
            Status = (int)r.Status,
            StatusName = r.Status.ToString(),
            ManufactureDate = r.ManufactureDate,
            ExpiryDate = r.ExpiryDate,
            ReceivedAt = r.ReceivedAt,
            FirstUseAt = r.FirstUseAt,
            LastUseAt = r.LastUseAt
        }).ToList();

        return ApiResponse<List<ReelIdDto>>.Success(dtos, total, page, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<ReelIdDto>> GetById(long id)
    {
        var reel = await _reelRepository.GetByIdAsync(id);
        if (reel == null)
        {
            return ApiResponse<ReelIdDto>.Fail(404, "料卷不存在");
        }

        return ApiResponse<ReelIdDto>.Success(MapToDto(reel));
    }

    [HttpGet("{id}/usage-logs")]
    public async Task<ApiResponse<List<ReelUsageLog>>> GetUsageLogs(long id)
    {
        var logs = await _usageLogRepository.GetByReelIdAsync(id);
        return ApiResponse<List<ReelUsageLog>>.Success(logs.ToList());
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<ReelIdDto>> Update(long id, [FromBody] UpdateReelRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var reel = await _reelRepository.GetByIdAsync(id);
            if (reel == null)
            {
                return ApiResponse<ReelIdDto>.Fail(404, "料卷不存在");
            }

            if (reel.Status != ReelStatus.InStock)
            {
                return ApiResponse<ReelIdDto>.Fail(400, "只有在库状态的料卷才能编辑");
            }

            if (!string.IsNullOrEmpty(request.ReelNo) && request.ReelNo != reel.ReelNo)
            {
                var existing = await _reelRepository.GetByReelNoAsync(request.ReelNo);
                if (existing != null && existing.Id != id)
                {
                    return ApiResponse<ReelIdDto>.Fail(400, "料卷号已存在");
                }
                reel.ReelNo = request.ReelNo;
            }

            if (request.PartNoId.HasValue && request.PartNoId != reel.PartNoId)
            {
                var partNo = await _partNoRepository.GetByIdAsync(request.PartNoId.Value);
                if (partNo == null)
                {
                    return ApiResponse<ReelIdDto>.Fail(400, "物料不存在");
                }
                
                // 更新原物料数量
                var oldPartNo = await _partNoRepository.GetByIdAsync(reel.PartNoId);
                if (oldPartNo != null)
                {
                    oldPartNo.TotalQuantity -= reel.CurrentQuantity;
                    _partNoRepository.Update(oldPartNo);
                }
                
                // 更新新物料数量
                partNo.TotalQuantity += reel.CurrentQuantity;
                _partNoRepository.Update(partNo);
                
                reel.PartNoId = request.PartNoId.Value;
            }

            if (request.CellId.HasValue)
            {
                reel.CellId = request.CellId.Value;
            }

            if (request.ManufactureDate.HasValue)
            {
                reel.ManufactureDate = request.ManufactureDate.Value;
            }

            if (request.ExpiryDate.HasValue)
            {
                reel.ExpiryDate = request.ExpiryDate.Value;
            }

            reel.UpdatedAt = DateTime.Now;
            reel.UpdatedBy = GetCurrentUser();
            _reelRepository.Update(reel);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponse<ReelIdDto>.Success(MapToDto(reel), "编辑成功");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPost]
    public async Task<ApiResponse<ReelIdDto>> Create([FromBody] CreateReelRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var partNo = await _partNoRepository.GetByIdAsync(request.PartNoId);
            if (partNo == null)
            {
                return ApiResponse<ReelIdDto>.Fail(400, "物料不存在");
            }

            var reel = new ReelId
            {
                ReelNo = request.ReelNo,
                PartNoId = request.PartNoId,
                CellId = request.CellId,
                InitialQuantity = request.InitialQuantity,
                CurrentQuantity = request.InitialQuantity,
                Status = ReelStatus.InStock,
                ManufactureDate = request.ManufactureDate,
                ExpiryDate = request.ExpiryDate,
                ReceivedAt = DateTime.Now,
                CreatedBy = GetCurrentUser()
            };

            _reelRepository.Add(reel);
            await _dbContext.SaveChangesAsync();

            // Update partno total quantity
            partNo.TotalQuantity += request.InitialQuantity;
            _partNoRepository.Update(partNo);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            var dto = MapToDto(reel);
            dto.PartNoCode = partNo.PartNoCode;
            dto.PartNoName = partNo.Name;

            return ApiResponse<ReelIdDto>.Success(dto, "创建成功");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPut("{id}/checkout")]
    public async Task<ApiResponse<ReelIdDto>> Checkout(long id, [FromBody] UsageUploadRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var reel = await _reelRepository.GetByIdAsync(id);
            if (reel == null)
            {
                return ApiResponse<ReelIdDto>.Fail(404, "料卷不存在");
            }

            if (reel.Status != ReelStatus.InStock)
            {
                return ApiResponse<ReelIdDto>.Fail(400, "料卷状态不允许出库");
            }

            reel.Status = ReelStatus.OutStock;
            reel.UpdatedBy = GetCurrentUser();
            _reelRepository.Update(reel);

            // Record usage
            var log = new ReelUsageLog
            {
                ReelId = id,
                EmployeeId = request.EmployeeId,
                UsageType = UsageType.Checkout,
                Quantity = request.Quantity > 0 ? request.Quantity : reel.CurrentQuantity,
                RemainingQuantity = reel.CurrentQuantity,
                Remark = request.Remark,
                RecordedAt = DateTime.Now,
                CreatedBy = GetCurrentUser()
            };
            _usageLogRepository.Add(log);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponse<ReelIdDto>.Success(MapToDto(reel), "出库成功");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPut("{id}/online")]
    public async Task<ApiResponse<ReelIdDto>> Online(long id, [FromBody] UsageUploadRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var reel = await _reelRepository.GetByIdAsync(id);
            if (reel == null)
            {
                return ApiResponse<ReelIdDto>.Fail(404, "料卷不存在");
            }

            if (reel.Status != ReelStatus.OutStock)
            {
                return ApiResponse<ReelIdDto>.Fail(400, "料卷状态不允许上线");
            }

            reel.Status = ReelStatus.Online;
            reel.FirstUseAt = reel.FirstUseAt ?? DateTime.Now;
            reel.LastUseAt = DateTime.Now;
            reel.UpdatedBy = GetCurrentUser();
            _reelRepository.Update(reel);

            var log = new ReelUsageLog
            {
                ReelId = id,
                EmployeeId = request.EmployeeId,
                UsageType = UsageType.OnlineUse,
                Quantity = request.Quantity,
                RemainingQuantity = reel.CurrentQuantity,
                UsageDuration = request.UsageDuration.HasValue ? (int)request.UsageDuration.Value.TotalSeconds : null,
                Remark = request.Remark,
                RecordedAt = DateTime.Now,
                CreatedBy = GetCurrentUser()
            };
            _usageLogRepository.Add(log);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponse<ReelIdDto>.Success(MapToDto(reel), "上线成功");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPut("{id}/return")]
    public async Task<ApiResponse<ReelIdDto>> Return(long id, [FromBody] UsageUploadRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var reel = await _reelRepository.GetByIdAsync(id);
            if (reel == null)
            {
                return ApiResponse<ReelIdDto>.Fail(404, "料卷不存在");
            }

            if (reel.Status != ReelStatus.Online && reel.Status != ReelStatus.OutStock)
            {
                return ApiResponse<ReelIdDto>.Fail(400, "料卷状态不允许退库");
            }

            reel.Status = ReelStatus.InStock;
            reel.CurrentQuantity = request.Quantity;
            reel.UpdatedBy = GetCurrentUser();
            _reelRepository.Update(reel);

            var log = new ReelUsageLog
            {
                ReelId = id,
                EmployeeId = request.EmployeeId,
                UsageType = UsageType.Return,
                Quantity = request.Quantity,
                RemainingQuantity = reel.CurrentQuantity,
                Remark = request.Remark,
                RecordedAt = DateTime.Now,
                CreatedBy = GetCurrentUser()
            };
            _usageLogRepository.Add(log);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponse<ReelIdDto>.Success(MapToDto(reel), "退库成功");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPut("{id}/scrap")]
    public async Task<ApiResponse<ReelIdDto>> Scrap(long id, [FromBody] UsageUploadRequest request)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var reel = await _reelRepository.GetByIdAsync(id);
            if (reel == null)
            {
                return ApiResponse<ReelIdDto>.Fail(404, "料卷不存在");
            }

            var previousQuantity = reel.CurrentQuantity;
            reel.Status = ReelStatus.Scrapped;
            reel.CurrentQuantity = 0;
            reel.IsActive = false;
            reel.UpdatedBy = GetCurrentUser();
            _reelRepository.Update(reel);

            var log = new ReelUsageLog
            {
                ReelId = id,
                EmployeeId = request.EmployeeId,
                UsageType = UsageType.Scrap,
                Quantity = previousQuantity,
                RemainingQuantity = 0,
                Remark = request.Remark,
                RecordedAt = DateTime.Now,
                CreatedBy = GetCurrentUser()
            };
            _usageLogRepository.Add(log);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponse<ReelIdDto>.Success(MapToDto(reel), "报废成功");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private ReelIdDto MapToDto(ReelId r)
    {
        return new ReelIdDto
        {
            Id = r.Id,
            ReelNo = r.ReelNo,
            PartNoId = r.PartNoId,
            PartNoCode = r.PartNo?.PartNoCode ?? "",
            PartNoName = r.PartNo?.Name ?? "",
            CellId = r.CellId ?? 0,
            InitialQuantity = r.InitialQuantity,
            CurrentQuantity = r.CurrentQuantity,
            Status = (int)r.Status,
            StatusName = r.Status.ToString(),
            ManufactureDate = r.ManufactureDate,
            ExpiryDate = r.ExpiryDate,
            ReceivedAt = r.ReceivedAt,
            FirstUseAt = r.FirstUseAt,
            LastUseAt = r.LastUseAt
        };
    }

    private string GetCurrentUser()
    {
        return User.Identity?.Name ?? "System";
    }
}
