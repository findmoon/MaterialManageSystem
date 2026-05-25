using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Enums;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaterialManageSystem.Infrastructure.Services;

public class WarningDetectionService : IWarningDetectionService
{
    private readonly MaterialDbContext _dbContext;
    private readonly IReelIdRepository _reelRepository;
    private readonly IWarningConfigRepository _configRepository;
    private readonly IWarningRecordRepository _recordRepository;

    public WarningDetectionService(
        MaterialDbContext dbContext,
        IReelIdRepository reelRepository,
        IWarningConfigRepository configRepository,
        IWarningRecordRepository recordRepository)
    {
        _dbContext = dbContext;
        _reelRepository = reelRepository;
        _configRepository = configRepository;
        _recordRepository = recordRepository;
    }

    public async Task DetectAndCreateWarningsAsync()
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var activeReels = await _dbContext.ReelIds
                .Include(r => r.PartNo)
                .Where(r => r.IsActive && r.Status != ReelStatus.Scrapped)
                .ToListAsync();

            var activeConfigs = await _configRepository.GetAllActiveAsync();

            foreach (var reel in activeReels)
            {
                var now = DateTime.Now;
                
                // 检查所有配置（全局和特定物料）
                var relevantConfigs = activeConfigs.Where(c => 
                    c.PartNoId == null || c.PartNoId == reel.PartNoId).ToList();

                foreach (var config in relevantConfigs)
                {
                    await CheckWarningForReelAsync(reel, config, now);
                }
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task CheckWarningForReelAsync(ReelId reel, WarningConfig config, DateTime now)
    {
        bool shouldWarn = false;
        string? message = null;
        decimal currentQty = reel.CurrentQuantity;
        int? remainingDays = null;

        switch (config.WarningType)
        {
            case WarningType.Quantity:
                if (config.QuantityThreshold.HasValue && reel.CurrentQuantity <= config.QuantityThreshold.Value)
                {
                    shouldWarn = true;
                    message = $"料卷 {reel.ReelNo} 库存不足，当前数量：{reel.CurrentQuantity}";
                }
                break;

            case WarningType.Time:
                if (reel.ExpiryDate.HasValue && config.DaysThreshold.HasValue)
                {
                    var daysUntilExpiry = (reel.ExpiryDate.Value - now).TotalDays;
                    remainingDays = (int)Math.Floor(daysUntilExpiry);
                    if (daysUntilExpiry <= (double)config.DaysThreshold.Value)
                    {
                        shouldWarn = true;
                        message = $"料卷 {reel.ReelNo} 即将过期，过期日期：{reel.ExpiryDate.Value:yyyy-MM-dd}";
                    }
                }
                break;

            case WarningType.Both:
                // 检查数量和时间
                if (config.QuantityThreshold.HasValue && reel.CurrentQuantity <= config.QuantityThreshold.Value)
                {
                    shouldWarn = true;
                    message = $"料卷 {reel.ReelNo} 库存不足，当前数量：{reel.CurrentQuantity}";
                }
                else if (reel.ExpiryDate.HasValue && config.DaysThreshold.HasValue)
                {
                    var days = (reel.ExpiryDate.Value - now).TotalDays;
                    remainingDays = (int)Math.Floor(days);
                    if (days <= (double)config.DaysThreshold.Value)
                    {
                        shouldWarn = true;
                        message = $"料卷 {reel.ReelNo} 即将过期，过期日期：{reel.ExpiryDate.Value:yyyy-MM-dd}";
                    }
                }
                break;
        }

        if (shouldWarn && !await _recordRepository.HasUnresolvedWarningAsync(reel.Id, (int)config.WarningType))
        {
            var warning = new WarningRecord
            {
                ReelId = reel.Id,
                WarningType = config.WarningType,
                CurrentQuantity = currentQty,
                RemainingDays = remainingDays,
                WarningLevel = WarningLevel.Warning,
                Remark = message,
                IsResolved = false,
                CreatedBy = "System"
            };

            _recordRepository.Add(warning);
        }
    }
}
