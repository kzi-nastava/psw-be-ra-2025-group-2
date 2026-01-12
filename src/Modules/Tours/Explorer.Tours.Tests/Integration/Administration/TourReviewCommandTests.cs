using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourReviewCommandTests : BaseToursIntegrationTest
{
    public TourReviewCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 1. Fetch the Execution
        var execution = dbContext.TourExecutions
            .FirstOrDefault(e => e.TouristId == -23 && e.TourId == -1);

        if (execution != null)
        {
            // A) Fix "7 days" rule
            execution.RecordActivity();

            // B) Fix "35% progress" rule via Reflection
            var visitsField = typeof(TourExecution).GetField("_keyPointVisits",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (visitsField != null)
            {
                var visits = (List<KeyPointVisit>)visitsField.GetValue(execution);
                visits.Clear();

                // Ensure we add at least one visit if count is 0, otherwise fill all
                int countToFill = execution.KeyPointsCount > 0 ? execution.KeyPointsCount : 5;

                for (int i = 1; i <= countToFill; i++)
                {
                    visits.Add(new KeyPointVisit(i, DateTime.UtcNow));
                }
            }

            // --- CRITICAL FIX START ---
            // Force EF Core to detect that the entity is modified.
            // This is required because we bypassed the public interface using Reflection.
            dbContext.Entry(execution).State = EntityState.Modified;

            // Also explicitly tell EF that the navigation collection has changed/is modified
            // (Only necessary if KeyPointVisit is a separate entity/table, but good practice here)
            // If KeyPointVisit is a value object (Owned Type), setting State=Modified above is usually enough.
            // If it's a separate table, we might need to add them to the context, but let's try the simple fix first.

            dbContext.SaveChanges();

            // Detach the entity to ensure the Controller fetches a fresh copy from the DB
            // This guarantees the test proves the data was actually saved.
            dbContext.Entry(execution).State = EntityState.Detached;
            // --- CRITICAL FIX END ---
        }

        var newEntity = new TourReviewDto
        {
            TourId = -1,
            ExecutionId = -1,
            Rating = 5,
            Comment = "Odlična tura, preporučujem!",
            Images = new List<string> { "new_image.jpg" },
            TouristId = -23,
            ReviewDate = DateTime.UtcNow,
            CompletedPercentage = 100
        };

        // Act
        var result = (ObjectResult)controller.RateTour(newEntity).Result;

        // Assert
        result.ShouldNotBeNull();

        // If this still fails, I've added a debugging step below to print the error
        if (result.StatusCode == 400)
        {
            // This will show up in the Test Output if it fails
            throw new Exception($"Controller returned 400. Message: {result.Value}");
        }

        result.StatusCode.ShouldBe(200);

        var response = result.Value as TourReviewDto;
        response.ShouldNotBeNull();
        response.TourId.ShouldBe(-1);
        response.Comment.ShouldBe(newEntity.Comment);
        response.Rating.ShouldBe(newEntity.Rating);

        // Assert - Database check
        // Re-fetch context for assertion to ensure we aren't reading cached data
        var tourInDb = dbContext.Tours
            .Include(t => t.Reviews)
            .FirstOrDefault(t => t.Id == -1);

        tourInDb.ShouldNotBeNull();
        var reviewInDb = tourInDb.Reviews.FirstOrDefault(r => r.Comment == "Odlična tura, preporučujem!");
        reviewInDb.ShouldNotBeNull();
        reviewInDb.Rating.ShouldBe(5);
    }

    [Fact]
    public void Create_fails_invalid_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var newEntity = new TourReviewDto
        {
            TourId = -1,
            ExecutionId = -1,
            Rating = 10, // Neispravna ocena (mora biti 1-5)
            Comment = "Test"
        };

        // Act
        var result = (ObjectResult)controller.RateTour(newEntity).Result;

        // Assert
        result.ShouldNotBeNull();
        // Sada će ovo proći jer smo u kontroler dodali try-catch koji vraća 400
        result.StatusCode.ShouldBe(400);
    }

    private static TourController CreateController(IServiceScope scope, string userId)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>(),
            scope.ServiceProvider.GetRequiredService<IPaymentRecordService>(),
            scope.ServiceProvider.GetRequiredService<ITourExecutionService>()
        )
        {
            ControllerContext = BuildContext(userId)
        };
    }
}