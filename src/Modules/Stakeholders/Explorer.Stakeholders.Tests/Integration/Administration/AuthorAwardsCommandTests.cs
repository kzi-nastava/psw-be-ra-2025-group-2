using AutoMapper;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public.Administration;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class AuthorAwardsCommandTests : BaseStakeholdersIntegrationTest
    {

        public AuthorAwardsCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var newEntity = new AuthorAwardsDto
            {
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2035,
                State = "Draft",
                VotingStartDate = new DateOnly(2035, 9, 5),
                VotingEndDate = new DateOnly(2035, 9, 25)
            };

            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as AuthorAwardsDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Name.ShouldBe(newEntity.Name);
            result.Description.ShouldBe(newEntity.Description);
            result.Year.ShouldBe(newEntity.Year);
            result.State.ShouldBe(newEntity.State);
            result.VotingStartDate.ShouldBe(newEntity.VotingStartDate);
            result.VotingEndDate.ShouldBe(newEntity.VotingEndDate);

            // Assert - Database
            var storedEntity = dbContext.AuthorAwards.FirstOrDefault(i => i.Id == result.Id);
            storedEntity.ShouldNotBeNull();
        }

        [Fact]
        public void Create_fails_invalid_invariants()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var yearNotMatchingDatesEntity = new AuthorAwardsDto
            {
                Name = "FVA",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2036,
                State = "Draft",
                VotingStartDate = new DateOnly(2035, 9, 5),
                VotingEndDate = new DateOnly(2035, 9, 25)
            };

            var invalidDateIntervalEntity = new AuthorAwardsDto
            {
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2035,
                State = "Draft",
                VotingStartDate = new DateOnly(2035, 9, 25),
                VotingEndDate = new DateOnly(2035, 9, 5)
            };

            var invalidEnumEntity = new AuthorAwardsDto
            {
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2035,
                State = "BLABLABLA",
                VotingStartDate = new DateOnly(2035, 9, 5),
                VotingEndDate = new DateOnly(2035, 9, 25)
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(yearNotMatchingDatesEntity));
            Should.Throw<ArgumentException>(() => controller.Create(invalidDateIntervalEntity));
            Should.Throw<AutoMapperMappingException>(() => controller.Create(invalidEnumEntity));
        }

        [Fact]
        public void Create_fails_table_constraints()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var pastYearEntity = new AuthorAwardsDto
            {
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2023,
                State = "Draft",
                VotingStartDate = new DateOnly(2023, 9, 5),
                VotingEndDate = new DateOnly(2023, 9, 25)
            };

            var pastDateEntity = new AuthorAwardsDto
            {
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = DateTime.Today.Year,
                State = "Draft",
                VotingStartDate = new DateOnly(DateTime.Today.Year, 1, 1),
                VotingEndDate = new DateOnly(DateTime.Today.Year, 1, 2)
            };

            // Act & Assert
            Should.Throw<EntityValidationException>(() => controller.Create(pastYearEntity));
            Should.Throw<EntityValidationException>(() => controller.Create(pastDateEntity));

        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updatedEntity = new AuthorAwardsDto
            {
                Id = -3,
                Name = "Author's spotlights",
                Description = "Recognizes stars in the making",
                Year = 2026,
                State = "Draft",
                VotingStartDate = new DateOnly(2026, 3, 10),
                VotingEndDate = new DateOnly(2026, 3, 20)
            };

            // Act
            var result = ((ObjectResult)controller.Create(updatedEntity).Result)?.Value as AuthorAwardsDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(updatedEntity.Id);
            result.Name.ShouldBe(updatedEntity.Name);
            result.Description.ShouldBe(updatedEntity.Description);
            result.Year.ShouldBe(updatedEntity.Year);
            result.State.ShouldBe(updatedEntity.State);
            result.VotingStartDate.ShouldBe(updatedEntity.VotingStartDate);
            result.VotingEndDate.ShouldBe(updatedEntity.VotingEndDate);

            // Assert - Database
            var storedEntity = dbContext.AuthorAwards.FirstOrDefault(i => i.Name == updatedEntity.Name);
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBe(result.Id);
        }

        [Fact]
        public void Update_fails_invalid_invariants()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var yearNotMatchingDatesEntity = new AuthorAwardsDto
            {
                Id = -1,
                Name = "FVA",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2036,
                State = "Draft",
                VotingStartDate = new DateOnly(2035, 9, 5),
                VotingEndDate = new DateOnly(2035, 9, 25)
            };

            var invalidDateIntervalEntity = new AuthorAwardsDto
            {
                Id = -1,
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2035,
                State = "Draft",
                VotingStartDate = new DateOnly(2035, 9, 25),
                VotingEndDate = new DateOnly(2035, 9, 5)
            };

            var invalidEnumEntity = new AuthorAwardsDto
            {
                Id = -1,
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2035,
                State = "BLABLABLA",
                VotingStartDate = new DateOnly(2035, 9, 5),
                VotingEndDate = new DateOnly(2035, 9, 25)
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Update(yearNotMatchingDatesEntity));
            Should.Throw<ArgumentException>(() => controller.Update(invalidDateIntervalEntity));
            Should.Throw<AutoMapperMappingException>(() => controller.Update(invalidEnumEntity));
        }

        [Fact]
        public void Update_fails_table_constraints()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var pastYearEntity = new AuthorAwardsDto
            {
                Id = -1,
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = 2023,
                State = "Draft",
                VotingStartDate = new DateOnly(2023, 9, 5),
                VotingEndDate = new DateOnly(2023, 9, 25)
            };

            var pastDateEntity = new AuthorAwardsDto
            {
                Id = -1,
                Name = "Future Voices Award",
                Description = "Celebrating innovative authors of the next generation",
                Year = DateTime.Today.Year,
                State = "Draft",
                VotingStartDate = new DateOnly(DateTime.Today.Year, 1, 1),
                VotingEndDate = new DateOnly(DateTime.Today.Year, 1, 2)
            };

            // Act & Assert
            Should.Throw<EntityValidationException>(() => controller.Create(pastYearEntity));
            Should.Throw<EntityValidationException>(() => controller.Create(pastDateEntity));

        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            // Act
            var result = (OkResult)controller.Delete(-3);

            // Assert - Response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Assert - Database
            var storedCourse = dbContext.AuthorAwards.FirstOrDefault(i => i.Id == -3);
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

        private static AuthorAwardsController CreateController(IServiceScope scope)
        {
            return new AuthorAwardsController(scope.ServiceProvider.GetRequiredService<IAuthorAwardsService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
