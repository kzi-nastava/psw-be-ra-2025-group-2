using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Controllers;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminUserControllerTests : BaseStakeholdersIntegrationTest
{
    public AdminUserControllerTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void CreateAccount_ShouldCreateUserSuccessfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new AccountRegistrationDto
        {
            Username = "new_author",
            Password = "Password123!",
            Email = "new_author@example.com",
            Role = "Author",
            Name = "Test",
            Surname = "Author"
        };

        var result = controller.CreateAccount(dto);

        result.ShouldBeOfType<OkObjectResult>();

        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var user = dbContext.Users.FirstOrDefault(u => u.Username == "new_author");
        user.ShouldNotBeNull();
        user.Role.ShouldBe(UserRole.Author);
        user.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void BlockUser_ShouldSetIsActiveToFalse()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var testUser = new User("block_test", "Password!", UserRole.Author, true);
        dbContext.Users.Add(testUser);
        dbContext.SaveChanges();

        var result = controller.BlockUser("block_test");

        result.ShouldBeOfType<OkObjectResult>();
        var user = dbContext.Users.First(u => u.Username == "block_test");
        user.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void UnblockUser_ShouldSetIsActiveToTrue()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Insert inactive user with negative ID
        var testUser = new User("unblock_test", "Password!", UserRole.Author, false);
        dbContext.Users.Add(testUser);
        dbContext.SaveChanges();

        // Act
        var result = controller.UnblockUser("unblock_test");

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var user = dbContext.Users.First(u => u.Username == "unblock_test");
        user.IsActive.ShouldBeTrue();
    }

    private static AdminUserController CreateController(IServiceScope scope)
    {
        // Use the interface, which is registered in DI
        var service = scope.ServiceProvider.GetRequiredService<IAdminUserService>();
        return new AdminUserController(service);
    }
}
