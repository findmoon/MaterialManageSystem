using System;

namespace MaterialManageSystem.Core.Entities;

public class OperationLog : BaseEntity
{
    public string OperationType { get; set; } = string.Empty;
    
    public string ControllerName { get; set; } = string.Empty;
    
    public string ActionName { get; set; } = string.Empty;
    
    public string RequestMethod { get; set; } = string.Empty;
    
    public string RequestPath { get; set; } = string.Empty;
    
    public string? RequestBody { get; set; }
    
    public string? ResponseBody { get; set; }
    
    public int StatusCode { get; set; }
    
    public string? UserId { get; set; }
    
    public string? UserName { get; set; }
    
    public string? IpAddress { get; set; }
    
    public string? UserAgent { get; set; }
    
    public long DurationMs { get; set; }
}