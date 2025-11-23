using System.Collections.Generic;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class ClubQueryTests : BaseStakeholdersIntegrationTest
{
    public ClubQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // "Ulogovani" turista
        var userId = -21;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                    {
                        new Claim("id", userId.ToString()),
                        new Claim(ClaimTypes.Role, "tourist")
                    }, "TestAuth"))
            }
        };

        var createdResult = controller.Create(new ClubDto
        {
            Name = "Klub za pretragu",
            Description = "Opis kluba za test Retrieves_all",
            // OwnerId ne moraš, kontroler ga postavlja na userId iz tokena
            ImageUrls = new List<string> { "retrieves-all.jpg" }
        });

        var created = (createdResult.Result as OkObjectResult)?.Value as ClubDto;
        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(0);

        // Act
        var actionResult = controller.GetAll();
        var okResult = actionResult.Result as OkObjectResult;
        var result = okResult?.Value as List<ClubDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);   // ima bar jedan klub

        // proverimo da je baš taj koji smo kreirali u listi
        result.Any(c => c.Id == created.Id && c.Name == created.Name).ShouldBeTrue();
    }



    private static ClubsController CreateController(IServiceScope scope)
    {
        return new ClubsController(scope.ServiceProvider.GetRequiredService<IClubService>())
        {
            ControllerContext = BuildContext("-21")
        };
    }
}