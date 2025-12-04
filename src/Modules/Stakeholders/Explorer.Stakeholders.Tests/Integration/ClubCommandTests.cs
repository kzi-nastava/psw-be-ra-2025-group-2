using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Explorer.Stakeholders.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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

        // "ulogovani" korisnik (turista) sa ID -21
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

        var newEntity = new ClubDto
        {
            Name = "Test klub",
            Description = "Opis test kluba",
            // OwnerId više ne moraš da setuješ, kontroler ga ignoriše i postavlja na userId
            ImageUrls = new List<string> { "test-image.jpg" }
        };

        // Act
        var actionResult = controller.Create(newEntity);
        var result = (actionResult.Result as OkObjectResult)?.Value as ClubDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.OwnerId.ShouldBe(userId);  // vlasnik je ulogovani korisnik

        // Assert - Database
        var storedEntity = dbContext.Clubs.FirstOrDefault(c => c.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Name.ShouldBe(newEntity.Name);
        storedEntity.OwnerId.ShouldBe(userId);
    }


    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // >>> Dodajemo "ulogovanog" korisnika da GetCurrentUserId ne puca
        var userId = -21; // neki postojeći turista iz test podataka
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

        var invalid = new ClubDto
        {
            // nema Name, a očekujemo da servis/entitet baci grešku
            Description = "Opis bez imena"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalid));
    }


[Fact]
public void Updates()
{
    long idToUpdate;
    var userId = -21;   // turista koji je vlasnik

    // 1) Kreiranje kluba
    using (var createScope = Factory.Services.CreateScope())
    {
        var createController = CreateController(createScope);

        // Ulogovani korisnik za create
        createController.ControllerContext = new ControllerContext
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

        var created = createController.Create(new ClubDto
        {
            Name = "Klub za izmenu",
            Description = "Originalni opis",
            // OwnerId NE moraš da šalješ – kontroler ga postavlja na userId
            ImageUrls = new List<string> { "original.jpg" }
        });

        var createdResult = (created.Result as OkObjectResult)?.Value as ClubDto;
        createdResult.ShouldNotBeNull();
        createdResult.Id.ShouldNotBe(0);
        createdResult.OwnerId.ShouldBe(userId);

        idToUpdate = createdResult.Id;
    }

    // 2) Izmena istog kluba kao isti korisnik
    using (var updateScope = Factory.Services.CreateScope())
    {
        var controller = CreateController(updateScope);
        var dbContext = updateScope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Ulogovani korisnik za update (isti vlasnik)
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

        var updated = new ClubDto
        {
            Id = idToUpdate,
            Name = "Izmenjeni klub",
            Description = "Izmenjeni opis",
            // OwnerId se opet ignoriše u kontroleru, ali može da stoji
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
        result.OwnerId.ShouldBe(userId);

        // Assert - Database
        var stored = dbContext.Clubs.FirstOrDefault(c => c.Id == idToUpdate);
        stored.ShouldNotBeNull();
        stored.Name.ShouldBe(updated.Name);
        stored.Description.ShouldBe(updated.Description);
        stored.OwnerId.ShouldBe(userId);
    }
}




    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // "ulogovani" korisnik (turista)
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

        var updated = new ClubDto
        {
            Id = -1000,                     // ne postoji u bazi
            Name = "Nepostojeci klub",
            Description = "Opis nepostojeceg kluba",
            // OwnerId ne moraš da setuješ – kontroler ga postavlja na userId
            ImageUrls = new List<string> { "nepostojeci.jpg" }
        };

        // Act & Assert
        // _clubService.Get(-1000) -> repo baca NotFoundException
        Should.Throw<NotFoundException>(() => controller.Update(updated.Id, updated));
    }



    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // "ulogovani" korisnik – vlasnik kluba
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

        var newEntity = new ClubDto
        {
            Name = "Klub za brisanje",
            Description = "Ovaj klub je kreiran u testu da bi bio obrisan.",
            // OwnerId više ne moraš da setaš – kontroler ga postavlja na userId
            ImageUrls = new List<string> { "delete-me.jpg" }
        };

        // prvo ga KREIRAMO
        var createActionResult = controller.Create(newEntity);
        var created = (createActionResult.Result as OkObjectResult)?.Value as ClubDto;

        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(0);
        created.OwnerId.ShouldBe(userId);

        var idToDelete = created.Id;

        // Act - brišemo taj klub
        var deleteActionResult = controller.Delete(idToDelete);
        var deleteResult = deleteActionResult as NoContentResult; // jer Delete sada radi return NoContent();

        // Assert - Response
        deleteResult.ShouldNotBeNull();
        deleteResult.StatusCode.ShouldBe(204);

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

        // "ulogovani" korisnik (turista), bitan je samo da postoji claim "id"
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

        // Act & Assert
        // -1000 ne postoji → _clubService.Get(id) → repo baca NotFoundException
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    private static ClubsController CreateController(IServiceScope scope)
    {
        return new ClubsController(scope.ServiceProvider.GetRequiredService<IClubService>())
        {
            
            ControllerContext = BuildContext("-21")
        };
    }

    [Fact]
    public void Invite_and_accept_invitation_adds_member_and_removes_invitation()
    {
        // Arrange
        var club = new Club(
            name: "Test club",
            description: "Some description",
            ownerId: 1,
            imageUrls: new List<string> { "https://example.com/image.jpg" });

        var touristId = 10L;

        // Act 1: vlasnik šalje poziv
        club.InviteTourist(touristId);

        // Assert posle poziva
        Assert.Single(club.Invitations);
        Assert.Empty(club.Members);

        // Act 2: turista prihvata poziv
        club.AcceptInvitation(touristId);

        // Assert posle prihvatanja
        Assert.Empty(club.Invitations);
        Assert.Single(club.Members);
        Assert.Equal(touristId, club.Members.Single().TouristId);
    }

    [Fact]
    public void Invite_fails_when_tourist_is_already_member()
    {
        // Arrange
        var club = new Club(
            name: "Test club",
            description: "Desc",
            ownerId: 1,
            imageUrls: new List<string> { "img" });

        var touristId = 10L;

        // prvo vlasnik pošalje poziv
        club.InviteTourist(touristId);
        // turista prihvati poziv -> postaje član
        club.AcceptInvitation(touristId);

        // Act & Assert: sada je već član i novi poziv treba da padne
        Assert.Throws<InvalidOperationException>(() =>
            club.InviteTourist(touristId));
    }

    [Fact]
    public void Accept_invitation_fails_when_invitation_does_not_exist()
    {
        // Arrange
        var club = new Club(
            name: "Test club",
            description: "Desc",
            ownerId: 1,
            imageUrls: new List<string> { "img" });

        var touristId = 10L;

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            club.AcceptInvitation(touristId));

        Assert.Equal("Invitation not found.", ex.Message);
    }

    [Fact]
    public void Remove_member_removes_existing_member()
    {
        // Arrange
        var club = new Club(
            name: "Test club",
            description: "Desc",
            ownerId: 1,
            imageUrls: new List<string> { "img" });

        var touristId = 10L;

        // napravi člana preko domenske logike
        club.InviteTourist(touristId);
        club.AcceptInvitation(touristId);

        Assert.Single(club.Members);

        // Act
        club.RemoveMember(touristId);

        // Assert
        Assert.Empty(club.Members);
    }
}
