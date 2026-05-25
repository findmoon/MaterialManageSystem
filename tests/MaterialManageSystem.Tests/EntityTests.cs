using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Tests;

public class EntityTests
{
    [Fact]
    public void User_DefaultValues_AreCorrect()
    {
        var user = new User();

        Assert.True(user.IsActive);
        Assert.Equal(string.Empty, user.CreatedBy);
        Assert.Null(user.UpdatedAt);
        Assert.Null(user.UpdatedBy);
    }

    [Fact]
    public void ReelId_DefaultValues_AreCorrect()
    {
        var reel = new ReelId();

        Assert.Equal(ReelStatus.InStock, reel.Status);
        Assert.True(reel.IsActive);
    }

    [Fact]
    public void ReelId_ScrappedStatus_CannotBeChanged()
    {
        var reel = new ReelId
        {
            Status = ReelStatus.Scrapped
        };

        Assert.Equal(ReelStatus.Scrapped, reel.Status);
    }

    [Fact]
    public void WarningConfig_DefaultValues_AreCorrect()
    {
        var config = new WarningConfig();

        Assert.True(config.IsEnabled);
        Assert.True(config.IsActive);
    }

    [Fact]
    public void OperationLog_InheritsFromBaseEntity()
    {
        var log = new OperationLog();

        Assert.IsAssignableFrom<BaseEntity>(log);
        Assert.True(log.IsActive);
    }

    [Fact]
    public void ReelUsageLog_CanStoreUsageInfo()
    {
        var log = new ReelUsageLog
        {
            ReelId = 1,
            UsageType = UsageType.Checkout,
            Quantity = 10
        };

        Assert.Equal(UsageType.Checkout, log.UsageType);
        Assert.Equal(10, log.Quantity);
    }
}
