using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaterialManageSystem.Api.DTOs;

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public ApiResponse(T? data, int total = 0)
    {
        Code = 200;
        Message = "操作成功";
        Data = data;
        Total = total;
    }

    public static ApiResponse<T> Success(T? data, string? message = null)
    {
        return new ApiResponse<T>(data)
        {
            Message = message ?? "操作成功"
        };
    }

    public static ApiResponse<T> Success(T? data, int total, int page, int pageSize)
    {
        return new ApiResponse<T>(data, total)
        {
            Page = page,
            PageSize = pageSize
        };
    }

    public static ApiResponse<T> Fail(int code, string message)
    {
        return new ApiResponse<T>(default)
        {
            Code = code,
            Message = message
        };
    }
}

public class LoginRequest
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public long UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int UserType { get; set; }
    public UserDto User { get; set; } = new();
}

public class RegisterRequest
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;

    public long? EmployeeId { get; set; }
}

public class RegisterResponse
{
    public long UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}

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

public class CreateUserRequest
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, ErrorMessage = "用户名长度3-50", MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度6-100")]
    public string Password { get; set; } = string.Empty;

    public long? EmployeeId { get; set; }

    [Range(0, 2, ErrorMessage = "用户类型必须在0-2之间")]
    public int UserType { get; set; }
}

public class UpdateUserRequest
{
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度6-100")]
    public string? Password { get; set; }

    public long? EmployeeId { get; set; }

    [Range(0, 2, ErrorMessage = "用户类型必须在0-2之间")]
    public int? UserType { get; set; }

    public bool? IsActive { get; set; }
}

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

public class CreateEmployeeRequest
{
    [Required(ErrorMessage = "员工编号不能为空")]
    [StringLength(50, ErrorMessage = "员工编号不能超过50个字符")]
    public string EmployeeNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "姓名不能为空")]
    [StringLength(100, ErrorMessage = "姓名不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "部门不能超过100个字符")]
    public string? Department { get; set; }

    [StringLength(100, ErrorMessage = "职位不能超过100个字符")]
    public string? Position { get; set; }

    [StringLength(50, ErrorMessage = "电话不能超过50个字符")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(200, ErrorMessage = "邮箱不能超过200个字符")]
    public string? Email { get; set; }
}

public class UpdateEmployeeRequest
{
    [StringLength(50, ErrorMessage = "员工编号不能超过50个字符")]
    public string? EmployeeNo { get; set; }

    [StringLength(100, ErrorMessage = "姓名不能超过100个字符")]
    public string? Name { get; set; }

    [StringLength(100, ErrorMessage = "部门不能超过100个字符")]
    public string? Department { get; set; }

    [StringLength(100, ErrorMessage = "职位不能超过100个字符")]
    public string? Position { get; set; }

    [StringLength(50, ErrorMessage = "电话不能超过50个字符")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(200, ErrorMessage = "邮箱不能超过200个字符")]
    public string? Email { get; set; }

    public long? UserId { get; set; }

    public bool? IsActive { get; set; }
}

public class CellDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public long LayerId { get; set; }
    public string? LayerCode { get; set; }
    public string? RackCode { get; set; }
    public string? WarehouseCode { get; set; }
    public string CellNo { get; set; } = string.Empty;
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCellRequest
{
    [Required(ErrorMessage = "单元格编号不能为空")]
    public string CellNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "仓库ID不能为空")]
    public long WarehouseId { get; set; }
}

public class UpdateCellRequest
{
    public string? CellNo { get; set; }
    public long? WarehouseId { get; set; }
    public bool? IsActive { get; set; }
}

public class PartNoDto
{
    public long Id { get; set; }
    public string PartNoCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Size { get; set; }
    public string? Packaging { get; set; }
    public string? Unit { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    public long? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal WarningQuantity { get; set; }
    public int WarningDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePartNoRequest
{
    [Required(ErrorMessage = "零件编号不能为空")]
    public string PartNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "名称不能为空")]
    public string Name { get; set; } = string.Empty;

    public string? Specification { get; set; }
    public string? Unit { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
}

public class UpdatePartNoRequest
{
    public string? PartNo { get; set; }
    public string? Name { get; set; }
    public string? Specification { get; set; }
    public string? Unit { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    public bool? IsActive { get; set; }
}

public class DashboardDto
{
    public int TotalWarehouses { get; set; }
    public int TotalPartNos { get; set; }
    public int TotalReels { get; set; }
    public int TotalCells { get; set; }
    public int InStockReels { get; set; }
    public int OutStockReels { get; set; }
    public int OnlineReels { get; set; }
    public int ActiveWarnings { get; set; }
    public int TotalWarnings { get; set; }
    public int TotalEmployees { get; set; }
    public List<WarningDto> Warnings { get; set; } = new();
}

public class WarningDto
{
    public long Id { get; set; }
    public long ReelId { get; set; }
    public string ReelNo { get; set; } = string.Empty;
    public string PartNoName { get; set; } = string.Empty;
    public int WarningType { get; set; }
    public int WarningLevel { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RoleDto
{
    public long Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateRoleRequest
{
    [Required(ErrorMessage = "角色名称不能为空")]
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateRoleRequest
{
    public string? RoleName { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class WarehouseDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Remark { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateWarehouseRequest
{
    [Required(ErrorMessage = "仓库编号不能为空")]
    public string WarehouseNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "仓库名称不能为空")]
    public string Name { get; set; } = string.Empty;

    public string? Location { get; set; }
}

public class UpdateWarehouseRequest
{
    public string? WarehouseNo { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public bool? IsActive { get; set; }
}

public class ReelDto
{
    public long Id { get; set; }
    public string ReelNo { get; set; } = string.Empty;
    public long PartNoId { get; set; }
    public string PartNo { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    public long CellId { get; set; }
    public string CellNo { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReelRequest
{
    [Required(ErrorMessage = "料卷编号不能为空")]
    public string ReelNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "零件ID不能为空")]
    public long PartNoId { get; set; }

    [Required(ErrorMessage = "数量不能为空")]
    public decimal Quantity { get; set; }

    public decimal InitialQuantity { get; set; }

    [Required(ErrorMessage = "单元格ID不能为空")]
    public long CellId { get; set; }

    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class UpdateReelRequest
{
    public string? ReelNo { get; set; }
    public long? PartNoId { get; set; }
    public decimal? Quantity { get; set; }
    public long? CellId { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool? IsActive { get; set; }
}

public class OperationLogDto
{
    public long Id { get; set; }
    public string? OperationType { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestBody { get; set; }
    public int? StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public string? IpAddress { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public long? ExecutionTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReelIdDto
{
    public long Id { get; set; }
    public string ReelNo { get; set; } = string.Empty;
    public long PartNoId { get; set; }
    public string PartNoCode { get; set; } = string.Empty;
    public string PartNoName { get; set; } = string.Empty;
    public long CellId { get; set; }
    public string CellLocation { get; set; } = string.Empty;
    public decimal InitialQuantity { get; set; }
    public decimal CurrentQuantity { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime? FirstUseAt { get; set; }
    public DateTime? LastUseAt { get; set; }
}

public class UsageUploadRequest
{
    public long EmployeeId { get; set; }
    public decimal Quantity { get; set; }
    public TimeSpan? UsageDuration { get; set; }
    public string? Remark { get; set; }
}
