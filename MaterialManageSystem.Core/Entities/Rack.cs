using System.Collections.Generic;

namespace MaterialManageSystem.Core.Entities;

public class Rack : BaseEntity
{
    public long WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RowCount { get; set; } = 1;
    public int ColumnCount { get; set; } = 1;

    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual ICollection<Layer> Layers { get; set; } = new List<Layer>();
}
