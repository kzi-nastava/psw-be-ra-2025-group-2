using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class MonumentCommandTests : BaseToursIntegrationTest
{
    public MonumentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new MonumentDto
        {
            Name = "Pobednik",
            Description = "Simbol Beograda",
            YearOfCreation = 1928,
            State = "ACTIVE",
            Latitude = 44.78f,
            Longitude = 20.45f
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as MonumentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Description.ShouldBe(newEntity.Description);
        result.YearOfCreation.ShouldBe(newEntity.YearOfCreation);
        result.State.ShouldBe(newEntity.State);
        result.Latitude.ShouldBe(newEntity.Latitude);
        result.Longitude.ShouldBe(newEntity.Longitude);

        // Assert - Database
        var storedEntity = dbContext.Monument.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }
    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new MonumentDto
        {
            Description = "Test"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Create_fails_invalid_latitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new MonumentDto
        {
            Name = "Pobednik",
            Description = "Simbol Beograda",
            YearOfCreation = 1928,
            State = "ACTIVE",
            Latitude = -244.78f,
            Longitude = 20.45f
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Create_fails_invalid_longitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new MonumentDto
        {
            Name = "Pobednik",
            Description = "Simbol Beograda",
            YearOfCreation = 1928,
            State = "ACTIVE",
            Latitude = 44.78f,
            Longitude = -220.45f
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new MonumentDto
        {
            Id = -1,
            Name = "Spomenik Karađorđu",
            Description = "Test",
            YearOfCreation = 1913,
            State = "ACTIVE",
            Latitude = 44.78f,
            Longitude = 20.45f
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as MonumentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);
        result.YearOfCreation.ShouldBe(updatedEntity.YearOfCreation);
        result.State.ShouldBe(updatedEntity.State);
        result.Latitude.ShouldBe(updatedEntity.Latitude);
        result.Longitude.ShouldBe(updatedEntity.Longitude);

        // Assert - Database
        var storedEntity = dbContext.Monument.FirstOrDefault(i => i.Name == "Spomenik Karađorđu");
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        storedEntity.YearOfCreation.ShouldBe(updatedEntity.YearOfCreation);
        var oldEntity = dbContext.Monument.FirstOrDefault(i => i.Name == "Pobednik");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new MonumentDto
        {
            Id = -1000,
            Name = "Nepostojeci spomenik",
            Description = "Validan opis",
            YearOfCreation = 1900,
            State = "ACTIVE",
            Latitude = 44,
            Longitude = 20
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(-3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedCourse = dbContext.Monument.FirstOrDefault(i => i.Id == -3);
        storedCourse.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    private static MonumentController CreateController(IServiceScope scope)
    {
        return new MonumentController(scope.ServiceProvider.GetRequiredService<IMonumentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
