using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaterialManageSystem.Api.DTOs;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Interfaces;

namespace MaterialManageSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OperationLogsController : ControllerBase
{
    private readonly IRepository<OperationLog> _operationLogRepository;

    public OperationLogsController(IRepository<OperationLog> operationLogRepository)
    {
        _operationLogRepository = operationLogRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<List<OperationLogDto>>> GetAll(
        [FromQuery] string? keyword = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _operationLogRepository.Query();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(l =>
                (l.UserName != null && l.UserName.Contains(keyword)) ||
                (l.RequestPath != null && l.RequestPath.Contains(keyword)) ||
                (l.ControllerName != null && l.ControllerName.Contains(keyword)) ||
                (l.ActionName != null && l.ActionName.Contains(keyword)));
        }

        query = query.OrderByDescending(l => l.CreatedAt);

        var total = query.Count();
        var logs = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var dtos = logs.Select(l => new OperationLogDto
        {
            Id = l.Id,
            OperationType = l.OperationType,
            ControllerName = l.ControllerName,
            ActionName = l.ActionName,
            RequestMethod = l.RequestMethod,
            RequestPath = l.RequestPath,
            RequestBody = l.RequestBody,
            StatusCode = l.StatusCode,
            ResponseBody = l.ResponseBody,
            IpAddress = l.IpAddress,
            UserId = l.UserId,
            UserName = l.UserName,
            ExecutionTime = l.DurationMs,
            CreatedAt = l.CreatedAt
        }).ToList();

        var response = new ApiResponse<List<OperationLogDto>>(dtos, total);
        response.Page = page;
        response.PageSize = pageSize;
        response.Total = total;

        return response;
    }
}
