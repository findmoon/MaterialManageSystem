using System.Collections.Generic;

namespace MaterialManageSystem.Core.Entities;

public class Layer : BaseEntity
{
    public long RackId { get; set; }
    public int Level { get; set; }
    public decimal? Height { get; set; }
    public decimal? WeightLimit { get; set; }
    public string Code { get; set; } = string.Empty;

    public virtual Rack Rack { get; set; } = null!;
    public virtual ICollection<Cell> Cells { get; set; } = new List<Cell>();
}
