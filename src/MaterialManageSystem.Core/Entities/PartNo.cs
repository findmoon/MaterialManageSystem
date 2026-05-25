using System.Collections.Generic;

namespace MaterialManageSystem.Core.Entities;

public class PartNo : BaseEntity
{
    public string PartNoCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Size { get; set; }
    public string? Packaging { get; set; }
    public string Unit { get; set; } = "EA";
    public long? CategoryId { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal? WarningQuantity { get; set; }
    public int? WarningDays { get; set; }

    public virtual PartNoCategory? Category { get; set; }
    public virtual ICollection<ReelId> Reels { get; set; } = new List<ReelId>();
}
