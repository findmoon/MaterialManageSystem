namespace MaterialManageSystem.Admin.Pages.Users;

public class UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeNo { get; set; } = string.Empty;
    public int UserType { get; set; }
    public string UserTypeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public UserDto? Data { get; set; }
}

public class UserListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<UserDto>? Data { get; set; }
}

public class EmployeeDto
{
    public long Id { get; set; }
    public string EmployeeNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long UserId { get; set; }
}

public class EmployeeListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<EmployeeDto>? Data { get; set; }
}

public class ApiErrorResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
}
