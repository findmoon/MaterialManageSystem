using System.Collections.Generic;

namespace MaterialManageSystem.Core.Entities;

public class Warehouse : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Remark { get; set; }

    public virtual ICollection<Rack> Racks { get; set; } = new List<Rack>();
}
