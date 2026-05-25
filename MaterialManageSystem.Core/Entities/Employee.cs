namespace MaterialManageSystem.Core.Entities;

public class Employee : BaseEntity
{
    public string EmployeeNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<ReelUsageLog> UsageLogs { get; set; } = new List<ReelUsageLog>();
}
