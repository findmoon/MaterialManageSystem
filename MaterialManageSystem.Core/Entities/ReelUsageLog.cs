using System;
using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Core.Entities;

public class ReelUsageLog : BaseEntity
{
    public long ReelId { get; set; }
    public long EmployeeId { get; set; }
    public UsageType UsageType { get; set; }
    public decimal Quantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public int? UsageDuration { get; set; }
    public string? Remark { get; set; }
    public DateTime RecordedAt { get; set; }

    public virtual ReelId Reel { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;
}
