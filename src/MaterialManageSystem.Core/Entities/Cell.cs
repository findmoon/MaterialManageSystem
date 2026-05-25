using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Core.Entities;

public class Cell : BaseEntity
{
    public long LayerId { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public StorageMode StorageMode { get; set; } = StorageMode.NoRestriction;
    public long? PartNoId { get; set; }
    public string Code { get; set; } = string.Empty;

    public virtual Layer Layer { get; set; } = null!;
    public virtual PartNo? PartNo { get; set; }
    public virtual ICollection<ReelId> Reels { get; set; } = new List<ReelId>();
}
