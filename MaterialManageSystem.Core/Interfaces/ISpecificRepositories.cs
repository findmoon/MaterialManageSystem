using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Entities;

namespace MaterialManageSystem.Core.Interfaces;

public interface IWarehouseRepository : IRepository<Warehouse>
{
    Task<IEnumerable<Warehouse>> GetAllWithRacksAsync();
    Task<Warehouse?> GetByCodeAsync(string code);
}

public interface IRackRepository : IRepository<Rack>
{
    Task<IEnumerable<Rack>> GetByWarehouseIdAsync(long warehouseId);
    Task<Rack?> GetWithLayersAsync(long id);
}

public interface ILayerRepository : IRepository<Layer>
{
    Task<IEnumerable<Layer>> GetByRackIdAsync(long rackId);
}

public interface ICellRepository : IRepository<Cell>
{
    Task<IEnumerable<Cell>> GetByLayerIdAsync(long layerId);
    Task<Cell?> GetWithReelsAsync(long id);
}

public interface IPartNoRepository : IRepository<PartNo>
{
    Task<PartNo?> GetByPartNoCodeAsync(string partNoCode);
    Task<IEnumerable<PartNo>> GetByCategoryIdAsync(long categoryId);
}

public interface IPartNoCategoryRepository : IRepository<PartNoCategory>
{
    Task<IEnumerable<PartNoCategory>> GetTreeAsync();
    Task<PartNoCategory?> GetByCodeAsync(string code);
}

public interface IReelIdRepository : IRepository<ReelId>
{
    Task<ReelId?> GetByReelNoAsync(string reelNo);
    Task<IEnumerable<ReelId>> GetByPartNoIdAsync(long partNoId);
    Task<IEnumerable<ReelId>> GetByCellIdAsync(long cellId);
    Task<IEnumerable<ReelId>> GetByStatusAsync(int status);
}

public interface IReelUsageLogRepository : IRepository<ReelUsageLog>
{
    Task<IEnumerable<ReelUsageLog>> GetByReelIdAsync(long reelId);
    Task<decimal> GetUsageRateAsync(long reelId, int days = 7);
    Task<IEnumerable<ReelUsageLog>> GetUsageTrendAsync(DateTime startDate);
}

public interface IWarningConfigRepository : IRepository<WarningConfig>
{
    Task<IEnumerable<WarningConfig>> GetAllActiveAsync();
    Task<WarningConfig?> GetByPartNoIdAsync(long partNoId);
}

public interface IWarningRecordRepository : IRepository<WarningRecord>
{
    Task<IEnumerable<WarningRecord>> GetActiveWarningsAsync();
    Task<IEnumerable<WarningRecord>> GetByReelIdAsync(long reelId);
    Task<bool> HasUnresolvedWarningAsync(long reelId, int warningType);
}

public interface IOperationLogRepository : IRepository<OperationLog>
{
    Task<IEnumerable<OperationLog>> GetByUserIdAsync(string userId);
    Task<IEnumerable<OperationLog>> GetByOperationTypeAsync(string operationType);
    Task<IEnumerable<OperationLog>> GetRecentLogsAsync(int count);
}

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetActiveRolesAsync();
}
