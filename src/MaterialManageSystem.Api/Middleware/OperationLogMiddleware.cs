using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Http;

namespace MaterialManageSystem.Api.Middleware;

public class OperationLogMiddleware
{
    private readonly RequestDelegate _next;

    public OperationLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, MaterialDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var log = new OperationLog
        {
            RequestMethod = context.Request.Method,
            RequestPath = context.Request.Path,
            IpAddress = GetClientIpAddress(context),
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            UserName = context.User.FindFirst(ClaimTypes.Name)?.Value,
            CreatedAt = DateTime.Now
        };

        string? requestBody = null;
        if (context.Request.ContentLength > 0 && context.Request.ContentType?.Contains("application/json") == true)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            log.RequestBody = requestBody.Length > 2000 ? requestBody.Substring(0, 2000) : requestBody;
        }

        var originalResponseBody = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);
            
            log.StatusCode = context.Response.StatusCode;
            log.OperationType = GetOperationType(context);
            log.ControllerName = GetControllerName(context);
            log.ActionName = GetActionName(context);

            responseBodyStream.Position = 0;
            using var reader = new StreamReader(responseBodyStream);
            var responseBody = await reader.ReadToEndAsync();
            log.ResponseBody = responseBody.Length > 2000 ? responseBody.Substring(0, 2000) : responseBody;

            responseBodyStream.Position = 0;
            await responseBodyStream.CopyToAsync(originalResponseBody);
        }
        catch
        {
            log.StatusCode = 500;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            log.DurationMs = stopwatch.ElapsedMilliseconds;
            context.Response.Body = originalResponseBody;

            await dbContext.OperationLogs.AddAsync(log);
            await dbContext.SaveChangesAsync();
        }
    }

    private string GetClientIpAddress(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Connection.RemoteIpAddress?.ToString();
        }
        return ip ?? string.Empty;
    }

    private string GetOperationType(HttpContext context)
    {
        return context.Request.Method switch
        {
            "POST" => "Create",
            "PUT" => "Update",
            "DELETE" => "Delete",
            "GET" => "Query",
            _ => "Unknown"
        };
    }

    private string GetControllerName(HttpContext context)
    {
        return context.GetRouteValue("controller")?.ToString() ?? string.Empty;
    }

    private string GetActionName(HttpContext context)
    {
        return context.GetRouteValue("action")?.ToString() ?? string.Empty;
    }
}