using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaterialManageSystem.Api.DTOs;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IPartNoRepository _partNoRepository;
    private readonly IReelIdRepository _reelRepository;
    private readonly IWarningRecordRepository _warningRepository;
    private readonly IReelUsageLogRepository _usageLogRepository;

    public DashboardController(
        IWarehouseRepository warehouseRepository,
        IPartNoRepository partNoRepository,
        IReelIdRepository reelRepository,
        IWarningRecordRepository warningRepository,
        IReelUsageLogRepository usageLogRepository)
    {
        _warehouseRepository = warehouseRepository;
        _partNoRepository = partNoRepository;
        _reelRepository = reelRepository;
        _warningRepository = warningRepository;
        _usageLogRepository = usageLogRepository;
    }

    [HttpGet("overview")]
    public async Task<ApiResponse<DashboardDto>> GetOverview()
    {
        var warehouses = await _warehouseRepository.GetAllAsync();
        var partNos = await _partNoRepository.GetAllAsync();
        var allReels = await _reelRepository.GetAllAsync();
        var activeWarnings = await _warningRepository.GetActiveWarningsAsync();

        var dto = new DashboardDto
        {
            TotalWarehouses = warehouses.Count(),
            TotalPartNos = partNos.Count(),
            TotalReels = allReels.Count(),
            InStockReels = allReels.Count(r => r.Status == ReelStatus.InStock),
            OutStockReels = allReels.Count(r => r.Status == ReelStatus.OutStock),
            OnlineReels = allReels.Count(r => r.Status == ReelStatus.Online),
            ActiveWarnings = activeWarnings.Count()
        };

        return ApiResponse<DashboardDto>.Success(dto);
    }

    [HttpGet("inventory")]
    public async Task<ApiResponse<object>> GetInventory()
    {
        var partNos = await _partNoRepository.GetAllAsync();

        var inventory = partNos.Select(p => new
        {
            p.Id,
            p.PartNoCode,
            p.Name,
            p.TotalQuantity,
            p.WarningQuantity,
            ReelCount = p.Reels.Count
        });

        return ApiResponse<object>.Success(inventory);
    }

    [HttpGet("usage-trend")]
    public async Task<ApiResponse<object>> GetUsageTrend([FromQuery] int days = 7)
    {
        var startDate = DateTime.Now.AddDays(-days);
        
        var usageData = await _usageLogRepository.GetUsageTrendAsync(startDate);
        
        var trendData = usageData
            .GroupBy(d => d.RecordedAt.Date)
            .Select(g => new
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                TotalUsage = g.Sum(d => d.Quantity),
                UsageCount = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToList();

        return ApiResponse<object>.Success(new
        {
            Days = days,
            Trend = trendData,
            TotalUsage = trendData.Sum(t => t.TotalUsage),
            AverageDailyUsage = trendData.Any() ? trendData.Average(t => t.TotalUsage) : 0
        });
    }

    [HttpGet("warning-summary")]
    public async Task<ApiResponse<object>> GetWarningSummary()
    {
        var activeWarnings = await _warningRepository.GetActiveWarningsAsync();
        
        return ApiResponse<object>.Success(new
        {
            TotalWarnings = activeWarnings.Count(),
            CriticalWarnings = activeWarnings.Count(w => w.WarningLevel == WarningLevel.Severe),
            NormalWarnings = activeWarnings.Count(w => w.WarningLevel == WarningLevel.Notice),
            Warnings = activeWarnings.Select(w => new
            {
                w.Id,
                w.ReelId,
                ReelNo = w.Reel?.ReelNo ?? string.Empty,
                PartNoName = w.Reel?.PartNo?.Name ?? string.Empty,
                w.WarningType,
                w.WarningLevel,
                Message = w.Remark,
                w.CreatedAt
            }).ToList()
        });
    }
}
