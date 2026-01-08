using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class BundleCommandTests : BaseToursIntegrationTest
    {
        public BundleCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new CreateBundleDto
            {
                Name = "New Test Bundle",
                Price = 180.00m,
                TourIds = new List<long> { -1, -2 }
            };

            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as BundleDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Name.ShouldBe(newEntity.Name);
            result.Price.ShouldBe(newEntity.Price);
            result.Status.ShouldBe("Draft");
            result.AuthorId.ShouldBe(-11);

            // Assert - Database
            var storedEntity = dbContext.Bundles.FirstOrDefault(b => b.Name == newEntity.Name);
            storedEntity.ShouldNotBeNull();
            storedEntity.Status.ToString().ShouldBe("Draft");
        }

        [Fact]
        public void Create_fails_invalid_name()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new CreateBundleDto
            {
                Name = "",
                Price = 100.00m,
                TourIds = new List<long> { -1, -2 }
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(newEntity).Result);
        }

        [Fact]
        public void Create_fails_negative_price()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new CreateBundleDto
            {
                Name = "Test Bundle",
                Price = -10.00m,
                TourIds = new List<long> { -1, -2 }
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(newEntity).Result);
        }

        [Fact]
        public void Create_fails_empty_tour_list()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new CreateBundleDto
            {
                Name = "Test Bundle",
                Price = 100.00m,
                TourIds = new List<long>()
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(newEntity).Result);
        }

        [Fact]
        public void Create_fails_nonexistent_tours()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new CreateBundleDto
            {
                Name = "Test Bundle",
                Price = 100.00m,
                TourIds = new List<long> { -9999, -8888 }
            };

            // Act
            var result = controller.Create(newEntity).Result;

            // Assert
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Create_fails_unauthorized_tours()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new CreateBundleDto
            {
                Name = "Unauthorized Bundle",
                Price = 100.00m,
                TourIds = new List<long> { -13 }
            };

            // Act
            var result = controller.Create(newEntity).Result;

            // Assert
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void GetMyBundles()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetMyBundles().Result)?.Value as List<BundleDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.All(b => b.AuthorId == -11).ShouldBeTrue();
        }

        [Fact]
        public void GetById()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as BundleDto;

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(-1);
            result.Name.ShouldBe("Test Draft Bundle");
            result.AuthorId.ShouldBe(-11);
        }

        [Fact]
        public void GetById_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.GetById(-9999).Result;

            // Assert
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var updatedEntity = new UpdateBundleDto
            {
                Name = "Updated Bundle Name",
                Price = 200.00m,
                TourIds = new List<long> { -1, -2 }
            };

            // Act - KORISTI -6
            var result = ((ObjectResult)controller.Update(-6, updatedEntity).Result)?.Value as BundleDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(-6);
            result.Name.ShouldBe(updatedEntity.Name);
            result.Price.ShouldBe(updatedEntity.Price);

            // Assert - Database
            var storedEntity = dbContext.Bundles.FirstOrDefault(b => b.Id == -6);
            storedEntity.ShouldNotBeNull();
            storedEntity.Name.ShouldBe(updatedEntity.Name);
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new UpdateBundleDto
            {
                Name = "Test",
                Price = 100.00m,
                TourIds = new List<long> { -1 }
            };

            // Act
            var result = controller.Update(-9999, updatedEntity).Result;

            // Assert
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Update_fails_published_bundle()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new UpdateBundleDto
            {
                Name = "Try Update Published",
                Price = 200.00m,
                TourIds = new List<long> { -1, -2 }
            };

            // Act
            var result = controller.Update(-7, updatedEntity).Result;

            // Assert
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Update_fails_unauthorized_author()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12");
            var updatedEntity = new UpdateBundleDto
            {
                Name = "Try Update Other Author Bundle",
                Price = 200.00m,
                TourIds = new List<long> { -13, -14 }
            };

            // Act
            var result = controller.Update(-1, updatedEntity).Result;

            // Assert
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Publishes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var result = controller.Publish(-1);

            // Assert - Response
            result.ShouldNotBeNull();
            result.ShouldBeOfType<NoContentResult>();

            // Assert - Database
            var storedEntity = dbContext.Bundles.FirstOrDefault(b => b.Id == -1);
            storedEntity.ShouldNotBeNull();
            storedEntity.Status.ToString().ShouldBe("Published");
        }

        [Fact]
        public void Publish_fails_insufficient_published_tours()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Publish(-3);

            // Assert
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Publish_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Publish(-9999);

            // Assert
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Publish_fails_unauthorized_author()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12");

            // Act
            var result = controller.Publish(-1);

            // Assert
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Archives()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var result = controller.Archive(-2);

            // Assert - Response
            result.ShouldNotBeNull();
            ((ObjectResult)result).StatusCode.ShouldBe(200);

            // Assert - Database
            var storedEntity = dbContext.Bundles.FirstOrDefault(b => b.Id == -2);
            storedEntity.ShouldNotBeNull();
            storedEntity.Status.ToString().ShouldBe("Archived");
        }

        [Fact]
        public void Archive_fails_not_published()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Archive(-8);

            // Assert
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Archive_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Archive(-9999);

            // Assert
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Archive_fails_unauthorized_author()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12");

            // Act
            var result = controller.Archive(-2);

            // Assert
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var result = controller.Delete(-4);

            // Assert - Response
            result.ShouldNotBeNull();
            result.ShouldBeOfType<NoContentResult>();

            // Assert - Database
            var storedEntity = dbContext.Bundles.FirstOrDefault(b => b.Id == -4);
            storedEntity.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Delete(-9999);

            // Assert
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Delete_fails_published_bundle()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Delete(-2);

            // Assert
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Delete_fails_unauthorized_author()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12");

            // Act
            var result = controller.Delete(-1);

            // Assert
            result.ShouldBeOfType<ForbidResult>();
        }

        private static BundleController CreateController(IServiceScope scope, string authorId = "-11")
        {
            return new BundleController(scope.ServiceProvider.GetRequiredService<IBundleService>())
            {
                ControllerContext = BuildContext(authorId)
            };
        }
    }
}