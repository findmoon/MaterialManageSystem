namespace MaterialManageSystem.Admin.Pages.Employees;

public class EmployeeDto
{
    public long Id { get; set; }
    public string EmployeeNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public long UserId { get; set; }
    public string? Username { get; set; }
    public bool HasUser { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EmployeeResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public EmployeeDto? Data { get; set; }
}

public class EmployeeListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<EmployeeDto>? Data { get; set; }
}

public class UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public long EmployeeId { get; set; }
}

public class UserListResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public List<UserDto>? Data { get; set; }
}

public class ApiErrorResponse
{
    public int Code { get; set; }
    public string? Message { get; set; }
}
