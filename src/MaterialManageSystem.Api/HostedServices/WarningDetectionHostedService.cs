using System;
using System.Threading;
using System.Threading.Tasks;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MaterialManageSystem.Api.HostedServices;

public class WarningDetectionHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WarningDetectionHostedService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

    public WarningDetectionHostedService(
        IServiceProvider serviceProvider,
        ILogger<WarningDetectionHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("预警检测服务已启动");

        // 初始延迟，让应用程序完全启动
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("开始执行预警检测...");
                
                using var scope = _serviceProvider.CreateScope();
                var detectionService = scope.ServiceProvider.GetRequiredService<IWarningDetectionService>();
                await detectionService.DetectAndCreateWarningsAsync();

                // 如果有新警告，通过 SignalR 推送
                var recordRepository = scope.ServiceProvider.GetRequiredService<IWarningRecordRepository>();
                var activeWarnings = await recordRepository.GetActiveWarningsAsync();

                if (activeWarnings.Any())
                {
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<WarningHub>>();
                    await hubContext.Clients.All.SendAsync("ReceiveWarning", activeWarnings, stoppingToken);
                }

                _logger.LogInformation("预警检测完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "预警检测过程中发生错误");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("预警检测服务已停止");
    }
}
