using System;
using System.Collections.Generic;
using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Core.Entities;

public class ReelId : BaseEntity
{
    public string ReelNo { get; set; } = string.Empty;
    public long PartNoId { get; set; }
    public long? CellId { get; set; }
    public decimal InitialQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public ReelStatus Status { get; set; } = ReelStatus.InStock;
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime? FirstUseAt { get; set; }
    public DateTime? LastUseAt { get; set; }

    public virtual PartNo PartNo { get; set; } = null!;
    public virtual Cell? Cell { get; set; }
    public virtual ICollection<ReelUsageLog> UsageLogs { get; set; } = new List<ReelUsageLog>();
}
