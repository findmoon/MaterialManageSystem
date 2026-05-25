using System.Collections.Generic;

namespace MaterialManageSystem.Core.Entities;

public class PartNoCategory : BaseEntity
{
    public long? ParentId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public virtual PartNoCategory? Parent { get; set; }
    public virtual ICollection<PartNoCategory> Children { get; set; } = new List<PartNoCategory>();
    public virtual ICollection<PartNo> PartNos { get; set; } = new List<PartNo>();
}
