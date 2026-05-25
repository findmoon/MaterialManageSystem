using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Enums;
using MaterialManageSystem.Core.Interfaces;
using Moq;

namespace MaterialManageSystem.Tests;

public class ServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IReelIdRepository> _mockReelRepo;

    public ServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockReelRepo = new Mock<IReelIdRepository>();
    }

    [Fact]
    public async Task UserRepository_GetByUsername_ReturnsUser()
    {
        // Arrange
        var expectedUser = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "hashedpassword"
        };
        _mockUserRepo.Setup(r => r.GetByUsernameAsync("admin"))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _mockUserRepo.Object.GetByUsernameAsync("admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("admin", result.Username);
        _mockUserRepo.Verify(r => r.GetByUsernameAsync("admin"), Times.Once);
    }

    [Fact]
    public async Task UserRepository_GetByUsername_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByUsernameAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _mockUserRepo.Object.GetByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ReelIdRepository_GetByPartNoId_ReturnsReels()
    {
        // Arrange
        var reels = new List<ReelId>
        {
            new ReelId
            {
                Id = 1,
                ReelNo = "R001",
                Status = ReelStatus.InStock
            },
            new ReelId
            {
                Id = 2,
                ReelNo = "R002",
                Status = ReelStatus.InStock
            }
        };
        _mockReelRepo.Setup(r => r.GetByPartNoIdAsync(1))
            .ReturnsAsync(reels);

        // Act
        var result = await _mockReelRepo.Object.GetByPartNoIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task ReelIdRepository_GetByReelNo_ReturnsCorrectReel()
    {
        // Arrange
        var expectedReel = new ReelId
        {
            Id = 1,
            ReelNo = "R001",
            PartNoId = 1
        };
        _mockReelRepo.Setup(r => r.GetByReelNoAsync("R001"))
            .ReturnsAsync(expectedReel);

        // Act
        var result = await _mockReelRepo.Object.GetByReelNoAsync("R001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("R001", result.ReelNo);
    }

    [Fact]
    public async Task ReelIdRepository_GetByReelNo_ReturnsNull_WhenNotFound()
    {
        // Arrange
        _mockReelRepo.Setup(r => r.GetByReelNoAsync("NONEXISTENT"))
            .ReturnsAsync((ReelId?)null);

        // Act
        var result = await _mockReelRepo.Object.GetByReelNoAsync("NONEXISTENT");

        // Assert
        Assert.Null(result);
    }
}
