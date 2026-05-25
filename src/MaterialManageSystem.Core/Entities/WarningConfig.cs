using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Core.Entities;

public class WarningConfig : BaseEntity
{
    public long? PartNoId { get; set; }
    public WarningType WarningType { get; set; }
    public decimal? QuantityThreshold { get; set; }
    public int? DaysThreshold { get; set; }
    public bool IsEnabled { get; set; } = true;

    public virtual PartNo? PartNo { get; set; }
}
