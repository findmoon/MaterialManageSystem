using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MaterialManageSystem.Api.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _limit;
    private readonly TimeSpan _period;
    private readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();

    public RateLimitMiddleware(RequestDelegate next, int limit = 100, int periodMinutes = 1)
    {
        _next = next;
        _limit = limit;
        _period = TimeSpan.FromMinutes(periodMinutes);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = GetClientIp(context);
        var key = $"{clientIp}:{context.Request.Path}";

        var now = DateTime.UtcNow;
        var rateLimitInfo = _rateLimitStore.GetOrAdd(key, _ => new RateLimitInfo(now));

        bool isRateLimited;
        lock (rateLimitInfo)
        {
            if (now - rateLimitInfo.WindowStart > _period)
            {
                rateLimitInfo.Reset(now);
            }

            rateLimitInfo.RequestCount++;
            isRateLimited = rateLimitInfo.RequestCount > _limit;
        }

        if (isRateLimited)
        {
            context.Response.StatusCode = 429;
            context.Response.Headers["Retry-After"] = ((int)_period.TotalSeconds).ToString();
            await context.Response.WriteAsJsonAsync(new
            {
                Code = 429,
                Message = "请求过于频繁，请稍后重试",
                Data = (object?)null
            });
            return;
        }

        await _next(context);
    }

    private string GetClientIp(HttpContext context)
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
        return ip ?? "unknown";
    }

    private class RateLimitInfo
    {
        public DateTime WindowStart { get; private set; }
        public int RequestCount { get; set; }

        public RateLimitInfo(DateTime windowStart)
        {
            WindowStart = windowStart;
            RequestCount = 0;
        }

        public void Reset(DateTime newWindowStart)
        {
            WindowStart = newWindowStart;
            RequestCount = 0;
        }
    }
}