using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class ClubCommandTests : BaseStakeholdersIntegrationTest
{
    public ClubCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var newEntity = new ClubDto
        {
            Name = "Test klub",
            Description = "Opis test kluba",
            OwnerId = -21,
            ImageUrls = new List<string> { "test-image.jpg" }
        };

        // Act
        var actionResult = controller.Create(newEntity);
        var result = (actionResult.Result as OkObjectResult)?.Value as ClubDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);

        // Assert - Database
        var storedEntity = dbContext.Clubs.FirstOrDefault(c => c.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Name.ShouldBe(newEntity.Name);
    }


    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalid = new ClubDto
        {
            // nema Name, a očekujemo da servis baci grešku
            Description = "Opis bez imena"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalid));
    }

    [Fact]
    public void Updates()
    {
        long idToUpdate;
        
        using (var createScope = Factory.Services.CreateScope())
        {
            var createController = CreateController(createScope);

            var created = createController.Create(new ClubDto
            {
                Name = "Klub za izmenu",
                Description = "Originalni opis",
                OwnerId = -21,
                ImageUrls = new List<string> { "original.jpg" }
            });

            var createdResult = (created.Result as OkObjectResult)?.Value as ClubDto;
            createdResult.ShouldNotBeNull();
            createdResult.Id.ShouldNotBe(0);

            idToUpdate = createdResult.Id;
        }

        
        using (var updateScope = Factory.Services.CreateScope())
        {
            var controller = CreateController(updateScope);
            var dbContext = updateScope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updated = new ClubDto
            {
                Id = idToUpdate,
                Name = "Izmenjeni klub",
                Description = "Izmenjeni opis",
                OwnerId = -21,
                ImageUrls = new List<string> { "updated-image.jpg" }
            };

            // Act
            var actionResult = controller.Update(idToUpdate, updated);
            var result = (actionResult.Result as OkObjectResult)?.Value as ClubDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(idToUpdate);
            result.Name.ShouldBe(updated.Name);
            result.Description.ShouldBe(updated.Description);

            // Assert - Database
            var stored = dbContext.Clubs.FirstOrDefault(c => c.Id == idToUpdate);
            stored.ShouldNotBeNull();
            stored.Name.ShouldBe(updated.Name);
            stored.Description.ShouldBe(updated.Description);
        }
    }



    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updated = new ClubDto
        {
            Id = -1000,                     // ne postoji u bazi
            Name = "Nepostojeci klub",
            Description = "Opis nepostojeceg kluba",
            OwnerId = -21,                  // postoji u Users (turista1)
            ImageUrls = new List<string> { "nepostojeci.jpg" }  // bar jedna slika
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updated.Id, updated));
    }


    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var newEntity = new ClubDto
        {
            Name = "Klub za brisanje",
            Description = "Ovaj klub je kreiran u testu da bi bio obrisan.",
            OwnerId = -21,
            ImageUrls = new List<string> { "delete-me.jpg" }
        };

        // prvo GA KREIRAMO
        var createResult = (controller.Create(newEntity).Result as OkObjectResult)?.Value as ClubDto;
        createResult.ShouldNotBeNull();
        createResult.Id.ShouldNotBe(0);

        var idToDelete = createResult.Id;

        // Act - brišemo taj klub
        var deleteResult = controller.Delete(idToDelete) as OkResult;

        // Assert - Response
        deleteResult.ShouldNotBeNull();
        deleteResult.StatusCode.ShouldBe(200);

        // Assert - Database
        var stored = dbContext.Clubs.FirstOrDefault(c => c.Id == idToDelete);
        stored.ShouldBeNull();
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

    private static ClubsController CreateController(IServiceScope scope)
    {
        return new ClubsController(scope.ServiceProvider.GetRequiredService<IClubService>())
        {
            
            ControllerContext = BuildContext("-21")
        };
    }
}
