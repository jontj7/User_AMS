using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using User_AMS.Controllers;
using User_AMS.Data;
using User_AMS.DTOs;
using User_AMS.Models;
using User_AMS.Services;
using Xunit;

public class AuthControllerTests
{
    private readonly LoginDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly Mock<IJwtService> _jwtMock;

    public AuthControllerTests()
    {
        _db = TestDbContextFactory.Create();
        _hasher = new PasswordHasher();
        _jwtMock = new Mock<IJwtService>();
    }

    [Fact(DisplayName = "Debe devolver un token cuando las credenciales son válidas")]
    public async Task Login_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var role = new Role { Name = "Administrator" };
        var status = new Status { Name = "Active" };
        _db.Roles.Add(role);
        _db.Statuses.Add(status);
        await _db.SaveChangesAsync();

        var user = new User
        {
            Username = "testuser",
            PasswordHash = _hasher.Hash("password123"),
            RoleId = role.Id,
            StatusId = status.Id
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _jwtMock.Setup(j => j.CreateToken(It.IsAny<User>(), "Administrator"))
                .Returns("fake-jwt-token");

        var controller = new AuthController(_db, _hasher, _jwtMock.Object);
        var request = new LoginRequest("testuser", "password123");

        // Act
        var result = await controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal("fake-jwt-token", response.Token);
    }
}
