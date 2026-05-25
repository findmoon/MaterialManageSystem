using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Enums;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaterialManageSystem.Infrastructure.Repositories;

public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Warehouse>> GetAllWithRacksAsync()
    {
        return await _dbSet
            .Include(w => w.Racks)
            .Where(w => w.IsActive)
            .ToListAsync();
    }

    public async Task<Warehouse?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.Code == code && w.IsActive);
    }
}

public class RackRepository : Repository<Rack>, IRackRepository
{
    public RackRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rack>> GetByWarehouseIdAsync(long warehouseId)
    {
        return await _dbSet
            .Include(r => r.Layers)
            .Where(r => r.WarehouseId == warehouseId && r.IsActive)
            .ToListAsync();
    }

    public async Task<Rack?> GetWithLayersAsync(long id)
    {
        return await _dbSet
            .Include(r => r.Layers)
            .ThenInclude(l => l.Cells)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
    }
}

public class LayerRepository : Repository<Layer>, ILayerRepository
{
    public LayerRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Layer>> GetByRackIdAsync(long rackId)
    {
        return await _dbSet
            .Include(l => l.Cells)
            .Where(l => l.RackId == rackId && l.IsActive)
            .ToListAsync();
    }
}

public class CellRepository : Repository<Cell>, ICellRepository
{
    public CellRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Cell>> GetByLayerIdAsync(long layerId)
    {
        return await _dbSet
            .Include(c => c.Reels)
            .Where(c => c.LayerId == layerId && c.IsActive)
            .ToListAsync();
    }

    public async Task<Cell?> GetWithReelsAsync(long id)
    {
        return await _dbSet
            .Include(c => c.Reels)
            .ThenInclude(r => r.PartNo)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
    }
}

public class PartNoRepository : Repository<PartNo>, IPartNoRepository
{
    public PartNoRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<PartNo?> GetByPartNoCodeAsync(string partNoCode)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.PartNoCode == partNoCode && p.IsActive);
    }

    public async Task<IEnumerable<PartNo>> GetByCategoryIdAsync(long categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync();
    }
}

public class PartNoCategoryRepository : Repository<PartNoCategory>, IPartNoCategoryRepository
{
    public PartNoCategoryRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PartNoCategory>> GetTreeAsync()
    {
        return await _dbSet
            .Include(c => c.Children)
            .Where(c => c.ParentId == null && c.IsActive)
            .ToListAsync();
    }

    public async Task<PartNoCategory?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Code == code && c.IsActive);
    }
}

public class ReelIdRepository : Repository<ReelId>, IReelIdRepository
{
    public ReelIdRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<ReelId?> GetByReelNoAsync(string reelNo)
    {
        return await _dbSet
            .Include(r => r.PartNo)
            .Include(r => r.Cell)
            .ThenInclude(c => c!.Layer)
            .ThenInclude(l => l.Rack)
            .ThenInclude(r => r.Warehouse)
            .FirstOrDefaultAsync(r => r.ReelNo == reelNo && r.IsActive);
    }

    public async Task<IEnumerable<ReelId>> GetByPartNoIdAsync(long partNoId)
    {
        return await _dbSet
            .Include(r => r.Cell)
            .Where(r => r.PartNoId == partNoId && r.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReelId>> GetByCellIdAsync(long cellId)
    {
        return await _dbSet
            .Include(r => r.PartNo)
            .Where(r => r.CellId == cellId && r.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReelId>> GetByStatusAsync(int status)
    {
        return await _dbSet
            .Include(r => r.PartNo)
            .Where(r => r.Status == (ReelStatus)status && r.IsActive)
            .ToListAsync();
    }
}

public class ReelUsageLogRepository : Repository<ReelUsageLog>, IReelUsageLogRepository
{
    public ReelUsageLogRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ReelUsageLog>> GetByReelIdAsync(long reelId)
    {
        return await _dbSet
            .Include(l => l.Employee)
            .Where(l => l.ReelId == reelId)
            .OrderByDescending(l => l.RecordedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetUsageRateAsync(long reelId, int days = 7)
    {
        var startDate = System.DateTime.Now.AddDays(-days);
        var logs = await _dbSet
            .Where(l => l.ReelId == reelId && l.RecordedAt >= startDate && l.UsageType != UsageType.Return)
            .ToListAsync();

        if (logs.Count == 0) return 0;

        var totalUsage = logs.Sum(l => l.Quantity);
        return totalUsage / days;
    }

    public async Task<IEnumerable<ReelUsageLog>> GetUsageTrendAsync(DateTime startDate)
    {
        return await _dbSet
            .Include(l => l.Reel)
            .ThenInclude(r => r.PartNo)
            .Where(l => l.RecordedAt >= startDate && l.UsageType != UsageType.Return)
            .OrderBy(l => l.RecordedAt)
            .ToListAsync();
    }
}

public class WarningConfigRepository : Repository<WarningConfig>, IWarningConfigRepository
{
    public WarningConfigRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WarningConfig>> GetAllActiveAsync()
    {
        return await _dbSet.Where(c => c.IsActive && c.IsEnabled).ToListAsync();
    }

    public async Task<WarningConfig?> GetByPartNoIdAsync(long partNoId)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.PartNoId == partNoId && c.IsActive && c.IsEnabled);
    }
}

public class WarningRecordRepository : Repository<WarningRecord>, IWarningRecordRepository
{
    public WarningRecordRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WarningRecord>> GetActiveWarningsAsync()
    {
        return await _dbSet
            .Include(r => r.Reel)
            .ThenInclude(r => r.PartNo)
            .Where(r => r.IsActive && !r.IsResolved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WarningRecord>> GetByReelIdAsync(long reelId)
    {
        return await _dbSet.Where(r => r.ReelId == reelId).ToListAsync();
    }

    public async Task<bool> HasUnresolvedWarningAsync(long reelId, int warningType)
    {
        return await _dbSet.AnyAsync(r => r.ReelId == reelId && (int)r.WarningType == warningType && !r.IsResolved && r.IsActive);
    }
}

public class OperationLogRepository : Repository<OperationLog>, IOperationLogRepository
{
    public OperationLogRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<OperationLog>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OperationLog>> GetByOperationTypeAsync(string operationType)
    {
        return await _dbSet
            .Where(l => l.OperationType == operationType)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OperationLog>> GetRecentLogsAsync(int count)
    {
        return await _dbSet
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(MaterialDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Name == name && r.IsActive);
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync()
    {
        return await _dbSet.Where(r => r.IsActive).ToListAsync();
    }
}
