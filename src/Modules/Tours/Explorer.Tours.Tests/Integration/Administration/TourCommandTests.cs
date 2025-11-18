using System.Security.Claims;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourCommandTests : BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newEntity = new CreateTourDto
        {
            Name = "Đerdap Exploration",
            Description = "Istraživanje Đerdapske klisure i nacionalnog parka",
            Difficulty = 3,
            AuthorId = -11,
            Tags = new List<string> { "priroda", "nacionalni park", "reka" }
        };

        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Description.ShouldBe(newEntity.Description);
        result.Difficulty.ShouldBe(newEntity.Difficulty);
        result.Status.ShouldBe("Draft");
        result.Price.ShouldBe(0);
        result.Tags.Count.ShouldBe(3);

        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.AuthorId.ShouldBe(-11);
    }

    [Fact]
    public void Create_fails_invalid_name()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var newEntity = new CreateTourDto
        {
            Name = "",
            Description = "Test description",
            Difficulty = 3,
            AuthorId = -11
        };

        Should.Throw<ArgumentNullException>(() => controller.Create(newEntity));
    }

    [Fact]
    public void Create_fails_invalid_difficulty()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var newEntity = new CreateTourDto
        {
            Name = "Test Tour",
            Description = "Test description",
            Difficulty = 6, // Invalid
            AuthorId = -11
        };

        Should.Throw<ArgumentOutOfRangeException>(() => controller.Create(newEntity));
    }

    [Fact]
    public void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updatedEntity = new UpdateTourDto
        {
            Name = "Fruška Gora Premium",
            Description = "Proširena verzija ture sa više vidikovaca",
            Difficulty = 4,
            Tags = new List<string> { "priroda", "planinarenje", "premium" }
        };

        var result = ((ObjectResult)controller.Update(-11, updatedEntity).Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-11);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);
        result.Difficulty.ShouldBe(4);
        result.Tags.Count.ShouldBe(3);

        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Id == -11);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe("Fruška Gora Premium");
        storedEntity.Difficulty.ShouldBe(4);

        var oldEntity = dbContext.Tours.FirstOrDefault(t => t.Name == "Fruška Gora Adventure");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var updatedEntity = new UpdateTourDto
        {
            Name = "Test Tour",
            Description = "Test",
            Difficulty = 3
        };

        Should.Throw<Exception>(() => controller.Update(-1000, updatedEntity));
    }

    [Fact]
    public void Update_fails_invalid_difficulty()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var updatedEntity = new UpdateTourDto
        {
            Name = "Updated Tour",
            Description = "Updated description",
            Difficulty = 0
        };

        Should.Throw<ArgumentOutOfRangeException>(() => controller.Update(-11, updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var result = controller.Delete(-13) as NoContentResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Id == -13);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        Should.Throw<Exception>(() => controller.Delete(-1000));
    }

    [Fact]
    public void Delete_fails_published_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        Should.Throw<Exception>(() => controller.Delete(-12)); // assuming -12 is published
    }

    private static TourController CreateController(IServiceScope scope, string authorId = "-11")
    {
        var controller = new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                    new Claim("id", authorId) // ovde ide ID autora koji test koristi
                    }, "TestAuth"))
                }
            }
        };
        return controller;
    }

}
