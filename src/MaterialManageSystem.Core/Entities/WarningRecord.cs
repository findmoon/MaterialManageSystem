using System;
using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Core.Entities;

public class WarningRecord : BaseEntity
{
    public long ReelId { get; set; }
    public WarningType WarningType { get; set; }
    public decimal CurrentQuantity { get; set; }
    public int? RemainingDays { get; set; }
    public WarningLevel WarningLevel { get; set; }
    public bool IsPushed { get; set; }
    public DateTime? PushedAt { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public string? Remark { get; set; }

    public virtual ReelId Reel { get; set; } = null!;
}
