using System.Collections.Generic;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Microsoft.AspNetCore.Mvc;

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

       
        var createdResult = controller.Create(new ClubDto
        {
            Name = "Klub za pretragu",
            Description = "Opis kluba za test Retrieves_all",
            OwnerId = -21,
            ImageUrls = new List<string> { "retrieves-all.jpg" }
        });

        var created = (createdResult.Result as OkObjectResult)?.Value as ClubDto;
        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(0);

       
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