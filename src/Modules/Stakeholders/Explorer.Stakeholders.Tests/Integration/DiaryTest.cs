using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.API.Controllers.Tourist;
namespace Explorer.Stakeholders.Tests.Integration.Diaries;
[Collection("Sequential")]
public class DiaryTests : BaseStakeholdersIntegrationTest
{
    public DiaryTests(StakeholdersTestFactory factory) : base(factory) { }
    [Fact]
    public void Successfully_creates_diary()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);
        var diary = new DiaryDto
        {
            Id = -100,
            UserId = -1,
            Name = "Test Diary",
            CreatedAt = DateTime.UtcNow,
            Status = "1",
            Country = "Serbia",
            City = "Belgrade"
        };
        // Act
        var result = ((ObjectResult)controller.Create(diary).Result).Value as DiaryDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-100);
        result.Name.ShouldBe("Test Diary");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedDiary = dbContext.Diaries.FirstOrDefault(d => d.Id == -100);
        storedDiary.ShouldNotBeNull();
        storedDiary.Name.ShouldBe("Test Diary");
        storedDiary.UserId.ShouldBe(-1);
    }


    [Fact]
    public void Successfully_deletes_diary()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        // Act
        var result = controller.Delete(-100) as OkResult;

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedDiary = dbContext.Diaries.FirstOrDefault(d => d.Id == -100);
        storedDiary.ShouldBeNull();
    }

    private static DiaryController CreateController(IServiceScope scope)
    {
      return new DiaryController(scope.ServiceProvider.GetRequiredService<IDiaryService>());
    }


}

