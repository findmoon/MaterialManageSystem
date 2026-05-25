using System;
using System.Collections.Generic;
using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Core.Entities;

public class User : BaseEntity
{
    public long EmployeeId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserType UserType { get; set; } = UserType.Normal;
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }

    public virtual Employee Employee { get; set; } = null!;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<OperationLog> OperationLogs { get; set; } = new List<OperationLog>();
}
