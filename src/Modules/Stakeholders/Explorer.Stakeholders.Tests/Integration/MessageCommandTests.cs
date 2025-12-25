using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Stakeholders;
using Explorer.Stakeholders.API.Dtos.Messages;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class MessageCommandTests : BaseStakeholdersIntegrationTest
    {
        public MessageCommandTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Sends_message()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");

            var dto = new SendMessageDto
            {
                ReceiverId = 2,
                Content = "Hello Bob"
            };

            var result = controller.Send(dto).Result.ShouldBeOfType<OkObjectResult>();
            var message = result.Value.ShouldBeOfType<MessageDto>();

            message.SenderId.ShouldBe(1);
            message.ReceiverId.ShouldBe(2);
            message.Content.ShouldBe("Hello Bob");
        }

        [Fact]
        public void Edits_own_message()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");

            var dto = new SendMessageDto
            {
                ReceiverId = 2,
                Content = "Original"
            };

            var sent = controller.Send(dto).Result.ShouldBeOfType<OkObjectResult>()
                .Value.ShouldBeOfType<MessageDto>();

            var updated = controller.Edit(sent.Id, new SendMessageDto
            {
                Content = "Edited"
            }).Result.ShouldBeOfType<OkObjectResult>()
              .Value.ShouldBeOfType<MessageDto>();

            updated.Content.ShouldBe("Edited");
        }

        [Fact]
        public void Edit_fails_for_other_user()
        {
            using var scope = Factory.Services.CreateScope();
            var controller1 = CreateController(scope, "1");
            var controller2 = CreateController(scope, "2");

            var sent = controller1.Send(new SendMessageDto
            {
                ReceiverId = 2,
                Content = "Private"
            }).Result.ShouldBeOfType<OkObjectResult>()
              .Value.ShouldBeOfType<MessageDto>();

            Should.Throw<Exception>(() =>
                controller2.Edit(sent.Id, new SendMessageDto { Content = "Hack" }));
        }

        [Fact]
        public void Deletes_message()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "1");
            var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var sent = controller.Send(new SendMessageDto
            {
                ReceiverId = 2,
                Content = "Bye"
            }).Result.ShouldBeOfType<OkObjectResult>()
              .Value.ShouldBeOfType<MessageDto>();

            controller.Delete(sent.Id).ShouldBeOfType<NoContentResult>();

            var deleted = db.Messages.Find(sent.Id);
            deleted.ShouldNotBeNull();
            deleted.IsDeleted.ShouldBeTrue();
        }

        private static MessageController CreateController(IServiceScope scope, string userId)
        {
            return new MessageController(
                scope.ServiceProvider.GetRequiredService<IMessageService>())
            {
                ControllerContext = BuildContext(userId)
            };
        }

        private static ControllerContext BuildContext(string id)
        {
            var claims = new List<Claim>
            {
                new Claim("personId", id),
                new Claim("id", id)
            };

            var identity = new ClaimsIdentity(claims, "test");
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }
    }
}
