using MaterialManageSystem.Core.Entities;

namespace MaterialManageSystem.Tests;

public class PasswordHashTests
{
    [Fact]
    public void BCryptHash_CanVerifyCorrectPassword()
    {
        // Arrange
        var password = "Admin@123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Act
        var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void BCryptHash_RejectsIncorrectPassword()
    {
        // Arrange
        var password = "Admin@123";
        var wrongPassword = "WrongPassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Act
        var isValid = BCrypt.Net.BCrypt.Verify(wrongPassword, hashedPassword);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void BCryptHash_DifferentPasswords_ProduceDifferentHashes()
    {
        // Arrange
        var password1 = "Password1";
        var password2 = "Password2";

        // Act
        var hash1 = BCrypt.Net.BCrypt.HashPassword(password1);
        var hash2 = BCrypt.Net.BCrypt.HashPassword(password2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void BCryptHash_SamePassword_ProducesDifferentHashes_ForSecurity()
    {
        // Arrange
        var password = "SamePassword";

        // Act
        var hash1 = BCrypt.Net.BCrypt.HashPassword(password);
        var hash2 = BCrypt.Net.BCrypt.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, hash1));
        Assert.True(BCrypt.Net.BCrypt.Verify(password, hash2));
    }

    [Fact]
    public void User_PasswordHash_CanBeSetAndVerified()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123")
        };

        // Act
        var isValid = BCrypt.Net.BCrypt.Verify("TestPassword123", user.PasswordHash);

        // Assert
        Assert.True(isValid);
    }
}
