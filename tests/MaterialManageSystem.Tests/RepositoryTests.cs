using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Enums;
using MaterialManageSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaterialManageSystem.Tests;

public class RepositoryTests : IDisposable
{
    private readonly MaterialDbContext _context;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MaterialDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MaterialDbContext(options);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task UserRepository_GetByUsernameAsync_ReturnsCorrectUser()
    {
        // Arrange
        var employee = new Employee
        {
            EmployeeNo = "E001",
            Name = "Test User",
            CreatedBy = "System"
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var user = new User
        {
            EmployeeId = employee.Id,
            Username = "testuser",
            PasswordHash = "hash123",
            CreatedBy = "System"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task ReelIdRepository_GetByReelNoAsync_ReturnsCorrectReel()
    {
        // Arrange
        var reel = new ReelId
        {
            ReelNo = "R001",
            PartNoId = 1,
            Status = ReelStatus.InStock,
            CreatedBy = "System"
        };
        _context.ReelIds.Add(reel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.ReelIds
            .FirstOrDefaultAsync(r => r.ReelNo == "R001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ReelStatus.InStock, result.Status);
    }

    [Fact]
    public async Task WarningConfigRepository_GetEnabledConfigs_ReturnsOnlyEnabled()
    {
        // Arrange
        var config1 = new WarningConfig
        {
            WarningType = WarningType.Quantity,
            IsEnabled = true,
            CreatedBy = "System"
        };
        var config2 = new WarningConfig
        {
            WarningType = WarningType.Time,
            IsEnabled = false,
            CreatedBy = "System"
        };
        _context.WarningConfigs.AddRange(config1, config2);
        await _context.SaveChangesAsync();

        // Act
        var enabledConfigs = await _context.WarningConfigs
            .Where(c => c.IsEnabled)
            .ToListAsync();

        // Assert
        Assert.Single(enabledConfigs);
        Assert.Equal(WarningType.Quantity, enabledConfigs[0].WarningType);
    }

    [Fact]
    public async Task OperationLog_CanBeAddedAndRetrieved()
    {
        // Arrange
        var log = new OperationLog
        {
            OperationType = "Login",
            ControllerName = "Auth",
            ActionName = "Login",
            RequestMethod = "POST",
            RequestPath = "/api/auth/login",
            StatusCode = 200,
            IpAddress = "127.0.0.1",
            DurationMs = 50,
            CreatedBy = "System"
        };
        _context.OperationLogs.Add(log);
        await _context.SaveChangesAsync();

        // Act
        var retrievedLog = await _context.OperationLogs
            .FirstOrDefaultAsync(l => l.OperationType == "Login");

        // Assert
        Assert.NotNull(retrievedLog);
        Assert.Equal("Auth", retrievedLog.ControllerName);
        Assert.Equal(200, retrievedLog.StatusCode);
    }
}
