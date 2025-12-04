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
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 1) kreiramo novu turu sa osnovnim podacima
        var newTour = new CreateTourDto
        {
            Name = "Test Publish Tour",
            Description = "Tura za testiranje objave",
            Difficulty = 3,
            AuthorId = -11,
            Tags = new List<string> { "test", "publish" }
            // KeyPoints ovde ignorise servis ionako, pa ih ne moramo slati
        };

        var created = ((ObjectResult)controller.Create(newTour).Result)?.Value as TourDto;
        created.ShouldNotBeNull();

        // 2) dodamo dve ključne tačke preko endpointa
        var firstKeyPoint = new KeyPointDto
        {
            OrdinalNo = 1,
            Name = "Prva tačka",
            Description = "Opis 1",
            SecretText = "Tajna 1",
            ImageUrl = "img1.jpg",
            Latitude = 45.0,
            Longitude = 19.0
        };

        var secondKeyPoint = new KeyPointDto
        {
            OrdinalNo = 2,
            Name = "Druga tačka",
            Description = "Opis 2",
            SecretText = "Tajna 2",
            ImageUrl = "img2.jpg",
            Latitude = 45.001,
            Longitude = 19.001
        };

        controller.AddKeyPoint(created.Id, firstKeyPoint);
        controller.AddKeyPoint(created.Id, secondKeyPoint);

        // Act – sad pokušavamo da objavimo
        var publishResult = controller.Publish(created.Id) as NoContentResult;

        // Assert
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

        // Act & Assert – pokušaj objave kao drugi autor
        Should.Throw<UnauthorizedAccessException>(() => controllerAsAuthor12.Publish(created.Id));
    }

    [Fact]
    public void Publish_fails_for_non_existing_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        Should.Throw<Exception>(() => controller.Publish(-9999));
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
            KeyPoints = new List<KeyPointDto>   // samo jedna tačka → treba 2
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
            Durations = new List<TourDurationDto>() // prazno → nema trajanja
        };

        var created = ((ObjectResult)controller.Create(invalidTour).Result)?.Value as TourDto;
        created.ShouldNotBeNull();

        // Publish treba da baci InvalidOperationException iz agregata
        Should.Throw<InvalidOperationException>(() => controller.Publish(created.Id));
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
