using System.Security.Claims;
using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourPublishCommandTests : BaseToursIntegrationTest
{
    public TourPublishCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Publish_succeeds_for_valid_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newTour = new CreateTourDto
        {
            Name = "Test Publish Tour",
            Description = "Tura za testiranje objave",
            Difficulty = 3,
            AuthorId = -11,
            Tags = new List<string> { "test", "publish" },

            Durations = new List<TourDurationDto>
            {
                new TourDurationDto
                {
                    TransportType = 0, // Walking
                    Minutes = 120
                }
            }
        };

        var created = ((ObjectResult)controller.Create(newTour).Result)?.Value as TourDto;
        created.ShouldNotBeNull();

        controller.AddKeyPoint(created.Id, new KeyPointDto
        {
            OrdinalNo = 1,
            Name = "Prva tačka",
            Description = "Opis 1",
            SecretText = "Tajna 1",
            ImageUrl = "img1.jpg",
            Latitude = 45.0,
            Longitude = 19.0
        });

        controller.AddKeyPoint(created.Id, new KeyPointDto
        {
            OrdinalNo = 2,
            Name = "Druga tačka",
            Description = "Opis 2",
            SecretText = "Tajna 2",
            ImageUrl = "img2.jpg",
            Latitude = 45.001,
            Longitude = 19.001
        });

        var publishResult = controller.Publish(created.Id) as NoContentResult;

        publishResult.ShouldNotBeNull();
        publishResult.StatusCode.ShouldBe(204);

        var stored = dbContext.Tours.FirstOrDefault(t => t.Id == created.Id);
        stored.ShouldNotBeNull();
        stored!.Status.ShouldBe(TourStatus.Published);
        stored.PublishedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Publish_fails_if_user_is_not_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controllerAsAuthor11 = CreateController(scope, "-11");
        var controllerAsAuthor12 = CreateController(scope, "-12"); // drugi autor

        var newTour = new CreateTourDto
        {
            Name = "NotOwnedTour",
            Description = "Tura čiji autor je -11",
            Difficulty = 2,
            AuthorId = -11,
            Tags = new List<string> { "tag" },
            KeyPoints = new List<KeyPointDto>
            {
                new KeyPointDto
                {
                    OrdinalNo = 1,
                    Name = "Tačka 1",
                    Description = "Opis",
                    SecretText = "Tajna",
                    ImageUrl = "img.jpg",
                    Latitude = 45.0,
                    Longitude = 19.0
                },
                new KeyPointDto
                {
                    OrdinalNo = 2,
                    Name = "Tačka 2",
                    Description = "Opis 2",
                    SecretText = "Tajna 2",
                    ImageUrl = "img2.jpg",
                    Latitude = 45.001,
                    Longitude = 19.001
                }
            },
            Durations = new List<TourDurationDto>
            {
                new TourDurationDto
                {
                    TransportType = TransportTypeDto.Walking,
                    Minutes = 60
                }
            }
        };

        var created = ((ObjectResult)controllerAsAuthor11.Create(newTour).Result)?.Value as TourDto;
        created.ShouldNotBeNull();

        var result = controllerAsAuthor12.Publish(created.Id);

        result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public void Publish_fails_for_non_existing_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Act
        var result = controller.Publish(-9999) as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(500); 
    }

    [Fact]
    public void Publish_fails_when_tour_violates_business_rules()
    {
        // Npr. nema dovoljno ključnih tačaka ili nema durations
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var invalidTour = new CreateTourDto
        {
            Name = "Invalid Publish Tour",
            Description = "Nema dovoljno keypoint-a ili nema durations",
            Difficulty = 3,
            AuthorId = -11,
            Tags = new List<string> { "tag" },
            KeyPoints = new List<KeyPointDto>  
            {
                new KeyPointDto
                {
                    OrdinalNo = 1,
                    Name = "Samo jedna tačka",
                    Description = "Opis",
                    SecretText = "Tajna",
                    ImageUrl = "img.jpg",
                    Latitude = 45.0,
                    Longitude = 19.0
                }
            },
            Durations = new List<TourDurationDto>() 
        };

        var created = ((ObjectResult)controller.Create(invalidTour).Result)?.Value as TourDto;
        created.ShouldNotBeNull();

        var result = controller.Publish(created.Id) as BadRequestObjectResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
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
                        new Claim("id", authorId)
                    }, "TestAuth"))
                }
            }
        };
        return controller;
    }
}
